using Modules.EventBus;
using Modules.LoadingCurtain;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace SampleGame.App.UI
{
    public sealed class MainMenuPresenter : MonoBehaviour
    {
        [SerializeField, Required] private MainMenuView _view;
        [SerializeField, Required] private RegionTogglesListPresenter _regionTogglesListPresenter;
        
        private ILoadingCurtain _loadingCurtain;
        private GameFacade _gameFacade;
        private ISignalBus _signalBus;
        private bool _isShow;
        private bool _isInitialized;

        [Inject]
        private void Construct(GameFacade gameFacade, ILoadingCurtain loadingCurtain, ISignalBus signalBus)
        {
            _gameFacade = gameFacade;
            _loadingCurtain = loadingCurtain;
            _signalBus = signalBus;
            
            _view.Initialize();
            
            _view.SinglePlayerClicked
                .Subscribe((_) => OnSinglePlayerClick())
                .AddTo(this);
            
            _view.LobbyClicked
                .Subscribe((_) => OnLobbyClick())
                .AddTo(this);
            
            _gameFacade.Disconnected
                .Subscribe((_) => OnDisconnect())
                .AddTo(this);
        }

        public void Show()
        {
            _regionTogglesListPresenter.SelectCurrentRegion();
            _regionTogglesListPresenter.StartAutoRefresh();
            _view.Show();
            _isShow = true;
        }

        public void ShowInstance()
        {
            _regionTogglesListPresenter.SelectCurrentRegion();
            _regionTogglesListPresenter.StartAutoRefresh();
            _view.ShowInstance();
            _isShow = true;
        }

        public void Hide()
        {
            _regionTogglesListPresenter.StopAutoRefresh();
            _view.Hide();
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

        private void OnDisconnect()
        {
            if (_isShow == false)
                return;
            
            _loadingCurtain.Hide();
            _regionTogglesListPresenter.StartAutoRefresh();
        }
    }
}