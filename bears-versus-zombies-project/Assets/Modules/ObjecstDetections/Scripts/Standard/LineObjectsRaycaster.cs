using Sirenix.OdinInspector;
using UnityEngine;

namespace Modules.ObjectsDetection
{
    public sealed class LineObjectsRaycaster : ObjectsRaycaster
    {
        [SerializeField, MinValue(0)] private float _distance = 100f;
        
        [SerializeField] 
        private QueryTriggerInteraction _triggerInteraction = QueryTriggerInteraction.Ignore;
        
        private RaycastHit[] _hits;
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position + Offset, 
                transform.position + Offset + transform.forward * _distance);
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
            
            int hitsCount = Runner.GetPhysicsScene().Raycast(CastPosition, 
                SelfTransform.forward, _hits, _distance, LayerMask, _triggerInteraction);

            if (hitsCount == 0)
                return false;
            
            for (int hitIndex = 0; hitIndex < hitsCount; hitIndex++)
            {
                if (hitIndex >= data.Length)
                    break;
                
                RaycastHit hit = _hits[hitIndex];
                
                data[hitIndex] = new CollisionData { Object = hit.collider.gameObject, Point = hit.point};
            }
            
            return true;
        }

        protected override void OnUpdateMaxHits()
        {
            _hits = new RaycastHit[MaxHits];
        }
    }
}