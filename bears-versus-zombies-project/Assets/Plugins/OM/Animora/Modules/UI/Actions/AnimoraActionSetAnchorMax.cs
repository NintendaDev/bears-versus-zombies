using System.Collections.Generic;
using OM.Animora.Runtime;
using UnityEngine;

namespace OM.Animora.Modules
{
    [System.Serializable]
    [AnimoraCreate("Set Anchor Max", "UI/Set Anchor Max")]
    [AnimoraIcon("AnimationClip Icon")]
    [AnimoraKeywords("set", "Anchor", "Max", "action")]
    [AnimoraDescription("This is a set Anchor Max action")]
    [AnimoraAction(typeof(RectTransform))]
    public class AnimoraActionSetAnchorMax : AnimoraActionWithTargets<RectTransform>
    {
        [SerializeField] private AnimoraValue<Vector2> anchorMax;
        
        public override void OnTriggerAction<TTarget>(IEnumerable<TTarget> targets, AnimoraPlayer animoraPlayer, AnimoraClip clip)
        {
            foreach (var target in GetTargets(targets))
            {
                target.anchorMax = anchorMax.GetValue(true);
            }
        }
        
        public override void OnPreviewChanged<TTarget>(AnimoraPlayer animoraPlayer, bool isOn, IEnumerable<TTarget> targets, AnimoraClip clip)
        {
            foreach (var rectTransform in GetTargets(targets))
            {
                AnimoraPreviewManager.RecordOrUndoObject(isOn,this,rectTransform.anchorMax, (e)=> rectTransform.anchorMax = e);
            }
        }
    }
}