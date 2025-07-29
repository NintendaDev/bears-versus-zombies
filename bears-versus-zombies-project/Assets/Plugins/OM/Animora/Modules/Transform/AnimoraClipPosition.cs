using OM.Animora.Runtime;
using UnityEngine;

namespace OM.Animora.Modules
{
    [System.Serializable]
    [AnimoraCreate("Position", "Transform/Position")]
    [AnimoraIcon("d_Transform Icon")]
    [AnimoraKeywords("Position", "animate","Move")]
    [AnimoraDescription("This is a position animation clip")]
    public class AnimoraClipPosition : AnimoraClipWithTarget<Transform>
    {
        [OM_StartGroup("Position")]
        [SerializeField] private Space space = Space.World;
        [SerializeField] private AnimoraInterpolation<Vector3> position;
        
        
        public override void Enter()
        {
            base.Enter();
            position.Setup(targets.GetTargets(), (i) => targets.GetTargetAt(i).GetPosition(space),
                (value1, value2) => value1 + value2,CurrentPlayDirection);
        }
        
        public override void OnEvaluate(float time, float clipTime, float normalizedTime, bool isPreviewing)
        {
            base.OnEvaluate(time, clipTime, normalizedTime, isPreviewing);
            for (var i = 0; i < targets.GetTargets().Count; i++)
            {
                var target = targets.GetTargetAt(i);
                if (target == null) continue;
                target.SetPosition(position.Interpolate(normalizedTime,i,Vector3.LerpUnclamped),space);
            }
        }
        
        public override void OnPreviewChanged(AnimoraPlayer animoraPlayer, bool isOn)
        {
            base.OnPreviewChanged(animoraPlayer, isOn);
            for (var i = 0; i < targets.GetTargets().Count; i++)
            {
                var target = targets.GetTargetAt(i);
                if (target == null) continue;
                AnimoraPreviewManager.RecordOrUndoObject(isOn, this, target.GetPosition(space),
                    (value) => target.SetPosition(value,space));
            }
        }
        
        public override void OnCreate(AnimoraPlayer player)
        {
            base.OnCreate(player);
            ActionsManager.AddActionDirect(AnimoraAction.CreateInstance<AnimoraActionSetPosition>(), player);
        }
    }
}