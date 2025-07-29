using System;
using Fusion;
using Modules.Conditions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.GameObjects
{
    public abstract class RotationComponent : NetworkBehaviour
    {
        [SerializeField, MinValue(0)]
        private float _angularSpeed = 7f;
        
        private readonly AndCondition _condition = new();
        
        public float AngularSpeed => _angularSpeed;
        
        protected Transform SelfTransform { get; private set; }
        
        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            _condition.Dispose();
        }

        public virtual void Initialize()
        {
            SelfTransform = transform;
        }

        public void AddCondition(Func<bool> condition)
        {
            _condition.AddCondition(condition);
        }

        public virtual bool CanRotate() => _condition.IsTrue();

        public bool IsLookingAt(Vector3 direction)
        {
            return Mathf.Approximately(Quaternion.Angle(GetRotationRoot().rotation, Quaternion.LookRotation(direction)), 0);
        }

        public bool TryRotate(Vector3 direction, float deltaTime)
        {
            if (CanRotate() == false)
                return false;
            
            RotateInternal(direction, deltaTime);

            return true;
        }
        
        protected abstract void RotateInternal(Vector3 direction, float deltaTime);
        
        protected virtual Transform GetRotationRoot() => SelfTransform;
    }
}