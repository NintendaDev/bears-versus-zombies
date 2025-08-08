using Fusion;
using SampleGame.Gameplay.Context;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.GameObjects
{
    public sealed class Bus : NetworkBehaviour
    {
        [SerializeField, Required] private HealthComponent _health;

        [SerializeField, Required]
        private RandomPointsService _pointsService;
        
        public override void Spawned()
        {
            GameCycle gameCycle = GameContext.Instance.Get<GameCycle>();
            _health.AddDieHandler(() => gameCycle.FinishGame(FinishReason.BusDestroyed));
        }

        public Transform GetTarget() => _pointsService.NextPoint();
    }
}