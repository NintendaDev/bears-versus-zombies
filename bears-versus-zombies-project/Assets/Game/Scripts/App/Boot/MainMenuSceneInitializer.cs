using Cysharp.Threading.Tasks;
using Modules.AssetsManagement.AddressablesOperations;
using Modules.AssetsManagement.StaticData;
using Modules.EventBus;
using Modules.LoadingCurtain;
using Modules.Services;
using SampleGame.App.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.App
{
    public sealed class MainMenuSceneInitializer : SceneInitializer
    {
        [SerializeField, Required, SceneObjectsOnly] 
        private ServiceLocator _serviceLocator;
        
        [Title("Factories")]
        [SerializeField, Required, SceneObjectsOnly] 
        private RegionToggleFactory _regionToggleFactory;
        
        [SerializeField, Required, SceneObjectsOnly] 
        private LanguageToggleFactory _languageToggleFactory;
        
        [SerializeField, Required, SceneObjectsOnly] 
        private SessionButtonFactory _sessionButtonFactory;
        
        [Title("Presenters")]
        [SerializeField, Required, SceneObjectsOnly] 
        private MainMenuPresenter _mainMenuPresenter;
        
        [SerializeField, Required, SceneObjectsOnly] 
        private LobbyPresenter _lobbyPresenter;
        
        [SerializeField, Required, SceneObjectsOnly] 
        private LanguageTogglesListPresenter _languageTogglesListPresenter;
        
        [SerializeField, Required, SceneObjectsOnly] 
        private RegionTogglesListPresenter _regionTogglesListPresenter;
        
        [SerializeField, Required, SceneObjectsOnly] 
        private SessionButtonListPresenter _sessionButtonListPresenter;

        [Title("Controllers")]
        [SerializeField, Required, SceneObjectsOnly] 
        private AppMenuController _appMenuController;
        
        public override async UniTask InitializeAsync()
        {
            BindServices();
            
            await UniTask.WhenAll(
                _regionToggleFactory.InitializeAsync(),
                _languageToggleFactory.InitializeAsync(),
                _sessionButtonFactory.InitializeAsync());
            
            await UniTask.WhenAll(
                _languageTogglesListPresenter.InitializeAsync(),
                _regionTogglesListPresenter.InitializeAsync(),
                _sessionButtonListPresenter.InitializeAsync(),
                _mainMenuPresenter.InitializeAsync(),
                _lobbyPresenter.InitializeAsync());

            await UniTask.WhenAll(
                _appMenuController.InitializeAsync()
            );
            
            _serviceLocator.Get<ISignalBus>().Invoke<ShowMainMenuInstanceSignal>();
        }

        private void BindServices()
        {
            _serviceLocator.Initialize();
            
            _serviceLocator.Add(_regionToggleFactory);
            _serviceLocator.Add(_languageToggleFactory);
            _serviceLocator.Add(_sessionButtonFactory);
            
            GameLoader gameLoader = FindAnyObjectByType<GameLoader>();
            
            GameFacade gameFacade = gameLoader.GetComponentInChildren<GameFacade>();
            _serviceLocator.Add(gameFacade);

            ILoadingCurtain loadingCurtain = gameLoader.GetComponentInChildren<ILoadingCurtain>();
            _serviceLocator.Add(loadingCurtain);
            
            ISignalBus signalBus = gameLoader.GetComponentInChildren<ISignalBus>();
            _serviceLocator.Add(signalBus);
            
            GameLocalizationSystem gameLocalizationSystem = gameLoader.GetComponentInChildren<GameLocalizationSystem>();
            _serviceLocator.Add(gameLocalizationSystem);
            
            IAddressablesService addressablesService = gameLoader.GetComponentInChildren<IAddressablesService>();
            _serviceLocator.Add(addressablesService);
            
            IStaticDataService staticDataService = gameLoader.GetComponentInChildren<IStaticDataService>();
            _serviceLocator.Add(staticDataService);
        }
    }
}