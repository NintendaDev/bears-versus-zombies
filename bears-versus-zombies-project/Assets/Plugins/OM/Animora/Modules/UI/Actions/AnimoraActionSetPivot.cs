using System.Collections.Generic;
using OM.Animora.Runtime;
using UnityEngine;

namespace OM.Animora.Modules
{
    [System.Serializable]
    [AnimoraCreate("Set Pivot", "UI/Set Pivot")]
    [AnimoraIcon("AnimationClip Icon")]
    [AnimoraKeywords("set", "Pivot", "action")]
    [AnimoraDescription("This is a set Pivot action")]
    [AnimoraAction(typeof(RectTransform))]
    public class AnimoraActionSetPivot : AnimoraActionWithTargets<RectTransform>
    {
        [SerializeField] private AnimoraValue<Vector2> pivot;
        
        public override void OnTriggerAction<TTarget>(IEnumerable<TTarget> targets, AnimoraPlayer animoraPlayer, AnimoraClip clip)
        {
            foreach (var target in GetTargets(targets))
            {
                target.pivot = pivot.GetValue(true);
            }
        }
        
        public override void OnPreviewChanged<TTarget>(AnimoraPlayer animoraPlayer, bool isOn, IEnumerable<TTarget> targets, AnimoraClip clip)
        {
            foreach (var rectTransform in GetTargets(targets))
            {
                AnimoraPreviewManager.RecordOrUndoObject(isOn,this,rectTransform.pivot, (e)=> rectTransform.pivot = e);
            }
        }
    }
}