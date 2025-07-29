using OM.Animora.Runtime;
using UnityEngine;

namespace OM.Animora.Modules
{
    [System.Serializable]
    [AnimoraCreate("Anchor", "UI/Anchor")]
    [AnimoraIcon("d_RectTransform Icon")]
    [AnimoraKeywords("Anchor")]
    [AnimoraDescription("This is a clip anchor action")]
    public class AnimoraClipAnchor : AnimoraClipWithTarget<RectTransform>
    {
        [SerializeField,AnimoraInterpolation(useOptional:true)] private AnimoraInterpolation<Vector2> anchorMin = new AnimoraInterpolation<Vector2>(false);
        [SerializeField,AnimoraInterpolation(useOptional:true)] private AnimoraInterpolation<Vector2> anchorMax = new AnimoraInterpolation<Vector2>(false);

        public override void Enter()
        {
            base.Enter();
            if (anchorMin.Enabled)
            {
                anchorMin.Setup(targets.GetTargets(), (i) => targets.GetTargetAt(i).anchorMin,
                    (value1, value2) => value1 + value2, CurrentPlayDirection);
            }

            if (anchorMax.Enabled)
            {
                anchorMax.Setup(targets.GetTargets(), (i) => targets.GetTargetAt(i).anchorMax,
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
                if (anchorMin.Enabled)
                    target.anchorMin = anchorMin.Interpolate(normalizedTime, i, Vector2.LerpUnclamped);
                if (anchorMax.Enabled)
                    target.anchorMax = anchorMax.Interpolate(normalizedTime, i, Vector2.LerpUnclamped);
            }
        }
        
        public override void OnPreviewChanged(AnimoraPlayer animoraPlayer, bool isOn)
        {
            base.OnPreviewChanged(animoraPlayer, isOn);
            for (var i = 0; i < targets.GetTargets().Count; i++)
            {
                var target = targets.GetTargetAt(i);
                if (target == null) continue;
                if (anchorMin.Enabled)
                {
                    AnimoraPreviewManager.RecordOrUndoObject(isOn, this, target.anchorMin,
                        (value) => target.anchorMin = value);
                }

                if (anchorMax.Enabled)
                {
                    AnimoraPreviewManager.RecordOrUndoObject(isOn, this, target.anchorMax,
                        (value) => target.anchorMax = value);
                }
            }
        }
        
        public override void OnCreate(AnimoraPlayer player)
        {
            base.OnCreate(player);
            ActionsManager.AddActionDirect(AnimoraAction.CreateInstance<AnimoraActionSetAnchorMin>(), player);
            ActionsManager.AddActionDirect(AnimoraAction.CreateInstance<AnimoraActionSetAnchorMax>(), player);
        }
    }
}