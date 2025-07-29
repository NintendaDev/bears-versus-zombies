using System.Collections.Generic;
using OM.Animora.Runtime;
using UnityEngine;

namespace OM.Animora.Modules
{
    [System.Serializable]
    [AnimoraCreate("Set Offset Max", "UI/Set Offset Max")]
    [AnimoraIcon("AnimationClip Icon")]
    [AnimoraKeywords("set", "Offset", "action","Max")]
    [AnimoraDescription("This is a set Offset action")]
    [AnimoraAction(typeof(RectTransform))]
    public class AnimoraActionSetOffsetMax : AnimoraActionWithTargets<RectTransform>
    {
        [SerializeField] private AnimoraValue<Vector2> offsetMax;
        
        public override void OnTriggerAction<TTarget>(IEnumerable<TTarget> targets, AnimoraPlayer animoraPlayer, AnimoraClip clip)
        {
            foreach (var target in GetTargets(targets))
            {
                target.offsetMax = offsetMax.GetValue(true);
            }
        }
        
        public override void OnPreviewChanged<TTarget>(AnimoraPlayer animoraPlayer, bool isOn, IEnumerable<TTarget> targets, AnimoraClip clip)
        {
            foreach (var rectTransform in GetTargets(targets))
            {
                AnimoraPreviewManager.RecordOrUndoObject(isOn,this,rectTransform.offsetMax, (e)=> rectTransform.offsetMax = e);
            }
        }
    }
}