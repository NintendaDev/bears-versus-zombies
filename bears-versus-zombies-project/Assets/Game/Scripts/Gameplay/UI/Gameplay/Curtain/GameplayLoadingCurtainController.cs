using Fusion;
using Modules.LoadingCurtain;
using Modules.Services;
using SampleGame.App;
using SampleGame.Gameplay.GameContext;

namespace SampleGame.Gameplay.UI.Curtain
{
    public sealed class GameplayLoadingCurtainController : SceneSimulationBehaviour, ISpawned, IDespawned
    {
        private ILoadingCurtain _loadingCurtain;
        private IGameInitializeEvent _initializeEvent;
        private GameFacade _gameFacade;

        void ISpawned.Spawned()
        {
            _loadingCurtain = ServiceLocator.Instance.Get<ILoadingCurtain>();
            _initializeEvent = ServiceLocator.Instance.Get<IGameInitializeEvent>();
            _gameFacade = ServiceLocator.Instance.Get<GameFacade>();
            
            _initializeEvent.Initialized += OnGameInitialized;
            _gameFacade.HostMigrationStarted += OnHostMigrationStart;
            _gameFacade.HostMigrationFinished += OnHostMigrationFinish;
        }

        void IDespawned.Despawned(NetworkRunner runner, bool hasState)
        {
            _initializeEvent.Initialized -= OnGameInitialized;
            _gameFacade.HostMigrationStarted -= OnHostMigrationStart;
            _gameFacade.HostMigrationFinished -= OnHostMigrationFinish;
        }

        private void OnGameInitialized()
        {
            _loadingCurtain.Hide();
        }

        private void OnHostMigrationStart()
        {
            _loadingCurtain.Show();
        }

        private void OnHostMigrationFinish()
        {
            _loadingCurtain.Hide();
        }
    }
}