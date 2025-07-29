using System;
using System.Collections.Generic;
using Fusion;
using Fusion.LagCompensation;
using Modules.Hitboxes;
using UnityEngine;

namespace Modules.ObjectsDetection
{
    public abstract class LagCompensationObjectsRaycaster : ObjectsRaycaster
    {
        [SerializeField] private string[] _hitboxRootTags = new string[] { "Enemy" };

        private List<LagCompensatedHit> _hits;
        private HitBoxFilter _hitboxFilter;

        public override void Initialize()
        {
            base.Initialize();
            OnUpdateMaxHits();
            _hitboxFilter = new HitBoxFilter(_hitboxRootTags);
        }
        
        public void AddNotValidFilter(Func<HitboxRoot, bool> filter)
        {
            _hitboxFilter.AddNotValidFilter(filter);
        }
        
        protected sealed override bool TryDetectInternal(CollisionData[] data)
        {
            if (data.Length == 0)
                return false;
            
            Clear(ref data);
            
            HitboxManager hitboxManager = Runner.LagCompensation;
            
            PlayerRef player = Object.InputAuthority;
            int hitCount = DetectInternal(_hits, hitboxManager, player);
            
            if (hitCount == 0)
                return false;

            for (int hitIndex = 0; hitIndex < hitCount; hitIndex++)
            {
                if (hitIndex >= data.Length)
                    break;
                
                data[hitIndex] = new CollisionData { Object = _hits[hitIndex].Hitbox.gameObject, 
                    Point = _hits[hitIndex].Point};
            }
            
            return true;
        }

        protected abstract int DetectInternal(List<LagCompensatedHit> hits, HitboxManager hitboxManager, 
            PlayerRef player);

        protected void FilterHitBoxes(Query query, HashSet<HitboxRoot> candidates, HashSet<int> colliderIndices)
        {
            _hitboxFilter.Filter(query, candidates, colliderIndices);
        }
        
        protected sealed override void OnUpdateMaxHits()
        {
            _hits = new List<LagCompensatedHit>(MaxHits);
        }
    }
}