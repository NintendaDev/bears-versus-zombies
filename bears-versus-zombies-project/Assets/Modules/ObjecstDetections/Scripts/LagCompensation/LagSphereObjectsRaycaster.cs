using System.Collections.Generic;
using Fusion;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Modules.ObjectsDetection
{
    public sealed class LagSphereObjectsRaycaster : LagCompensationObjectsRaycaster
    {
        [SerializeField, MinValue(0)] private float _radius = 10f;
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }

        protected override int DetectInternal(List<LagCompensatedHit> hits, HitboxManager hitboxManager, 
            PlayerRef player)
        {
            return hitboxManager.OverlapSphere(
                SelfTransform.position,
                _radius,
                player, 
                hits,
                LayerMask,
                options: HitOptions.SubtickAccuracy | HitOptions.IgnoreInputAuthority,
                preProcessRoots: FilterHitBoxes
            );
        }
    }
}