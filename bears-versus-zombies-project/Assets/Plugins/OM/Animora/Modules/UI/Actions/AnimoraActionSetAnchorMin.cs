using System.Collections.Generic;
using OM.Animora.Runtime;
using UnityEngine;

namespace OM.Animora.Modules
{
    [System.Serializable]
    [AnimoraCreate("Set Anchor Min", "UI/Set Anchor Min")]
    [AnimoraIcon("AnimationClip Icon")]
    [AnimoraKeywords("set", "Anchor", "Min", "action")]
    [AnimoraDescription("This is a set Anchor Min action")]
    [AnimoraAction(typeof(RectTransform))]
    public class AnimoraActionSetAnchorMin : AnimoraActionWithTargets<RectTransform>
    {
        [SerializeField] private AnimoraValue<Vector2> anchorMin;
        
        public override void OnTriggerAction<TTarget>(IEnumerable<TTarget> targets, AnimoraPlayer animoraPlayer, AnimoraClip clip)
        {
            foreach (var target in GetTargets(targets))
            {
                target.anchorMin = anchorMin.GetValue(true);
            }
        }
        
        public override void OnPreviewChanged<TTarget>(AnimoraPlayer animoraPlayer, bool isOn, IEnumerable<TTarget> targets, AnimoraClip clip)
        {
            foreach (var rectTransform in GetTargets(targets))
            {
                AnimoraPreviewManager.RecordOrUndoObject(isOn,this,rectTransform.anchorMin, (e)=> rectTransform.anchorMin = e);
            }
        }
    }
}