using System;
using System.Collections.Generic;
using Fusion;
using Fusion.LagCompensation;

namespace Modules.Hitboxes
{
    public sealed class HitBoxFilter
    {
        private readonly string[] _hitBoxRootTags;
        private readonly List<Func<HitboxRoot, bool>> _hitboxNegativeFilters = new();

        public HitBoxFilter(params string[] hitBoxRootTags)
        {
            _hitBoxRootTags = hitBoxRootTags;
        }
        
        public void AddNotValidFilter(Func<HitboxRoot, bool> filter)
        {
            _hitboxNegativeFilters.Add(filter);
        }
        
        public void Filter(Query query, HashSet<HitboxRoot> candidates, HashSet<int> colliderIndices)
        {
            candidates.RemoveWhere(IsNotValidTarget);
        }

        private bool IsNotValidTarget(HitboxRoot root)
        {
            bool isValidTag = true;
                
            if (_hitBoxRootTags.Length > 0)
            {
                isValidTag = false;
                
                foreach (string rootTag in _hitBoxRootTags)
                {
                    if (root.CompareTag(rootTag))
                    {
                        isValidTag = true;

                        break;
                    }
                }
            }

            if (isValidTag == false)
                return true;

            foreach (Func<HitboxRoot, bool> negativeFilter in _hitboxNegativeFilters)
            {
                if (negativeFilter(root))
                    return true;
            }

            return false;
        }
    }
}