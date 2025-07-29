using Cysharp.Threading.Tasks;
using Fusion;
using Modules.Services;
using SampleGame.App;

namespace SampleGame.Gameplay.GameContext.Snapshots
{
    public sealed class HostMigrationSnapshotController : SceneSimulationBehaviour, ISpawned, IDespawned
    {
        private GameFacade _gameFacade;
        private PlayersService _playersService;
        private IGameCycleState _gameCycleState;
        private UniTask _migrationTask;

        void ISpawned.Spawned()
        {
            _gameFacade = ServiceLocator.Instance.Get<GameFacade>();
            _playersService = ServiceLocator.Instance.Get<PlayersService>();
            _gameCycleState = ServiceLocator.Instance.Get<IGameCycleState>();

            _playersService.PlayerJoined += OnPlayerJoin;
            _gameCycleState.Changed += OnGameCycleChange;
        }

        void IDespawned.Despawned(NetworkRunner runner, bool hasState)
        {
            _playersService.PlayerJoined -= OnPlayerJoin;
            _gameCycleState.Changed -= OnGameCycleChange;
        }

        private void OnPlayerJoin(NetworkObject obj) => PushSnapshot();

        private void PushSnapshot()
        {
            if (Runner.IsServer == false)
                return;
            
            _gameFacade.PushHostMigrationSnapshotAsync().Forget();
        }

        private void OnGameCycleChange(GameState gameState, FinishReason finishReason) => PushSnapshot();
    }
}