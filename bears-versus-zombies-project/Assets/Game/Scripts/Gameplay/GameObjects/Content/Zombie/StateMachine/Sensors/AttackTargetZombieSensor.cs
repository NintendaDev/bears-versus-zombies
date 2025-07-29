using Fusion;
using Modules.ObjectsDetection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.GameObjects
{
    public sealed class AttackTargetZombieSensor : ZombieSensorBase
    {
        [SerializeField, Required] private AutoObjectsRaycaster _sphereObjectsRaycaster;

        public override void Spawned()
        {
            _sphereObjectsRaycaster.Hit += OnHit;
            _sphereObjectsRaycaster.EmptyHit += OnEmptyHit;
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            _sphereObjectsRaycaster.Hit -= OnHit;
            _sphereObjectsRaycaster.EmptyHit -= OnEmptyHit;
        }

        private void OnHit(CollisionData[] hits)
        {
            foreach (CollisionData hit in hits)
            {
                if (hit.IsValid == false)
                    return;
                
                HealthComponent target = hit.Object.GetComponentInParent<HealthComponent>();
                
                if (target != null && target.IsAlive)
                {
                    Blackboard.AttackTarget = target;

                    return;
                }
            }
        }

        private void OnEmptyHit()
        {
            Blackboard.AttackTarget = null;
        }
    }
}