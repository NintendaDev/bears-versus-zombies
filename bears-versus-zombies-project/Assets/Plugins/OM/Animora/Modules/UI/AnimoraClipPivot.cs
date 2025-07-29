using OM.Animora.Runtime;
using UnityEngine;

namespace OM.Animora.Modules
{
    [System.Serializable]
    [AnimoraCreate("Pivot", "UI/Pivot")]
    [AnimoraIcon("d_RectTransform Icon")]
    [AnimoraKeywords("set", "Pivot")]
    [AnimoraDescription("This is a set Pivot action")]
    public class AnimoraClipPivot : AnimoraClipWithTarget<RectTransform>
    {
        [SerializeField] private AnimoraInterpolation<Vector2> pivot;

        public override void Enter()
        {
            base.Enter();
            pivot.Setup(targets.GetTargets(), (i) => targets.GetTargetAt(i).pivot,
                (value1, value2) => value1 + value2, CurrentPlayDirection);
        }
        
        public override void OnEvaluate(float time, float clipTime, float normalizedTime, bool isPreviewing)
        {
            base.OnEvaluate(time, clipTime, normalizedTime, isPreviewing);
            for (var i = 0; i < targets.GetTargets().Count; i++)
            {
                var target = targets.GetTargetAt(i);
                if (target == null) continue;
                target.pivot = pivot.Interpolate(normalizedTime,i,Vector2.LerpUnclamped);
            }
        }
        
        public override void OnPreviewChanged(AnimoraPlayer animoraPlayer, bool isOn)
        {
            base.OnPreviewChanged(animoraPlayer, isOn);
            for (var i = 0; i < targets.GetTargets().Count; i++)
            {
                var target = targets.GetTargetAt(i);
                if (target == null) continue;
                AnimoraPreviewManager.RecordOrUndoObject(isOn, this, target.pivot,
                    (value) => target.pivot = value);
            }
        }
        
        public override void OnCreate(AnimoraPlayer player)
        {
            base.OnCreate(player);
            ActionsManager.AddActionDirect(AnimoraAction.CreateInstance<AnimoraActionSetPivot>(), player);
        }
        
    }
}