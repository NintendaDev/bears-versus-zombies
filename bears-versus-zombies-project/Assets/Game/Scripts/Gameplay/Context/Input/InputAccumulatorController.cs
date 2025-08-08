namespace SampleGame.Gameplay.Context
{
    public sealed class InputAccumulatorController : SceneGameCycleObserver
    {
        private InputAccumulator _inputAccumulator;

        protected override void OnSpawnedInternal()
        {
            base.OnSpawnedInternal();
            _inputAccumulator = GameContext.Instance.Get<InputAccumulator>();
        }

        protected override void OnGameStateChange(GameState gameState, FinishReason finishReason)
        {
            if (gameState == GameState.Finished)
                _inputAccumulator.Disable();
            else
                _inputAccumulator.Enable();
        }
    }
}