using System.Collections.Generic;
using OM.Animora.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace OM.Animora.Modules
{
    [System.Serializable]
    [AnimoraCreate("Set Color", "UI/Set Color")]
    [AnimoraIcon("AnimationClip Icon")]
    [AnimoraKeywords("set", "Color", "action")]
    [AnimoraDescription("This is a set Color action")]
    [AnimoraAction(typeof(Graphic))]
    public class AnimoraActionSetGraphicColor : AnimoraActionWithTargets<Graphic>
    {
        [SerializeField] private AnimoraValue<Color> color;
        
        public override void OnTriggerAction<TTarget>(IEnumerable<TTarget> targets, AnimoraPlayer animoraPlayer, AnimoraClip clip)
        {
            foreach (var target in GetTargets(targets))
            {
                target.color = color.GetValue(true);
            }
        }
        
        public override void OnPreviewChanged<TTarget>(AnimoraPlayer animoraPlayer, bool isOn, IEnumerable<TTarget> targets, AnimoraClip clip)
        {
            foreach (var graphic in GetTargets(targets))
            {
                AnimoraPreviewManager.RecordOrUndoObject(isOn,this,graphic.color, (e)=> graphic.color = e);
            }
        }
    }
}