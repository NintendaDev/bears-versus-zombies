using System.Collections.Generic;
using OM.Animora.Runtime;
using UnityEngine;

namespace OM.Animora.Modules
{
    [System.Serializable]
    [AnimoraCreate("Set Camera FOV", "Camera/Set Camera FOV")]
    [AnimoraIcon("d_Camera Icon")]
    [AnimoraKeywords("set", "Camera", "action", "FOV","field","of","view")]
    [AnimoraDescription("This is a set Camera FOV action")]
    [AnimoraAction(typeof(Camera))]
    public class AnimoraActionSetCameraFieldOfView : AnimoraActionWithTargets<Camera>
    {
        [SerializeField] private AnimoraValue<float> fieldOfView;

        public override void OnTriggerAction<TTarget>(IEnumerable<TTarget> targets, AnimoraPlayer animoraPlayer, AnimoraClip clip)
        {
            foreach (var target in GetTargets(targets))
            {
                target.fieldOfView = fieldOfView.GetValue(true);
            }
        }

        public override void OnPreviewChanged<TTarget>(AnimoraPlayer animoraPlayer, bool isOn, IEnumerable<TTarget> targets, AnimoraClip clip)
        {
            foreach (var camera in GetTargets(targets))
            {
                AnimoraPreviewManager.RecordOrUndoObject(isOn, this, camera.fieldOfView, (e) => camera.fieldOfView = e);
            }
        }
    }
}