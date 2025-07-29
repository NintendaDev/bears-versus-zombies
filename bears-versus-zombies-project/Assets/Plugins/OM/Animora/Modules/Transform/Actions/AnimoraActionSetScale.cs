using System.Collections.Generic;
using OM.Animora.Runtime;
using UnityEngine;

namespace OM.Animora.Modules
{
    [System.Serializable]
    [AnimoraCreate("Set Scale", "Transform/Set Scale")]
    [AnimoraIcon("AnimationClip Icon")]
    [AnimoraKeywords("Set", "Scale", "action")]
    [AnimoraDescription("This is a set Scale action")]
    [AnimoraAction(typeof(Transform))]
    public class AnimoraActionSetScale : AnimoraActionWithTargets<Transform>
    {
        [SerializeField] private AnimoraValue<Vector3> scale;

        public override void OnTriggerAction<TTarget>(IEnumerable<TTarget> targets, AnimoraPlayer animoraPlayer, AnimoraClip clip)
        {
            foreach (var target in GetTargets(targets))
            {
                target.localScale = scale.GetValue(true);
            }
        }

        public override void OnPreviewChanged<TTarget>(AnimoraPlayer animoraPlayer, bool isOn, IEnumerable<TTarget> targets, AnimoraClip clip)
        {
            foreach (var transform in GetTargets(targets))
            {
                AnimoraPreviewManager.RecordOrUndoObject(isOn,this,transform.localScale, (e)=> transform.localScale = e);
            }
        }
    }
}