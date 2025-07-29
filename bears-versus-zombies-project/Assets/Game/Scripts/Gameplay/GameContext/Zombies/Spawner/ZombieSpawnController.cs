using Modules.Services;

namespace SampleGame.Gameplay.GameContext
{
    public sealed class ZombieSpawnController : SceneGameCycleObserver
    {
        private ZombieSpawner _spawner;

        public override void Spawned()
        {
            base.Spawned();
            
            _spawner = ServiceLocator.Instance.Get<ZombieSpawner>();
        }

        protected override void OnGameStateChange(GameState gameState, FinishReason finishReason)
        {
            switch (gameState)
            {
                case GameState.Playing:
                    _spawner.Enable();
                    break;
                
                case GameState.Finished:
                    _spawner.Disable();
                    break;
            }
        }
    }
}