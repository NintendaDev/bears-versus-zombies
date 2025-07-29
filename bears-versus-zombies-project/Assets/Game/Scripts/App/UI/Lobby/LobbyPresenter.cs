using Cysharp.Threading.Tasks;
using Modules.EventBus;
using Modules.LoadingCurtain;
using Modules.Services;
using Sirenix.OdinInspector;
using UnityEngine;

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

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }
        
        public UniTask InitializeAsync()
        {
            if (_isInitialized)
                return UniTask.CompletedTask;
            
            _gameFacade = ServiceLocator.Instance.Get<GameFacade>();
            _signalBus = ServiceLocator.Instance.Get<ISignalBus>();
            _loadingCurtain = ServiceLocator.Instance.Get<ILoadingCurtain>();
            
            _view.Initialize(this);
            _view.DisableSessionConnectButton();
            _isInitialized = true;
            
            Subscribe();
            
            return UniTask.CompletedTask;
        }

        private void Subscribe()
        {
            if (_isInitialized == false)
                return;
            
            _view.BackClicked += OnBackClick;
            _view.SessionConnectClicked += OnSessionConnectClick;
            _view.CreateSessionClicked += OnCreateSessionClick;
            _signalBus.Subscribe<SessionButtonClickSignal>(OnSessionButtonClick);
            _signalBus.Subscribe<SessionButtonDropSignal>(OnSessionButtonDestroy);
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
        
        private void Unsubscribe()
        {
            if (_isInitialized == false)
                return;
            
            _view.BackClicked -= OnBackClick;
            _view.SessionConnectClicked -= OnSessionConnectClick;
            _view.CreateSessionClicked -= OnCreateSessionClick;
            _signalBus.Unsubscribe<SessionButtonClickSignal>(OnSessionButtonClick);
            _signalBus.Unsubscribe<SessionButtonDropSignal>(OnSessionButtonDestroy);
        }
    }
}