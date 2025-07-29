using OM.Animora.Runtime;
using UnityEngine;

namespace OM.Animora.Modules
{
    [System.Serializable]
    [AnimoraCreate("Set Size Delta", "UI/Set Size Delta")]
    [AnimoraIcon("d_RectTransform Icon")]
    [AnimoraKeywords("set", "Size", "Delta", "action")]
    [AnimoraDescription("This is a set Size Delta action")]
    public class AnimoraClipSizeDelta : AnimoraClipWithTarget<RectTransform>
    {
        [SerializeField] private AnimoraInterpolation<Vector2> sizeDelta;
        
        public override void Enter()
        {
            base.Enter();
            sizeDelta.Setup(targets.GetTargets(), (i) => targets.GetTargetAt(i).sizeDelta,
                (value1, value2) => value1 + value2, CurrentPlayDirection);
        }
        
        public override void OnEvaluate(float time, float clipTime, float normalizedTime, bool isPreviewing)
        {
            base.OnEvaluate(time, clipTime, normalizedTime, isPreviewing);
            for (var i = 0; i < targets.GetTargets().Count; i++)
            {
                var target = targets.GetTargetAt(i);
                if (target == null) continue;
                target.sizeDelta = sizeDelta.Interpolate(normalizedTime,i,Vector2.LerpUnclamped);
            }
        }
        
        public override void OnPreviewChanged(AnimoraPlayer animoraPlayer, bool isOn)
        {
            base.OnPreviewChanged(animoraPlayer, isOn);
            for (var i = 0; i < targets.GetTargets().Count; i++)
            {
                var target = targets.GetTargetAt(i);
                if (target == null) continue;
                AnimoraPreviewManager.RecordOrUndoObject(isOn, this, target.sizeDelta,
                    (value) => target.sizeDelta = value);
            }
        }

        public override void OnCreate(AnimoraPlayer player)
        {
            base.OnCreate(player);
            ActionsManager.AddActionDirect(AnimoraAction.CreateInstance<AnimoraActionSetSizeDelta>(), player);
        }
    }
}