#if Animora_TMP
using System;
using System.Collections.Generic;
using OM.Animora.Runtime;
using TMPro;
using UnityEngine;

namespace OM.Animora.Modules.TMP
{
    [Serializable]
    [AnimoraCreate("Set Font Color", "TMP/Set Font Color")]
    [AnimoraIcon("AnimationClip Icon")]
    [AnimoraKeywords("set", "TMP", "action", "Color")]
    [AnimoraDescription("This is a set TMP Font Color action")]
    [AnimoraAction(typeof(TMP_Text))]
    public class AnimoraActionSetTmpFontColor : AnimoraActionWithTargets<TMP_Text>
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
            foreach (var tmpText in GetTargets(targets))
            {
                AnimoraPreviewManager.RecordOrUndoObject(isOn, this, tmpText.color, (e) => tmpText.color = e);
            }
        }
    }
}
#endif