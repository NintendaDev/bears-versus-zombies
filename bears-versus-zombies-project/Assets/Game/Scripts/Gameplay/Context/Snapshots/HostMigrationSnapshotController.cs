using Cysharp.Threading.Tasks;
using Fusion;
using SampleGame.App;
using Zenject;

namespace SampleGame.Gameplay.Context.Snapshots
{
    public sealed class HostMigrationSnapshotController : SimulationBehaviour, ISpawned, IDespawned
    {
        private GameFacade _gameFacade;
        private PlayersService _playersService;
        private IGameCycleState _gameCycleState;
        private UniTask _migrationTask;

        [Inject]
        private void Construct(GameFacade gameFacade)
        {
            _gameFacade = gameFacade;
        }

        void ISpawned.Spawned()
        {
            _playersService = GameContext.Instance.Get<PlayersService>();
            _gameCycleState = GameContext.Instance.Get<GameCycle>();

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