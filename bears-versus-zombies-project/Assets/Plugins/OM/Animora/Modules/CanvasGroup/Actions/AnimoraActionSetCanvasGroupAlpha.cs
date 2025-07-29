using System.Collections.Generic;
using OM.Animora.Runtime;
using UnityEngine;

namespace OM.Animora.Modules
{
    [System.Serializable]
    [AnimoraCreate("Set CanvasGroup Alpha", "CanvasGroup/Set CanvasGroup Alpha")]
    [AnimoraIcon("AnimationClip Icon")]
    [AnimoraKeywords("set", "CanvasGroup", "action","Alpha")]
    [AnimoraDescription("This is a set CanvasGroup Alpha action")]
    [AnimoraAction(typeof(CanvasGroup))]
    public class AnimoraActionSetCanvasGroupAlpha : AnimoraActionWithTargets<CanvasGroup>
    {
        [SerializeField] private AnimoraValue<float> alpha;

        public override void OnTriggerAction<TTarget>(IEnumerable<TTarget> targets, AnimoraPlayer animoraPlayer, AnimoraClip clip)
        {
            foreach (var target in GetTargets(targets))
            {
                target.alpha = alpha.GetValue(true);
            }
        }

        public override void OnPreviewChanged<TTarget>(AnimoraPlayer animoraPlayer, bool isOn, IEnumerable<TTarget> targets, AnimoraClip clip)
        {
            foreach (var canvasGroup in GetTargets(targets))
            {
                AnimoraPreviewManager.RecordOrUndoObject(isOn,this,canvasGroup.alpha, (e)=> canvasGroup.alpha = e);
            }
        }
    }
}