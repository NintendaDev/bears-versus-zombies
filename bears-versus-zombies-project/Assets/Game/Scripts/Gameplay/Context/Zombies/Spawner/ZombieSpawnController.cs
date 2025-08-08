namespace SampleGame.Gameplay.Context
{
    public sealed class ZombieSpawnController : SceneGameCycleObserver
    {
        private ZombieSpawner _spawner;

        protected override void OnSpawnedInternal()
        {
            base.OnSpawnedInternal();
            _spawner = GameContext.Instance.Get<ZombieSpawner>();
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