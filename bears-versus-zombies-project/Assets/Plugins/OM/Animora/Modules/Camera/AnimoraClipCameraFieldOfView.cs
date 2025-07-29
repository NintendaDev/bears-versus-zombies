using OM.Animora.Runtime;
using UnityEngine;

namespace OM.Animora.Modules
{
    [System.Serializable]
    [AnimoraCreate("Camera FOV", "Camera/Camera FOV")]
    [AnimoraIcon("d_Camera Icon")]
    [AnimoraKeywords("set", "Camera", "action", "FOV","field","view","of")]
    [AnimoraDescription("This is a set Camera FOV action")]
    public class AnimoraClipCameraFieldOfView : AnimoraClipWithTarget<Camera>
    {
        [SerializeField] private AnimoraInterpolation<float> fieldOfView;

        public override void Enter()
        {
            base.Enter();
            fieldOfView.Setup(targets.GetTargets(), (i) => targets.GetTargetAt(i).fieldOfView,
                (value1, value2) => value1 + value2, CurrentPlayDirection);
        }

        public override void OnEvaluate(float time, float clipTime, float normalizedTime, bool isPreviewing)
        {
            base.OnEvaluate(time, clipTime, normalizedTime, isPreviewing);
            for (var i = 0; i < targets.GetTargets().Count; i++)
            {
                var target = targets.GetTargetAt(i);
                if (target == null) continue;
                target.fieldOfView = fieldOfView.Interpolate(normalizedTime,i,Mathf.LerpUnclamped);
            }
        }

        public override void OnPreviewChanged(AnimoraPlayer animoraPlayer, bool isOn)
        {
            base.OnPreviewChanged(animoraPlayer, isOn);
            for (var i = 0; i < targets.GetTargets().Count; i++)
            {
                var target = targets.GetTargetAt(i);
                if (target == null) continue;
                AnimoraPreviewManager.RecordOrUndoObject(isOn, this, target.fieldOfView,
                    (value) => target.fieldOfView = value);
            }
        }
        
        public override void OnCreate(AnimoraPlayer player)
        {
            base.OnCreate(player);
            ActionsManager.AddActionDirect(AnimoraAction.CreateInstance<AnimoraActionSetCameraFieldOfView>(), player);
        }
    }
}