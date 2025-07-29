using System.Collections.Generic;
using OM.Animora.Runtime;
using UnityEngine;

namespace OM.Animora.Modules
{
    [System.Serializable]
    [AnimoraCreate("Set Orthographic Size", "Camera/Set Orthographic Size")]
    [AnimoraIcon("d_Camera Icon")]
    [AnimoraKeywords("set", "Camera", "action", "Orthographic Size")]
    [AnimoraDescription("This is a set Camera Orthographic Size action")]
    [AnimoraAction(typeof(Camera))]
    public class AnimoraActionSetCameraOrthographicSize : AnimoraActionWithTargets<Camera>
    {
        [SerializeField] private AnimoraValue<float> orthographicSize;

        public override void OnTriggerAction<TTarget>(IEnumerable<TTarget> targets, AnimoraPlayer animoraPlayer, AnimoraClip clip)
        {
            foreach (var target in GetTargets(targets))
            {
                target.orthographicSize = orthographicSize.GetValue(true);
            }
        }

        public override void OnPreviewChanged<TTarget>(AnimoraPlayer animoraPlayer, bool isOn,
            IEnumerable<TTarget> targets, AnimoraClip clip)
        {
            foreach (var camera in GetTargets(targets))
            {
                AnimoraPreviewManager.RecordOrUndoObject(isOn, this, camera.orthographicSize,
                    (e) => camera.orthographicSize = e);
            }
        }
    }
}