using System.Collections.Generic;
using OM.Animora.Runtime;
using UnityEngine;

namespace OM.Animora.Modules
{
    [System.Serializable]
    [AnimoraCreate("Set Offset Min", "UI/Set Offset Min")]
    [AnimoraIcon("AnimationClip Icon")]
    [AnimoraKeywords("set", "Offset", "action","Min")]
    [AnimoraDescription("This is a set Offset action")]
    [AnimoraAction(typeof(RectTransform))]
    public class AnimoraActionSetOffsetMin : AnimoraActionWithTargets<RectTransform>
    {
        [SerializeField] private AnimoraValue<Vector2> offsetMin;
        
        public override void OnTriggerAction<TTarget>(IEnumerable<TTarget> targets, AnimoraPlayer animoraPlayer, AnimoraClip clip)
        {
            foreach (var target in GetTargets(targets))
            {
                target.offsetMin = offsetMin.GetValue(true);
            }
        }
        
        public override void OnPreviewChanged<TTarget>(AnimoraPlayer animoraPlayer, bool isOn, IEnumerable<TTarget> targets, AnimoraClip clip)
        {
            foreach (var rectTransform in GetTargets(targets))
            {
                AnimoraPreviewManager.RecordOrUndoObject(isOn,this,rectTransform.offsetMin, (e)=> rectTransform.offsetMin = e);
            }
        }
    }
}