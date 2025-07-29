#if Animora_TMP
using System;
using System.Collections.Generic;
using OM.Animora.Runtime;
using TMPro;
using UnityEngine;

namespace OM.Animora.Modules.TMP
{
    [Serializable]
    [AnimoraCreate("Set Word Spacing", "TMP/Set Word Spacing")]
    [AnimoraIcon("AnimationClip Icon")]
    [AnimoraKeywords("set", "TMP", "action", "Word Spacing")]
    [AnimoraDescription("This is a set TMP Word Spacing action")]
    [AnimoraAction(typeof(TMP_Text))]
    public class AnimoraActionSetTmpWordSpacing : AnimoraActionWithTargets<TMP_Text>
    {
        [SerializeField] private AnimoraValue<float> wordSpacing;

        public override void OnTriggerAction<TTarget>(IEnumerable<TTarget> targets, AnimoraPlayer animoraPlayer, AnimoraClip clip)
        {
            foreach (var target in GetTargets(targets))
            {
                target.wordSpacing = wordSpacing.GetValue(true);
            }
        }

        public override void OnPreviewChanged<TTarget>(AnimoraPlayer animoraPlayer, bool isOn, IEnumerable<TTarget> targets, AnimoraClip clip)
        {
            foreach (var tmpText in GetTargets(targets))
            {
                AnimoraPreviewManager.RecordOrUndoObject(isOn, this, tmpText.wordSpacing,
                    (e) => tmpText.wordSpacing = e);
            }
        }
    }
}
#endif