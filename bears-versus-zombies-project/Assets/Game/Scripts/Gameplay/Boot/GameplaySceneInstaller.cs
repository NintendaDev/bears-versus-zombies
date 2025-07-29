using Cysharp.Threading.Tasks;
using Modules.AssetsManagement.AddressablesOperations;
using Modules.AssetsManagement.StaticData;
using Modules.EventBus;
using Modules.LoadingCurtain;
using Modules.Services;
using Modules.Wallet;
using SampleGame.App;
using SampleGame.App.SceneManagement;
using SampleGame.Gameplay.GameContext;
using SampleGame.Gameplay.GameObjects;
using SampleGame.Gameplay.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.Boot
{
    public sealed class GameplaySceneInstaller : SceneInitializer
    {
        [SerializeField, Required, SceneObjectsOnly] 
        private ServiceLocator _serviceLocator;

        [Title("Context")]
        [SerializeField, Required, SceneObjectsOnly] 
        private Transform _worldContainer;
        
        [SerializeField, Required, SceneObjectsOnly] 
        private Wallet _wallet;
        
        [SerializeField, Required, SceneObjectsOnly] 
        private GameCycle _gameCycle;

        [SerializeField, Required, SceneObjectsOnly]
        private GameInitializeEventer _gameInitializeEvent;
        
        [SerializeField, Required, SceneObjectsOnly] 
        private InputAccumulatorController _inputAccumulatorController;
        
        [SerializeField, Required, SceneObjectsOnly] 
        private PlayerCameraProvider _playerCameraProvider;
        
        [SerializeField, Required, SceneObjectsOnly] 
        private PlayersWaiter _playersWaiter;
        
        [Title("Zombies")]
        [SerializeField, Required, SceneObjectsOnly] 
        private ZombieSpawnController _zombieSpawnController;
        
        [SerializeField, Required, SceneObjectsOnly] 
        private ZombieFactory _zombieFactory;
        
        [SerializeField, Required]
        private ZombieConfig _zombieConfig;
        
        [SerializeField, Required, SceneObjectsOnly] 
        private ZombieSpawner _zombieSpawner;
        
        [Title("Traps")]
        [SerializeField, Required]
        private TrapsSettings _trapsSettings;
        
        [SerializeField, Required, SceneObjectsOnly] 
        private TrapFactory _trapFactory;
        
        [Title("UI")]
        [SerializeField, Required, SceneObjectsOnly] 
        private GameplayUIController _gameplayUIController;

        private GameFacade _gameFacade;

        public override UniTask InitializeAsync()
        {
            BindServices();
            
            return UniTask.CompletedTask;
        }
        
        private void BindServices()
        {
            _serviceLocator.Initialize();
            
            GameLoader gameLoader = FindAnyObjectByType<GameLoader>();
            
            GameFacade gameFacade = gameLoader.GetComponentInChildren<GameFacade>();
            _serviceLocator.Add(gameFacade);
            _serviceLocator.Add(gameFacade.Runner);

            ILoadingCurtain loadingCurtain = gameLoader.GetComponentInChildren<ILoadingCurtain>();
            _serviceLocator.Add(loadingCurtain);
            
            ISignalBus signalBus = gameLoader.GetComponentInChildren<ISignalBus>();
            _serviceLocator.Add(signalBus);
            
            GameplayToMenuLoader gameplayToMenuLoader = gameLoader.GetComponentInChildren<GameplayToMenuLoader>();
            _serviceLocator.Add(gameplayToMenuLoader);
            
            GameLocalizationSystem gameLocalizationSystem = gameLoader.GetComponentInChildren<GameLocalizationSystem>();
            _serviceLocator.Add(gameLocalizationSystem);
            
            IAddressablesService addressablesService = gameLoader.GetComponentInChildren<IAddressablesService>();
            _serviceLocator.Add(addressablesService);
            
            IStaticDataService staticDataService = gameLoader.GetComponentInChildren<IStaticDataService>();
            _serviceLocator.Add(staticDataService);
            
            PlayersService playersService = gameFacade.Runner.GetComponentInChildren<PlayersService>();
            _serviceLocator.Add(playersService);
            
            InputAccumulator inputAccumulator = gameFacade.Runner.GetComponentInChildren<InputAccumulator>();
            _serviceLocator.Add(inputAccumulator);
            
            _serviceLocator.Add(_zombieSpawnController);
            _serviceLocator.Add(_inputAccumulatorController);
            _serviceLocator.Add(_playerCameraProvider);
            _serviceLocator.Add(_playersWaiter);
            _serviceLocator.Add(_zombieSpawner);
            _serviceLocator.Add(_zombieFactory);
            _serviceLocator.Add(_zombieConfig);
            _serviceLocator.Add(_trapsSettings);
            _serviceLocator.Add(_wallet);
            _serviceLocator.Add(_gameCycle);
            _serviceLocator.Add<IGameCycleState>(_gameCycle);
            _serviceLocator.Add(_trapFactory);
            _serviceLocator.Add<IGameInitializeEvent>(_gameInitializeEvent);
            _serviceLocator.Add(_gameplayUIController);
        }
    }
}