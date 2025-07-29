using OM.Animora.Runtime;
using UnityEngine;

namespace OM.Animora.Modules
{
    [System.Serializable]
    [AnimoraCreate("Canvas Alpha", "CanvasGroup/Canvas Alpha")]
    [AnimoraIcon("d_CanvasGroup Icon")]
    [AnimoraKeywords("set", "CanvasGroup", "action", "Alpha")]
    [AnimoraDescription("This is a set CanvasGroup Alpha action")]
    public class AnimoraClipCanvasGroupAlpha : AnimoraClipWithTarget<CanvasGroup>
    {
        [SerializeField] private AnimoraInterpolation<float> alpha;
        
        public override void Enter()
        {
            base.Enter();
            alpha.Setup(targets.GetTargets(), (i) => targets.GetTargetAt(i).alpha,
                (value1, value2) => value1 + value2, CurrentPlayDirection);
        }
        
        public override void OnEvaluate(float time, float clipTime, float normalizedTime, bool isPreviewing)
        {
            base.OnEvaluate(time, clipTime, normalizedTime, isPreviewing);
            for (var i = 0; i < targets.GetTargets().Count; i++)
            {
                var target = targets.GetTargetAt(i);
                if (target == null) continue;
                target.alpha = alpha.Interpolate(normalizedTime,i,Mathf.LerpUnclamped);
            }
        }
        
        public override void OnPreviewChanged(AnimoraPlayer animoraPlayer, bool isOn)
        {
            base.OnPreviewChanged(animoraPlayer, isOn);
            for (var i = 0; i < targets.GetTargets().Count; i++)
            {
                var target = targets.GetTargetAt(i);
                if (target == null) continue;
                AnimoraPreviewManager.RecordOrUndoObject(isOn, this, target.alpha,
                    (value) => target.alpha = value);
            }
        }
        
        public override void OnCreate(AnimoraPlayer player)
        {
            base.OnCreate(player);
            ActionsManager.AddActionDirect(AnimoraAction.CreateInstance<AnimoraActionSetCanvasGroupAlpha>(), player);
        }
    }
}