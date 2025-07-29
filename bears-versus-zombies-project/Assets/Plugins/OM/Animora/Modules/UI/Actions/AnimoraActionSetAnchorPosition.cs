using System.Collections.Generic;
using OM.Animora.Runtime;
using UnityEngine;

namespace OM.Animora.Modules
{
    [System.Serializable]
    [AnimoraCreate("Set Anchor Position", "UI/Set Anchor Position")]
    [AnimoraIcon("AnimationClip Icon")]
    [AnimoraKeywords("set", "Anchor", "Position", "action")]
    [AnimoraDescription("This is a set Anchor Position action")]
    [AnimoraAction(typeof(RectTransform))]
    public class AnimoraActionSetAnchorPosition : AnimoraActionWithTargets<RectTransform>
    {
        [SerializeField] private AnimoraValue<Vector3> anchorPosition;
        
        public override void OnTriggerAction<TTarget>(IEnumerable<TTarget> targets, AnimoraPlayer animoraPlayer, AnimoraClip clip)
        {
            foreach (var target in GetTargets(targets))
            {
                target.anchoredPosition3D = anchorPosition.GetValue(true);
            }
        }
        
        public override void OnPreviewChanged<TTarget>(AnimoraPlayer animoraPlayer, bool isOn, IEnumerable<TTarget> targets, AnimoraClip clip)
        {
            foreach (var rectTransform in GetTargets(targets))
            {
                AnimoraPreviewManager.RecordOrUndoObject(isOn,this,rectTransform.anchoredPosition3D, (e)=> rectTransform.anchoredPosition3D = e);
            }
        }
        
    }
}