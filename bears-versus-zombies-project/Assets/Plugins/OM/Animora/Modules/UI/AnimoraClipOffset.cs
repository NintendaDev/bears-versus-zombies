using OM.Animora.Runtime;
using UnityEngine;

namespace OM.Animora.Modules
{
    [System.Serializable]
    [AnimoraCreate("Offset", "UI/Offset")]
    [AnimoraIcon("d_RectTransform Icon")]
    [AnimoraKeywords("Offset")]
    [AnimoraDescription("This is a clip offset action")]
    public class AnimoraClipOffset : AnimoraClipWithTarget<RectTransform>
    {
        [SerializeField,AnimoraInterpolation(useOptional:true)] private AnimoraInterpolation<Vector2> offsetMin = new AnimoraInterpolation<Vector2>(false);
        [SerializeField,AnimoraInterpolation(useOptional:true)] private AnimoraInterpolation<Vector2> offsetMax = new AnimoraInterpolation<Vector2>(false);
        
        public override void Enter()
        {
            base.Enter();
            if (offsetMin.Enabled)
            {
                offsetMin.Setup(targets.GetTargets(), (i) => targets.GetTargetAt(i).offsetMin,
                    (value1, value2) => value1 + value2, CurrentPlayDirection);
            }
            if (offsetMax.Enabled)
            {
                offsetMax.Setup(targets.GetTargets(), (i) => targets.GetTargetAt(i).offsetMax,
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
                if(offsetMin.Enabled) target.offsetMin = offsetMin.Interpolate(normalizedTime,i,Vector2.LerpUnclamped);
                if(offsetMax.Enabled) target.offsetMax = offsetMax.Interpolate(normalizedTime,i,Vector2.LerpUnclamped);
            }
        }
        
        public override void OnPreviewChanged(AnimoraPlayer animoraPlayer, bool isOn)
        {
            base.OnPreviewChanged(animoraPlayer, isOn);
            for (var i = 0; i < targets.GetTargets().Count; i++)
            {
                var target = targets.GetTargetAt(i);
                if (target == null) continue;
                if (offsetMin.Enabled)
                {
                    AnimoraPreviewManager.RecordOrUndoObject(isOn, this, target.offsetMin,
                        (value) => target.offsetMin = value);
                }
                
                if (offsetMax.Enabled)
                {
                    AnimoraPreviewManager.RecordOrUndoObject(isOn, this, target.offsetMax,
                        (value) => target.offsetMax = value);
                }
            }
        }
        
        public override void OnCreate(AnimoraPlayer player)
        {
            base.OnCreate(player);
            ActionsManager.AddActionDirect(AnimoraAction.CreateInstance<AnimoraActionSetOffsetMin>(), player);
            ActionsManager.AddActionDirect(AnimoraAction.CreateInstance<AnimoraActionSetOffsetMax>(), player);
        }
    }
}