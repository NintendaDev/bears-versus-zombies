using Fusion;
using Modules.Services;
using SampleGame.Gameplay.GameContext;
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
            GameCycle gameCycle = ServiceLocator.Instance.Get<GameCycle>();
            _health.AddDieHandler(() => gameCycle.FinishGame(FinishReason.BusDestroyed));
        }

        public Transform GetTarget() => _pointsService.NextPoint();
    }
}