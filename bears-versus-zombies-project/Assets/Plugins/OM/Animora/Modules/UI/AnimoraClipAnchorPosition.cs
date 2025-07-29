using OM.Animora.Runtime;
using UnityEngine;

namespace OM.Animora.Modules
{
    [System.Serializable]
    [AnimoraCreate("Anchor Position", "UI/Anchor Position")]
    [AnimoraDescription("Animates the anchor position of a RectTransform.")]
    [AnimoraIcon("d_RectTransform Icon")]
    [AnimoraKeywords("Anchor", "Position", "RectTransform", "UI")]
    public class AnimoraClipAnchorPosition : AnimoraClipWithTarget<RectTransform>
    {
        [SerializeField] private AnimoraInterpolation<Vector3> anchorPosition;

        public override void Enter()
        {
            base.Enter();
            anchorPosition.Setup(targets.GetTargets(), (i) => targets.GetTargetAt(i).anchoredPosition3D,
                (value1, value2) => value1 + value2,CurrentPlayDirection);
        }

        public override void OnEvaluate(float time, float clipTime, float normalizedTime, bool isPreviewing)
        {
            base.OnEvaluate(time, clipTime, normalizedTime, isPreviewing);
            for (var i = 0; i < targets.GetTargets().Count; i++)
            {
                var target = targets.GetTargetAt(i);
                if (target == null) continue;
                target.anchoredPosition3D = anchorPosition.Interpolate(normalizedTime,i,Vector3.LerpUnclamped);
            }
        }

        public override void OnPreviewChanged(AnimoraPlayer animoraPlayer, bool isOn)
        {
            base.OnPreviewChanged(animoraPlayer, isOn);
            for (var i = 0; i < targets.GetTargets().Count; i++)
            {
                var target = targets.GetTargetAt(i);
                if (target == null) continue;
                AnimoraPreviewManager.RecordOrUndoObject(isOn, this, target.anchoredPosition3D,
                    (value) => target.anchoredPosition3D = value);
            }
        }

        public override void OnCreate(AnimoraPlayer player)
        {
            base.OnCreate(player);
            ActionsManager.AddActionDirect(AnimoraAction.CreateInstance<AnimoraActionSetAnchorPosition>(), player);
        }
    }
}