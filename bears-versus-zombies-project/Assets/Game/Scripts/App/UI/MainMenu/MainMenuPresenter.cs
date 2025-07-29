using Cysharp.Threading.Tasks;
using Fusion.Sockets;
using Modules.EventBus;
using Modules.LoadingCurtain;
using Modules.Services;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.App.UI
{
    public sealed class MainMenuPresenter : MonoBehaviour, IMainMenuPresenter
    {
        [SerializeField, Required] private MainMenuView _view;
        [SerializeField, Required] private RegionTogglesListPresenter _regionTogglesListPresenter;
        
        private ILoadingCurtain _loadingCurtain;
        private GameFacade _gameFacade;
        private ISignalBus _signalBus;
        private bool _isShow;
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
            _loadingCurtain = ServiceLocator.Instance.Get<ILoadingCurtain>();
            _signalBus = ServiceLocator.Instance.Get<ISignalBus>();
            _view.Initialize(this);
            _isInitialized = true;

            Subscribe();
            
            return UniTask.CompletedTask;
        }

        private void Subscribe()
        {
            if (_isInitialized == false)
                return;
            
            _view.SinglePlayerClicked += OnSinglePlayerClick;
            _view.LobbyClicked += OnLobbyClick;
            _gameFacade.Disconnected += OnDisconnect;
        }

        public void OnShow()
        {
            _regionTogglesListPresenter.SelectCurrentRegion();
            _regionTogglesListPresenter.StartAutoRefresh();
            _isShow = true;
        }

        public void OnHide()
        {
            _regionTogglesListPresenter.StopAutoRefresh();
            _isShow = false;
        }

        private async void OnSinglePlayerClick()
        {
            _regionTogglesListPresenter.StopAutoRefresh();
            _loadingCurtain.Show();
            
            if (await _gameFacade.TryLoadSingleGame() == false)
            {
                _loadingCurtain.Hide();
                _regionTogglesListPresenter.StartAutoRefresh();
            }
        }

        private async void OnLobbyClick()
        {
            _regionTogglesListPresenter.StopAutoRefresh();
            _loadingCurtain.Show();

            if (await _gameFacade.TryJoinLobby())
            {
                _signalBus.Invoke<OpenLobbyMenuSignal>();
            }
            else
            {
                _regionTogglesListPresenter.StartAutoRefresh();
            }
            
            _loadingCurtain.Hide();
        }

        private void OnDisconnect(NetDisconnectReason reason)
        {
            if (_isShow == false)
                return;
            
            _loadingCurtain.Hide();
            _regionTogglesListPresenter.StartAutoRefresh();
        }

        private void Unsubscribe()
        {
            if (_isInitialized == false)
                return;
            
            _view.SinglePlayerClicked -= OnSinglePlayerClick;
            _view.LobbyClicked -= OnLobbyClick;
            _gameFacade.Disconnected -= OnDisconnect;
        }
    }
}