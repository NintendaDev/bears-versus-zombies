using System.Collections.Generic;
using OM.Animora.Runtime;
using UnityEngine;

namespace OM.Animora.Modules
{
    [System.Serializable]
    [AnimoraCreate("Set Position", "Transform/Set Position")]
    [AnimoraIcon("AnimationClip Icon")]
    [AnimoraKeywords("Set", "Position", "action")]
    [AnimoraDescription("This is a set Position action")]
    [AnimoraAction(typeof(Transform))]
    public class AnimoraActionSetPosition : AnimoraActionWithTargets<Transform>
    {
        [SerializeField] private Space space = Space.World;
        [SerializeField] private AnimoraValue<Vector3> position;
        
        public override void OnTriggerAction<TTarget>(IEnumerable<TTarget> targets, AnimoraPlayer animoraPlayer, AnimoraClip clip)
        {
            foreach (var transform in GetTargets(targets))
            {
                transform.SetPosition(position.GetValue(true),space);
            }
        }

        public override void OnPreviewChanged<TTarget>(AnimoraPlayer animoraPlayer, bool isOn, IEnumerable<TTarget> targets, AnimoraClip clip)
        {
            foreach (var transform in GetTargets(targets))
            {
                AnimoraPreviewManager.RecordOrUndoObject(isOn,this,transform.GetPosition(space), (e)=> transform.SetPosition(e,space));
            }
        }
    }
}