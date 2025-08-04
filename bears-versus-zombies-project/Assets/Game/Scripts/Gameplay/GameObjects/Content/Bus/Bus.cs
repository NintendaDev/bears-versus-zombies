using Fusion;
using SampleGame.Gameplay.GameContext;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.GameObjects
{
    public sealed class Bus : NetworkBehaviour, ISpawned
    {
        [SerializeField, Required] private HealthComponent _health;

        [SerializeField, Required]
        private RandomPointsService _pointsService;
        
        void ISpawned.Spawned()
        {
            GameCycle gameCycle = GameContextService.Instance.Get<GameCycle>();
            _health.AddDieHandler(() => gameCycle.FinishGame(FinishReason.BusDestroyed));
        }

        public Transform GetTarget() => _pointsService.NextPoint();
    }
}