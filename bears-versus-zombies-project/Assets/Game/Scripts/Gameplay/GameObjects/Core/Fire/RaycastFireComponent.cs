using Modules.ObjectsDetection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.GameObjects
{
    public sealed class RaycastFireComponent : FireComponent
    {
        [SerializeField, Required] private LineObjectsRaycaster _raycaster;

        private const int MaxHits = 1;
        private CollisionData[] _hits;

        public void Initialize()
        {
            _hits = new CollisionData[MaxHits];
        }
        
        protected override bool TryGetTarget(out HealthComponent target)
        {
            target = null;
            
            if (_raycaster.TryDetect(_hits) == false)
                return false;

            target = _hits[0].Object.GetComponentInParent<HealthComponent>();
            
            return target != null;
        }
    }
}