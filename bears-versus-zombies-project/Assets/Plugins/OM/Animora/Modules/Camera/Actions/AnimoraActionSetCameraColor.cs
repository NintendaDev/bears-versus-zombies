using System.Collections.Generic;
using OM.Animora.Runtime;
using UnityEngine;

namespace OM.Animora.Modules
{
    [System.Serializable]
    [AnimoraCreate("Set Camera Color", "Camera/Set Camera Color")]
    [AnimoraIcon("d_Camera Icon")]
    [AnimoraKeywords("set", "Camera", "action", "Color")]
    [AnimoraDescription("This is a set Camera Color action")]
    [AnimoraAction(typeof(Camera))]
    public class AnimoraActionSetCameraColor : AnimoraActionWithTargets<Camera>
    {
        [SerializeField] private AnimoraValue<Color> color;

        public override void OnTriggerAction<TTarget>(IEnumerable<TTarget> targets, AnimoraPlayer animoraPlayer, AnimoraClip clip)
        {
            foreach (var target in GetTargets(targets))
            {
                target.backgroundColor = color.GetValue(true);
            }
        }

        public override void OnPreviewChanged<TTarget>(AnimoraPlayer animoraPlayer, bool isOn, IEnumerable<TTarget> targets, AnimoraClip clip)
        {
            foreach (var camera in GetTargets(targets))
            {
                AnimoraPreviewManager.RecordOrUndoObject(isOn, this, camera.backgroundColor, (e) => camera.backgroundColor = e);
            }
        }
    }
}