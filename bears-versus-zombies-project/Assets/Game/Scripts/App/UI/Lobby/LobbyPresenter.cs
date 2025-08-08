using Modules.EventBus;
using Modules.LoadingCurtain;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace SampleGame.App.UI
{
    public sealed class LobbyPresenter : MonoBehaviour
    {
        [SerializeField, Required] private LobbyMenuView _view;
        [SerializeField, Required] private SessionRowListPresenter _sessionRowsListPresenter;
        
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
            
            _view.Initialize();
            _view.DisableSessionConnectButton();
            
            _view.BackClicked
                .Subscribe((_) => OnBackClick())
                .AddTo(this);
            
            _view.SessionConnectClicked
                .Subscribe((_) => OnSessionConnectClick())
                .AddTo(this);
            
            _view.CreateSessionClicked
                .Subscribe(OnCreateSessionClick)
                .AddTo(this);
        }

        private void OnEnable()
        {
            _signalBus.Subscribe<SessionRowClickSignal>(OnSessionButtonClick);
            _signalBus.Subscribe<SessionRowDropSignal>(OnSessionButtonDestroy);
        }

        private void OnDisable()
        {
            _signalBus.Unsubscribe<SessionRowClickSignal>(OnSessionButtonClick);
            _signalBus.Unsubscribe<SessionRowDropSignal>(OnSessionButtonDestroy);
        }

        public void Show()
        {
            _sessionRowsListPresenter.Show();
            _view.Show();
        }

        public void Hide()
        {
            _sessionRowsListPresenter.Hide();
            _view.Hide();
        }

        private void OnBackClick() => SendMainMenuSignal();

        private void SendMainMenuSignal() => _signalBus.Invoke<ShowMainMenuSignal>();

        private async void OnSessionConnectClick()
        {
            _loadingCurtain.Show();

            if (await _gameFacade.TryConnectToMultiplayerGame(_selectedSessionName) == false)
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

        private void OnSessionButtonClick(SessionRowClickSignal signal)
        {
            _selectedSessionName = signal.SessionName;
            _view.EnableSessionConnectButton(signal.SessionName);
        }

        private void OnSessionButtonDestroy(SessionRowDropSignal signal)
        {
            if (_selectedSessionName != signal.SessionName)
                return;

            _selectedSessionName = string.Empty;
            _view.DisableSessionConnectButton();
        }
    }
}