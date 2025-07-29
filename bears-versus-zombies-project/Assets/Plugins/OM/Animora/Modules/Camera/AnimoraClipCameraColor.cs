using OM.Animora.Runtime;
using UnityEngine;

namespace OM.Animora.Modules
{
    [System.Serializable]
    [AnimoraCreate("Camera Color", "Camera/Camera Color")]
    [AnimoraIcon("d_Camera Icon")]
    [AnimoraKeywords("set", "Camera", "Color", "action")]
    [AnimoraDescription("This is a set Camera Color action")]
    public class AnimoraClipCameraColor : AnimoraClipWithTarget<Camera>
    {
        [SerializeField] private AnimoraInterpolation<Color> backgroundColor;
        
        public override void Enter()
        {
            base.Enter();
            backgroundColor.Setup(targets.GetTargets(), (i) => targets.GetTargetAt(i).backgroundColor,
                (value1, value2) => value1 * value2, CurrentPlayDirection);
        }
        
        public override void OnEvaluate(float time, float clipTime, float normalizedTime, bool isPreviewing)
        {
            base.OnEvaluate(time, clipTime, normalizedTime, isPreviewing);
            for (var i = 0; i < targets.GetTargets().Count; i++)
            {
                var target = targets.GetTargetAt(i);
                if (target == null) continue;
                target.backgroundColor = backgroundColor.Interpolate(normalizedTime,i,Color.LerpUnclamped);
            }
        }
        
        public override void OnPreviewChanged(AnimoraPlayer animoraPlayer, bool isOn)
        {
            base.OnPreviewChanged(animoraPlayer, isOn);
            for (var i = 0; i < targets.GetTargets().Count; i++)
            {
                var target = targets.GetTargetAt(i);
                if (target == null) continue;
                AnimoraPreviewManager.RecordOrUndoObject(isOn, this, target.backgroundColor,
                    (value) => target.backgroundColor = value);
            }
        }
        
        public override void OnCreate(AnimoraPlayer player)
        {
            base.OnCreate(player);
            ActionsManager.AddActionDirect(AnimoraAction.CreateInstance<AnimoraActionSetCameraColor>(), player);
        }
    }
}