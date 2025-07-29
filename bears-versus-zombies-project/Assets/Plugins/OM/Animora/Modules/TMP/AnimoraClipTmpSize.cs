#if Animora_TMP

using OM.Animora.Runtime;
using TMPro;
using UnityEngine;

namespace OM.Animora.Modules.TMP
{
    [System.Serializable]
    [AnimoraCreate("TMP Font Size", "TMP/TMP Font Size")]
    [AnimoraIcon("AnimationClip Icon")]
    [AnimoraKeywords("set", "TMP", "action", "Font Size")]
    [AnimoraDescription("This is a set TMP Font Size action")]
    public class AnimoraClipTmpSize : AnimoraClipWithTarget<TMP_Text>
    {
        [SerializeField] private AnimoraInterpolation<float> size;
        
        public override void Enter()
        {
            base.Enter();
            size.Setup(targets.GetTargets(), (i) => targets.GetTargetAt(i).fontSize,
                (value1, value2) => value1 + value2, CurrentPlayDirection);
        }
        
        public override void OnEvaluate(float time, float clipTime, float normalizedTime, bool isPreviewing)
        {
            base.OnEvaluate(time, clipTime, normalizedTime, isPreviewing);
            for (var i = 0; i < targets.GetTargets().Count; i++)
            {
                var target = targets.GetTargetAt(i);
                if (target == null) continue;
                target.fontSize = size.Interpolate(normalizedTime,i,Mathf.LerpUnclamped);
            }
        }
        
        public override void OnPreviewChanged(AnimoraPlayer animoraPlayer, bool isOn)
        {
            base.OnPreviewChanged(animoraPlayer, isOn);
            for (var i = 0; i < targets.GetTargets().Count; i++)
            {
                var target = targets.GetTargetAt(i);
                if (target == null) continue;
                AnimoraPreviewManager.RecordOrUndoObject(isOn, this, target.fontSize,
                    (value) => target.fontSize = value);
            }
        }
        
        public override void OnCreate(AnimoraPlayer player)
        {
            base.OnCreate(player);
            ActionsManager.AddActionDirect(AnimoraAction.CreateInstance<AnimoraActionSetTmpSize>(), player);
        }
    }
}

#endif
