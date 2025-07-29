using System.Collections.Generic;
using OM.Animora.Runtime;
using UnityEngine;

namespace OM.Animora.Modules
{
    [System.Serializable]
    [AnimoraCreate("Set Size Delta", "UI/Set Size Delta")]
    [AnimoraIcon("AnimationClip Icon")]
    [AnimoraKeywords("set", "Size", "Delta", "action")]
    [AnimoraDescription("This is a set Size Delta action")]
    [AnimoraAction(typeof(RectTransform))]
    public class AnimoraActionSetSizeDelta : AnimoraActionWithTargets<RectTransform>
    {
        [SerializeField] private AnimoraValue<Vector2> sizeDelta;
        
        public override void OnTriggerAction<TTarget>(IEnumerable<TTarget> targets, AnimoraPlayer animoraPlayer, AnimoraClip clip)
        {
            foreach (var target in GetTargets(targets))
            {
                target.sizeDelta = sizeDelta.GetValue(true);
            }
        }
        
        public override void OnPreviewChanged<TTarget>(AnimoraPlayer animoraPlayer, bool isOn, IEnumerable<TTarget> targets, AnimoraClip clip)
        {
            foreach (var rectTransform in GetTargets(targets))
            {
                AnimoraPreviewManager.RecordOrUndoObject(isOn,this,rectTransform.sizeDelta, (e)=> rectTransform.sizeDelta = e);
            }
        }
    }
}