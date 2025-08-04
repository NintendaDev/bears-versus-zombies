using Fusion;
using SampleGame.App;
using Zenject;

namespace SampleGame.Gameplay.GameContext
{
    public sealed class InvisibleSessionController : SceneGameCycleObserver, IAfterSpawned
    {
        private GameFacade _gameFacade;

        [Inject]
        private void Construct(GameFacade gameFacade)
        {
            _gameFacade = gameFacade;
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