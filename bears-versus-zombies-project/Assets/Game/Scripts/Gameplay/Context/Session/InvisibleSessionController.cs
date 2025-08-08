using SampleGame.App;
using Zenject;

namespace SampleGame.Gameplay.Context
{
    public sealed class InvisibleSessionController : SceneGameCycleObserver
    {
        private GameFacade _gameFacade;

        [Inject]
        private void Construct(GameFacade gameFacade)
        {
            _gameFacade = gameFacade;
        }

        protected override void OnSpawnedInternal()
        {
            base.OnSpawnedInternal();
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