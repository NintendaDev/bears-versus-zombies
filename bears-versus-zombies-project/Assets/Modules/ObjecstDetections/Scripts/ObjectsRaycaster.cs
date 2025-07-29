using System;
using Fusion;
using Modules.Conditions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Modules.ObjectsDetection
{
    public abstract class ObjectsRaycaster : NetworkBehaviour
    {
        [SerializeField, MinValue(1)] private int _maxHits = 20;
        [SerializeField] private Vector3 _offset;
        [SerializeField] private LayerMask _layerMask = ~0;

        private readonly AndCondition _condition = new();
        
        public int MaxHits => _maxHits;

        public Transform SelfTransform { get; private set; }

        protected LayerMask LayerMask => _layerMask;
        
        protected Vector3 CastPosition => SelfTransform.position + Offset;

        protected Vector3 Offset => _offset;

        public virtual void Initialize()
        {
            SelfTransform = transform;
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            _condition.Dispose();
        }

        public void AddCondition(Func<bool> condition)
        {
            _condition.AddCondition(condition);
        }

        public void SetMaxHits(int maxHits)
        {
            _maxHits = maxHits;
            OnUpdateMaxHits();
        }

        public bool TryDetect(CollisionData[] data)
        {
            if (_condition.IsTrue() == false)
                return false;

            return TryDetectInternal(data);
        }

        protected abstract bool TryDetectInternal(CollisionData[] data);
        
        protected abstract void OnUpdateMaxHits();

        protected void Clear(ref CollisionData[] data)
        {
            for (int i = 0; i < data.Length; i++)
                data[i] = default;
        }
    }
}