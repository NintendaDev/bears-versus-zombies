#if Animora_TMP

using System.Collections.Generic;
using OM.Animora.Runtime;
using TMPro;
using UnityEngine;

namespace OM.Animora.Modules.TMP
{
    [System.Serializable]
    [AnimoraCreate("Set Font Size", "TMP/Set Font Size")]
    [AnimoraIcon("AnimationClip Icon")]
    [AnimoraKeywords("set", "TMP", "action", "Size")]
    [AnimoraDescription("This is a set TMP Size action")]
    [AnimoraAction(typeof(TMP_Text))]
    public class AnimoraActionSetTmpSize : AnimoraActionWithTargets<TMP_Text>
    {
        [SerializeField] private AnimoraValue<float> size;

        public override void OnTriggerAction<TTarget>(IEnumerable<TTarget> targets, AnimoraPlayer animoraPlayer, AnimoraClip clip)
        {
            foreach (var target in GetTargets(targets))
            {
                target.fontSize = size.GetValue(true);
            }
        }

        public override void OnPreviewChanged<TTarget>(AnimoraPlayer animoraPlayer, bool isOn, IEnumerable<TTarget> targets, AnimoraClip clip)
        {
            foreach (var tmpText in GetTargets(targets))
            {
                AnimoraPreviewManager.RecordOrUndoObject(isOn, this, tmpText.fontSize, (e) => tmpText.fontSize = e);
            }
        }
    }
}

#endif
