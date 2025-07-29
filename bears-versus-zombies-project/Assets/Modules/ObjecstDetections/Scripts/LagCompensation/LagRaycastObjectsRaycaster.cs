using System.Collections.Generic;
using Fusion;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Modules.ObjectsDetection
{
    public sealed class LagRaycastObjectsRaycaster : LagCompensationObjectsRaycaster
    {
        [SerializeField, MinValue(0)] private float _distance = 10f;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Vector3 position = transform.position;
            Gizmos.DrawLine(position,  position + transform.forward * _distance);
        }

        protected override int DetectInternal(List<LagCompensatedHit> hits, HitboxManager hitboxManager, 
            PlayerRef player)
        {
            return hitboxManager.RaycastAll(
                SelfTransform.position,
                SelfTransform.forward,
                _distance,
                player, 
                hits,
                LayerMask,
                options: HitOptions.SubtickAccuracy | HitOptions.IgnoreInputAuthority,
                preProcessRoots: FilterHitBoxes
            );
        }
    }
}