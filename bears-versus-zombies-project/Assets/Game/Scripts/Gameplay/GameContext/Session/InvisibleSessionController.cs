using Fusion;
using Modules.Services;
using SampleGame.App;

namespace SampleGame.Gameplay.GameContext
{
    public sealed class InvisibleSessionController : SceneGameCycleObserver, IAfterSpawned
    {
        private GameFacade _gameFacade;

        public override void Spawned()
        {
            base.Spawned();
            
            _gameFacade = ServiceLocator.Instance.Get<GameFacade>();
        }

        void IAfterSpawned.AfterSpawned()
        {
            OnGameStateChange(CurrentGameState);
        }

        protected override void OnGameStateChange(GameState gameState, FinishReason finishReason) => 
            OnGameStateChange(gameState);

        private void OnGameStateChange(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.None:
                case GameState.Waiting:
                    _gameFacade.MakeVisibleCurrentSession();
                    break;
                
                case GameState.Finished:
                case GameState.Playing:
                    _gameFacade.MakeInvisibleCurrentSession();
                    break;
            }
        }
    }
}