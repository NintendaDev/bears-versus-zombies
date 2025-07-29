using System;
using Fusion;
using Modules.ObjectsDetection;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace SampleGame.Gameplay.GameObjects
{
    public sealed class AutoFireComponent : NetworkBehaviour
    {
        [FormerlySerializedAs("_autoObjectsDetector")] [SerializeField, Required] private AutoObjectsRaycaster _autoObjectsRaycaster;
        [SerializeField, Required] private FireComponent _fireComponent;
        [SerializeField, Required] private RotationComponent _rotationComponent;
        
        [ShowInInspector, Sirenix.OdinInspector.ReadOnly]
        private Transform _nearestTarget;
        private Transform _transform;

        public bool HasTarget => _nearestTarget != null;

        public void Initialize()
        {
            _autoObjectsRaycaster.TryAddNotValidFilter((hitboxRoot) => 
                hitboxRoot.GetComponent<HealthComponent>().IsAlive == false);
            
            _transform = transform;
        }

        public override void Spawned()
        {
            _autoObjectsRaycaster.Hit += OnHit;
            _autoObjectsRaycaster.EmptyHit += OnEmptyHit;
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            _autoObjectsRaycaster.Hit -= OnHit;
            _autoObjectsRaycaster.EmptyHit -= OnEmptyHit;
        }

        public override void FixedUpdateNetwork()
        {
            if (_nearestTarget == null)
                return;
            
            Vector3 direction = (_nearestTarget.transform.position - _transform.position).normalized;
            direction.y = 0;
            _rotationComponent.TryRotate(direction, Time.deltaTime);
            
            if (_rotationComponent.IsLookingAt(direction))
                _fireComponent.Fire();
        }

        public void AddCondition(Func<bool> condition)
        {
            _fireComponent.AddCondition(condition);
            _autoObjectsRaycaster.AddCondition(condition);
        }

        private void OnHit(CollisionData[] hits)
        {
            _nearestTarget = FindNearest(hits);
        }

        private Transform FindNearest(CollisionData[] hits)
        {
            float minDistance = float.MaxValue;
            CollisionData nearestHit = default;
            
            foreach (CollisionData hit in hits)
            {
                if (hit.IsValid == false)
                    continue;
                
                float hitDistance = Vector3.Distance(_transform.position, hit.Point);
                
                if (hitDistance < minDistance)
                {
                    minDistance = hitDistance;
                    nearestHit = hit;
                }
            }
            
            return nearestHit.Object.transform;
        }

        private void OnEmptyHit()
        {
            _nearestTarget = null;
        }
    }
}