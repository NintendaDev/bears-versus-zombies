using Modules.VFX;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.GameContext.Sound
{
    public sealed class StartGameEffectsController : SceneGameCycleObserver
    {
        [SerializeField, Required] private VisualEffect _effect;

        protected override void OnGameStateChange(GameState gameState, FinishReason finishReason)
        {
            switch (gameState)
            {
                case GameState.Playing:
                    _effect.Play();
                    break;
            }
        }
    }
}