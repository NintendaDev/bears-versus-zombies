using System.Collections.Generic;
using OM.Animora.Runtime;
using UnityEngine;

namespace OM.Animora.Modules
{
    [System.Serializable]
    [AnimoraCreate("Set Rotation", "Transform/Set Rotation")]
    [AnimoraIcon("AnimationClip Icon")]
    [AnimoraKeywords("Set", "Rotation", "action")]
    [AnimoraDescription("This is a set Rotation action")]
    [AnimoraAction(typeof(Transform))]
    public class AnimoraActionSetRotation : AnimoraActionWithTargets<Transform>
    {
        [SerializeField] private Space space = Space.World;
        [SerializeField] private AnimoraValue<Vector3> rotation;
        
        public override void OnTriggerAction<TTarget>(IEnumerable<TTarget> targets, AnimoraPlayer animoraPlayer, AnimoraClip clip)
        {
            foreach (var transform in GetTargets(targets))
            {
                transform.SetRotation(Quaternion.Euler(rotation.GetValue(true)),space);
            }
        }

        public override void OnPreviewChanged<TTarget>(AnimoraPlayer animoraPlayer, bool isOn, IEnumerable<TTarget> targets, AnimoraClip clip)
        {
            foreach (var transform in GetTargets(targets))
            {
                AnimoraPreviewManager.RecordOrUndoObject(isOn,this,transform.GetRotation(space), (e)=> transform.SetRotation(e,space));
            }
        }
    }
}