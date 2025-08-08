using Fusion;

namespace SampleGame.Gameplay.Context
{
    public abstract class SceneGameCycleObserver : SimulationBehaviour, ISpawned, IDespawned
    {
        private IGameCycleState _gameCycleState;

        protected GameState CurrentGameState => _gameCycleState.State;

        public void Spawned()
        {
            _gameCycleState = GameContext.Instance.Get<GameCycle>();
            _gameCycleState.Changed += OnGameStateChange;
            OnSpawnedInternal();
            OnGameStateChange(_gameCycleState.State, _gameCycleState.FinishReason);
        }

        public virtual void Despawned(NetworkRunner runner, bool hasState)
        {
            _gameCycleState.Changed -= OnGameStateChange;
        }

        protected virtual void OnSpawnedInternal()
        {
        }

        protected abstract void OnGameStateChange(GameState gameState, FinishReason finishReason);
    }
}