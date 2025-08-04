using Fusion;

namespace SampleGame.Gameplay.GameContext
{
    public abstract class SceneGameCycleObserver : SimulationBehaviour, ISpawned, IDespawned
    {
        private IGameCycleState _gameCycleState;

        protected GameState CurrentGameState => _gameCycleState.State;

        public virtual void Spawned()
        {
            _gameCycleState = GameContextService.Instance.Get<GameCycle>();
            _gameCycleState.Changed += OnGameStateChange;
        }

        public virtual void Despawned(NetworkRunner runner, bool hasState)
        {
            _gameCycleState.Changed -= OnGameStateChange;
        }

        protected abstract void OnGameStateChange(GameState gameState, FinishReason finishReason);
    }
}