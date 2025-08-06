using Modules.EventBus;
using Modules.LoadingCurtain;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace SampleGame.App.UI
{
    public sealed class LobbyPresenter : MonoBehaviour, ILobbyPresenter
    {
        [SerializeField, Required] private LobbyMenuView _view;
        [SerializeField, Required] private SessionButtonListPresenter _sessionButtonsListPresenter;
        
        private ISignalBus _signalBus;
        private GameFacade _gameFacade;
        private ILoadingCurtain _loadingCurtain;
        private string _selectedSessionName;
        private bool _isInitialized;

        [Inject]
        private void Construct(GameFacade gameFacade, ISignalBus signalBus, ILoadingCurtain loadingCurtain)
        {
            _gameFacade = gameFacade;
            _signalBus = signalBus;
            _loadingCurtain = loadingCurtain;
            
            _view.Initialize(this);
            _view.DisableSessionConnectButton();
        }

        private void OnEnable()
        {
            _view.BackClicked += OnBackClick;
            _view.SessionConnectClicked += OnSessionConnectClick;
            _view.CreateSessionClicked += OnCreateSessionClick;
            _signalBus.Subscribe<SessionButtonClickSignal>(OnSessionButtonClick);
            _signalBus.Subscribe<SessionButtonDropSignal>(OnSessionButtonDestroy);
        }

        private void OnDisable()
        {
            _view.BackClicked -= OnBackClick;
            _view.SessionConnectClicked -= OnSessionConnectClick;
            _view.CreateSessionClicked -= OnCreateSessionClick;
            _signalBus.Unsubscribe<SessionButtonClickSignal>(OnSessionButtonClick);
            _signalBus.Unsubscribe<SessionButtonDropSignal>(OnSessionButtonDestroy);
        }

        public void OnShow()
        {
            _sessionButtonsListPresenter.OnShow();
        }

        public void OnHide()
        {
            _sessionButtonsListPresenter.OnHide();
        }

        private void OnBackClick() => SendMainMenuSignal();

        private void SendMainMenuSignal() => _signalBus.Invoke<ShowMainMenuSignal>();

        private async void OnSessionConnectClick(string sessionName)
        {
            _loadingCurtain.Show();

            if (await _gameFacade.TryConnectToMultiplayerGame(sessionName) == false)
            {
                SendMainMenuSignal();
                _loadingCurtain.Hide();
            }
        }

        private async void OnCreateSessionClick(string sessionName)
        {
            _loadingCurtain.Show();

            if (await _gameFacade.TryCreateMultiplayerGame(sessionName) == false)
            {
                SendMainMenuSignal();
                _loadingCurtain.Hide();
            }
        }

        private void OnSessionButtonClick(SessionButtonClickSignal signal)
        {
            _selectedSessionName = signal.SessionName;
            _view.EnableSessionConnectButton(signal.SessionName);
        }

        private void OnSessionButtonDestroy(SessionButtonDropSignal signal)
        {
            if (_selectedSessionName != signal.SessionName)
                return;

            _selectedSessionName = string.Empty;
            _view.DisableSessionConnectButton();
        }
    }
}