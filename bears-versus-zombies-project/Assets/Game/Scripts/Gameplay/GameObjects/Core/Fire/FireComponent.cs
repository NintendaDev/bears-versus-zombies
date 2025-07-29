using Modules.ObjectsDetection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.GameObjects
{
    public class LagFireComponent : FireComponent
    {
        [SerializeField, Required, ChildGameObjectsOnly] 
        private LagRaycastObjectsRaycaster _raycaster;

        private CollisionData[] _hits;

        public void Initialize()
        {
            _hits = new CollisionData[_raycaster.MaxHits];
            _raycaster.AddNotValidFilter((root) => root.GetComponent<HealthComponent>().IsAlive == false);
        }

        protected override bool TryGetTarget(out HealthComponent target)
        {
            target = null;
            
            if (_raycaster.TryDetect(_hits))
                target = _hits[0].Object.GetComponentInParent<HealthComponent>();

            return target != null;
        }
    }
}