#if Animora_TMP
using OM.Animora.Runtime;
using TMPro;
using UnityEngine;

namespace OM.Animora.Modules.TMP
{
    [System.Serializable]
    [AnimoraCreate("TMP Spacing", "TMP/TMP Spacing")]
    [AnimoraIcon("AnimationClip Icon")]
    [AnimoraKeywords("TMP", "spacing", "clip")]
    [AnimoraDescription("This is a TMP Spacing clip")]
    public class AnimoraClipTmpSpacing : AnimoraClipWithTarget<TMP_Text>
    {
        [SerializeField,AnimoraInterpolation(useOptional:true)] private AnimoraInterpolation<float> characterSpacing = new AnimoraInterpolation<float>(false);
        [SerializeField,AnimoraInterpolation(useOptional:true)] private AnimoraInterpolation<float> lineSpacing = new AnimoraInterpolation<float>(false);
        [SerializeField,AnimoraInterpolation(useOptional:true)] private AnimoraInterpolation<float> wordSpacing = new AnimoraInterpolation<float>(false);
        
        public override void Enter()
        {
            base.Enter();
            if (characterSpacing.Enabled)
            {
                characterSpacing.Setup(targets.GetTargets(), (i) => targets.GetTargetAt(i).characterSpacing,
                    (value1, value2) => value1 + value2, CurrentPlayDirection);
            }

            if (lineSpacing.Enabled)
            {
                lineSpacing.Setup(targets.GetTargets(), (i) => targets.GetTargetAt(i).lineSpacing,
                    (value1, value2) => value1 + value2, CurrentPlayDirection);
            }

            if (wordSpacing.Enabled)
            {
                wordSpacing.Setup(targets.GetTargets(), (i) => targets.GetTargetAt(i).wordSpacing,
                    (value1, value2) => value1 + value2, CurrentPlayDirection);
            }
        }
        
        public override void OnEvaluate(float time, float clipTime, float normalizedTime, bool isPreviewing)
        {
            base.OnEvaluate(time, clipTime, normalizedTime, isPreviewing);
            for (var i = 0; i < targets.GetTargets().Count; i++)
            {
                var target = targets.GetTargetAt(i);
                if (target == null) continue;
                if (characterSpacing.Enabled)
                    target.characterSpacing = characterSpacing.Interpolate(normalizedTime, i, Mathf.LerpUnclamped);
                if (lineSpacing.Enabled)
                    target.lineSpacing = lineSpacing.Interpolate(normalizedTime, i, Mathf.LerpUnclamped);
                if (wordSpacing.Enabled)
                    target.wordSpacing = wordSpacing.Interpolate(normalizedTime, i, Mathf.LerpUnclamped);
            }
        }
        
        public override void OnPreviewChanged(AnimoraPlayer animoraPlayer, bool isOn)
        {
            base.OnPreviewChanged(animoraPlayer, isOn);
            for (var i = 0; i < targets.GetTargets().Count; i++)
            {
                var target = targets.GetTargetAt(i);
                if (target == null) continue;
                if (characterSpacing.Enabled)
                {
                    AnimoraPreviewManager.RecordOrUndoObject(isOn, this, target.characterSpacing,
                        (value) => target.characterSpacing = value);
                }

                if (lineSpacing.Enabled)
                {
                    AnimoraPreviewManager.RecordOrUndoObject(isOn, this, target.lineSpacing,
                        (value) => target.lineSpacing = value);
                }

                if (wordSpacing.Enabled)
                {
                    AnimoraPreviewManager.RecordOrUndoObject(isOn, this, target.wordSpacing,
                        (value) => target.wordSpacing = value);
                }
            }
        }
        
        public override void OnCreate(AnimoraPlayer player)
        {
            base.OnCreate(player);
                ActionsManager.AddActionDirect(AnimoraAction.CreateInstance<AnimoraActionSetTmpCharacterSpacing>(), player);
                ActionsManager.AddActionDirect(AnimoraAction.CreateInstance<AnimoraActionSetTmpLineSpacing>(), player);
                ActionsManager.AddActionDirect(AnimoraAction.CreateInstance<AnimoraActionSetTmpWordSpacing>(), player);
        }
        
    }
}
#endif