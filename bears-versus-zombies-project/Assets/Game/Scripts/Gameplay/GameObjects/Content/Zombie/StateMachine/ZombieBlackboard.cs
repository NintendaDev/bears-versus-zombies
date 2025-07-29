using Fusion;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.GameObjects
{
    public class ZombieBlackboard : NetworkBehaviour
    {
        [SerializeField, Required] private Zombie _zombie;

        [ShowInInspector, Sirenix.OdinInspector.ReadOnly]
        public Transform MoveTarget { get; set; }

        [ShowInInspector, Sirenix.OdinInspector.ReadOnly]
        public HealthComponent AttackTarget { get; set; }

        [ShowInInspector, Sirenix.OdinInspector.ReadOnly]
        public float DespawnDelay => (_zombie != null) ? _zombie.DespawnDelay : 0;

        [ShowInInspector, Sirenix.OdinInspector.ReadOnly]
        public int AttackDamage => (_zombie != null) ? _zombie.AttackDamage : 0;
        
        [ShowInInspector, Sirenix.OdinInspector.ReadOnly]
        public float HitStunSeconds => (_zombie != null) ? _zombie.HitStunSeconds : 0;

        public void Reset()
        {
            MoveTarget = null;
            AttackTarget = null;
        }
    }
}