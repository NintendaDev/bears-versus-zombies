using System.Collections.Generic;
using OM.Animora.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace OM.Animora.Modules
{
    [System.Serializable]
    [AnimoraCreate("Set Fill Amount", "UI/Set Fill Amount")]
    [AnimoraIcon("AnimationClip Icon")]
    [AnimoraKeywords("set", "Fill", "Amount", "action")]
    [AnimoraDescription("This is a set Fill Amount action")]
    [AnimoraAction(typeof(Image))]
    public class AnimoraActionSetFillAmount : AnimoraActionWithTargets<Image>
    {
        [SerializeField] private AnimoraValue<float> fillAmount;
        
        public override void OnTriggerAction<TTarget>(IEnumerable<TTarget> targets, AnimoraPlayer animoraPlayer, AnimoraClip clip)
        {
            foreach (var target in GetTargets(targets))
            {
                target.fillAmount = fillAmount.GetValue(true);
            }
        }
        
        public override void OnPreviewChanged<TTarget>(AnimoraPlayer animoraPlayer, bool isOn, IEnumerable<TTarget> targets, AnimoraClip clip)
        {
            foreach (var image in GetTargets(targets))
            {
                AnimoraPreviewManager.RecordOrUndoObject(isOn,this,image.fillAmount, (e)=> image.fillAmount = e);
            }
        }
    }
}