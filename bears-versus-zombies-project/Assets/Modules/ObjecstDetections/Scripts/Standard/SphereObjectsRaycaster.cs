using Sirenix.OdinInspector;
using UnityEngine;

namespace Modules.ObjectsDetection
{
    public sealed class SphereObjectsRaycaster : ObjectsRaycaster
    {
        [SerializeField, MinValue(0)]
        private float _radius = 100f;
        
        [SerializeField] 
        private QueryTriggerInteraction _triggerInteraction = QueryTriggerInteraction.Ignore;

        private Collider[] _hits;
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + Offset, _radius);
        }
        
        public override void Initialize()
        {
            base.Initialize();
            OnUpdateMaxHits();
        }

        protected override bool TryDetectInternal(CollisionData[] data)
        {
            if (data.Length == 0)
                return false;
            
            Clear(ref data);
            
            int hitsCount = Runner.GetPhysicsScene().OverlapSphere(CastPosition, 
                _radius, _hits, LayerMask, _triggerInteraction);
            
            if (hitsCount == 0)
                return false;
            
            for (int hitIndex = 0; hitIndex < hitsCount; hitIndex++)
            {
                if (hitIndex >= data.Length)
                    break;
                
                Collider hit = _hits[hitIndex];
                
                data[hitIndex] = new CollisionData { Object = hit.gameObject, 
                    Point = hit.transform.position};
            }

            return true;
        }

        protected override void OnUpdateMaxHits()
        {
            _hits = new Collider[MaxHits];
        }
    }
}