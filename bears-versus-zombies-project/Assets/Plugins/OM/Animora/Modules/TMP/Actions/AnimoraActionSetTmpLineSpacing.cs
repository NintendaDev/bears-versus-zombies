#if Animora_TMP
using System;
using System.Collections.Generic;
using OM.Animora.Runtime;
using TMPro;
using UnityEngine;

namespace OM.Animora.Modules.TMP
{
    [Serializable]
    [AnimoraCreate("Set Line Spacing", "TMP/Set Line Spacing")]
    [AnimoraIcon("AnimationClip Icon")]
    [AnimoraKeywords("set", "TMP", "action", "Line Spacing")]
    [AnimoraDescription("This is a set TMP Line Spacing action")]
    [AnimoraAction(typeof(TMP_Text))]
    public class AnimoraActionSetTmpLineSpacing : AnimoraActionWithTargets<TMP_Text>
    {
        [SerializeField] private AnimoraValue<float> lineSpacing;

        public override void OnTriggerAction<TTarget>(IEnumerable<TTarget> targets, AnimoraPlayer animoraPlayer, AnimoraClip clip)
        {
            foreach (var target in GetTargets(targets))
            {
                target.lineSpacing = lineSpacing.GetValue(true);
            }
        }

        public override void OnPreviewChanged<TTarget>(AnimoraPlayer animoraPlayer, bool isOn, IEnumerable<TTarget> targets, AnimoraClip clip)
        {
            foreach (var tmpText in GetTargets(targets))
            {
                AnimoraPreviewManager.RecordOrUndoObject(isOn, this, tmpText.lineSpacing,
                    (e) => tmpText.lineSpacing = e);
            }
        }
    }
}
#endif