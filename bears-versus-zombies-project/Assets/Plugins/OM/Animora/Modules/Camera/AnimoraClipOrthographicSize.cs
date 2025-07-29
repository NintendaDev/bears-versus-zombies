using OM.Animora.Runtime;
using UnityEngine;

namespace OM.Animora.Modules
{
    [System.Serializable]
    [AnimoraCreate("Orthographic Size", "Camera/Orthographic Size")]
    [AnimoraIcon("d_Camera Icon")]
    [AnimoraKeywords("set", "Camera", "action", "Orthographic Size")]
    [AnimoraDescription("This is a set Camera Orthographic Size action")]
    public class AnimoraClipOrthographicSize : AnimoraClipWithTarget<Camera>
    {
        [SerializeField] private AnimoraInterpolation<float> orthographicSize;

        public override void Enter()
        {
            base.Enter();
            orthographicSize.Setup(targets.GetTargets(), (i) => targets.GetTargetAt(i).orthographicSize,
                (value1, value2) => value1 + value2, CurrentPlayDirection);
        }

        public override void OnEvaluate(float time, float clipTime, float normalizedTime, bool isPreviewing)
        {
            base.OnEvaluate(time, clipTime, normalizedTime, isPreviewing);
            for (var i = 0; i < targets.GetTargets().Count; i++)
            {
                var target = targets.GetTargetAt(i);
                if (target == null) continue;
                target.orthographicSize = orthographicSize.Interpolate(normalizedTime,i,Mathf.LerpUnclamped);
            }
        }

        public override void OnPreviewChanged(AnimoraPlayer animoraPlayer, bool isOn)
        {
            base.OnPreviewChanged(animoraPlayer, isOn);
            for (var i = 0; i < targets.GetTargets().Count; i++)
            {
                var target = targets.GetTargetAt(i);
                if (target == null) continue;
                AnimoraPreviewManager.RecordOrUndoObject(isOn, this, target.orthographicSize,
                    (value) => target.orthographicSize = value);
            }
        }
        
        public override void OnCreate(AnimoraPlayer player)
        {
            base.OnCreate(player);
            ActionsManager.AddActionDirect(AnimoraAction.CreateInstance<AnimoraActionSetCameraOrthographicSize>(), player);
        }
    }
}