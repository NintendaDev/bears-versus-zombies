#if Animora_TMP
using OM.Animora.Runtime;
using TMPro;
using UnityEngine;

namespace OM.Animora.Modules.TMP
{
    [System.Serializable]
    [AnimoraCreate("Font Color", "TMP/Font Color")]
    [AnimoraIcon("AnimationClip Icon")]
    [AnimoraKeywords("set", "TMP", "action", "Color")]
    [AnimoraDescription("This is a set TMP Font Color action")]
    public class AnimoraClipTmpFontColor : AnimoraClipWithTarget<TMP_Text>
    {
        [SerializeField] private AnimoraInterpolation<Color> fontColor;
    
        public override void Enter()
        {
            base.Enter();
            fontColor.Setup(targets.GetTargets(), (i) => targets.GetTargetAt(i).color,
                (value1, value2) => value1 * value2, CurrentPlayDirection);
        }
    
        public override void OnEvaluate(float time, float clipTime, float normalizedTime, bool isPreviewing)
        {
            base.OnEvaluate(time, clipTime, normalizedTime, isPreviewing);
            for (var i = 0; i < targets.GetTargets().Count; i++)
            {
                var target = targets.GetTargetAt(i);
                if (target == null) continue;
                target.color = fontColor.Interpolate(normalizedTime,i,Color.LerpUnclamped);
            }
        }
    
        public override void OnPreviewChanged(AnimoraPlayer animoraPlayer, bool isOn)
        {
            base.OnPreviewChanged(animoraPlayer, isOn);
            for (var i = 0; i < targets.GetTargets().Count; i++)
            {
                var target = targets.GetTargetAt(i);
                if (target == null) continue;
                AnimoraPreviewManager.RecordOrUndoObject(isOn, this, target.color,
                    (value) => target.color = value);
            }
        }
    
        public override void OnCreate(AnimoraPlayer player)
        {
            base.OnCreate(player);
            ActionsManager.AddActionDirect(AnimoraAction.CreateInstance<AnimoraActionSetTmpFontColor>(), player);
        }
    }
}

#endif