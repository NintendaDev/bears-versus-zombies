namespace SampleGame.Gameplay.GameContext
{
    public sealed class InputAccumulatorController : SceneGameCycleObserver
    {
        private InputAccumulator _inputAccumulator;

        public override void Spawned()
        {
            base.Spawned();
            _inputAccumulator = GameContextService.Instance.Get<InputAccumulator>();
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