using Fusion;
using Modules.Services;

namespace SampleGame.Gameplay.GameContext
{
    public abstract class SceneGameCycleObserver : SceneSimulationBehaviour, ISpawned, IDespawned
    {
        private IGameCycleState _gameCycleState;

        protected GameState CurrentGameState => _gameCycleState.State;

        public virtual void Spawned()
        {
            _gameCycleState = ServiceLocator.Instance.Get<IGameCycleState>();
            _gameCycleState.Changed += OnGameStateChange;
        }

        public virtual void Despawned(NetworkRunner runner, bool hasState)
        {
            _gameCycleState.Changed -= OnGameStateChange;
        }

        protected abstract void OnGameStateChange(GameState gameState, FinishReason finishReason);
    }
}