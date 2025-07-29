#if Animora_TMP
using System.Collections.Generic;
using OM.Animora.Runtime;
using TMPro;
using UnityEngine;

namespace OM.Animora.Modules.TMP
{
    [System.Serializable]
    [AnimoraCreate("Set Character Spacing", "TMP/Set Character Spacing")]
    [AnimoraIcon("AnimationClip Icon")]
    [AnimoraKeywords("set", "TMP", "action", "Character Spacing")]
    [AnimoraDescription("This is a set TMP Character Spacing action")]
    [AnimoraAction(typeof(TMPro.TMP_Text))]
    public class AnimoraActionSetTmpCharacterSpacing : AnimoraActionWithTargets<TMP_Text>
    {
        [SerializeField] private AnimoraValue<float> characterSpacing;

        public override void OnTriggerAction<TTarget>(IEnumerable<TTarget> targets, AnimoraPlayer animoraPlayer, AnimoraClip clip)
        {
            foreach (var target in GetTargets(targets))
            {
                target.characterSpacing = characterSpacing.GetValue(true);
            }
        }

        public override void OnPreviewChanged<TTarget>(AnimoraPlayer animoraPlayer, bool isOn, IEnumerable<TTarget> targets, AnimoraClip clip)
        {
            foreach (var tmpText in GetTargets(targets))
            {
                AnimoraPreviewManager.RecordOrUndoObject(isOn, this, tmpText.characterSpacing,
                    (e) => tmpText.characterSpacing = e);
            }
        }
    }
}
#endif