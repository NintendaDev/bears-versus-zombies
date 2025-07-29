using OM.Animora.Runtime;
using UnityEngine;

namespace OM.Animora.Modules
{
    [System.Serializable]
    [AnimoraCreate("Rotation", "Transform/Rotation")]
    [AnimoraIcon("d_Transform Icon")]
    [AnimoraKeywords("Rotation", "animate","Rotate")]
    [AnimoraDescription("This is a rotation animation clip")]
    public class AnimoraClipRotation : AnimoraClipWithTarget<Transform>
    {
        [OM_StartGroup("Rotation")]
        [SerializeField] private Space space = Space.World;
        [SerializeField] private AnimoraInterpolation<Vector3> rotation;
        
        public override void Enter()
        {
            base.Enter();
            rotation.Setup(targets.GetTargets(), (i) => targets.GetTargetAt(i).GetRotation(space).eulerAngles,
                (value1, value2) => value1 + value2,CurrentPlayDirection);
        }
        
        public override void OnEvaluate(float time, float clipTime, float normalizedTime, bool isPreviewing)
        {
            base.OnEvaluate(time, clipTime, normalizedTime, isPreviewing);
            for (var i = 0; i < targets.GetTargets().Count; i++)
            {
                var target = targets.GetTargetAt(i);
                if (target == null) continue;
                target.SetRotation(Quaternion.Euler(rotation.Interpolate(normalizedTime,i,Vector3.LerpUnclamped)),space);
            }
        }
        
        public override void OnPreviewChanged(AnimoraPlayer animoraPlayer, bool isOn)
        {
            base.OnPreviewChanged(animoraPlayer, isOn);
            for (var i = 0; i < targets.GetTargets().Count; i++)
            {
                var target = targets.GetTargetAt(i);
                if (target == null) continue;
                AnimoraPreviewManager.RecordOrUndoObject(isOn, this, target.GetRotation(space).eulerAngles,
                    (value) => target.SetRotation(Quaternion.Euler(value),space));
            }
        }
        
        public override void OnCreate(AnimoraPlayer player)
        {
            base.OnCreate(player);
            ActionsManager.AddActionDirect(AnimoraAction.CreateInstance<AnimoraActionSetRotation>(), player);
        }
        
    }
}