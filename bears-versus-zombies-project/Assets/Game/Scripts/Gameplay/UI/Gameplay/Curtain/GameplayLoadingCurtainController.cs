using Fusion;
using Modules.LoadingCurtain;
using SampleGame.App;
using SampleGame.Gameplay.GameContext;
using Zenject;

namespace SampleGame.Gameplay.UI.Curtain
{
    public sealed class GameplayLoadingCurtainController : SimulationBehaviour, ISpawned, IDespawned
    {
        private ILoadingCurtain _loadingCurtain;
        private IGameInitializeEvent _initializeEvent;
        private GameFacade _gameFacade;

        [Inject]
        private void Construct(GameFacade gameFacade, ILoadingCurtain loadingCurtain)
        {
            _gameFacade = gameFacade;
            _loadingCurtain = loadingCurtain;
        }

        void ISpawned.Spawned()
        {
            _initializeEvent = GameContextService.Instance.Get<GameInitializeEventer>();
            
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