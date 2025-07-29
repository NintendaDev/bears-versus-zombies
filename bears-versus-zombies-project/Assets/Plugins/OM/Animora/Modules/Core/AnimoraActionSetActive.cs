using System.Collections.Generic;
using OM.Animora.Runtime;
using UnityEngine;

namespace OM.Animora.Modules
{
    [System.Serializable]
    [AnimoraCreate("Set Active", "Core/Set Active")]
    [AnimoraIcon("AnimationClip Icon")]
    [AnimoraKeywords("set", "Active", "action")]
    [AnimoraDescription("This is a set Active action")]
    public class AnimoraActionSetActive : AnimoraAction
    {
        [SerializeField] private AnimoraSetBool active = AnimoraSetBool.Disabled;

        public override void OnTriggerAction<TTarget>(IEnumerable<TTarget> targets, AnimoraPlayer animoraPlayer, AnimoraClip clip)
        {
            if(active == AnimoraSetBool.Disabled) return;
            
            foreach (var target in targets)
            {
                if (target is Component component)
                {
                    component.gameObject.SetActive(active.GetValue());
                }
                else if (target is GameObject gameObject)
                {
                    gameObject.SetActive(active.GetValue());
                }
                else
                {
                    Debug.LogWarning($"Target {target} is not a GameObject or Component. Skipping.");
                }
            }
        }

        public override bool CanBePreviewed(AnimoraPlayer animoraPlayer) => false;

    }
}