using OM.Animora.Runtime;
using UnityEngine;

namespace OM.Animora.Modules
{
    [System.Serializable]
    [AnimoraCreate("Scale", "Transform/Scale")]
    [AnimoraIcon("d_Transform Icon")]
    [AnimoraKeywords("Scale", "action")]
    [AnimoraDescription("This is a scale action")]
    public class AnimoraClipScale : AnimoraClipWithTarget<Transform>
    {
        [OM_StartGroup("Scale")]
        [SerializeField] private AnimoraInterpolation<Vector3> scale;
        
        public override void Enter()
        {
            base.Enter();
            scale.Setup(targets.GetTargets(), (i) => targets.GetTargetAt(i).localScale,
                (value1, value2) => value1 + value2,CurrentPlayDirection);
        }
        
        public override void OnEvaluate(float time, float clipTime, float normalizedTime, bool isPreviewing)
        {
            base.OnEvaluate(time, clipTime, normalizedTime, isPreviewing);
            for (var i = 0; i < targets.GetTargets().Count; i++)
            {
                var target = targets.GetTargetAt(i);
                if (target == null) continue;
                target.localScale = scale.Interpolate(normalizedTime,i,Vector3.LerpUnclamped);
            }
        }
        
        public override void OnPreviewChanged(AnimoraPlayer animoraPlayer, bool isOn)
        {
            base.OnPreviewChanged(animoraPlayer, isOn);
            for (var i = 0; i < targets.GetTargets().Count; i++)
            {
                var target = targets.GetTargetAt(i);
                if (target == null) continue;
                AnimoraPreviewManager.RecordOrUndoObject(isOn, this, target.localScale,
                    (value) => target.localScale = value);
            }
        }
        
        public override void OnCreate(AnimoraPlayer player)
        {
            base.OnCreate(player);
            ActionsManager.AddActionDirect(AnimoraAction.CreateInstance<AnimoraActionSetScale>(), player);
        }
    }
}