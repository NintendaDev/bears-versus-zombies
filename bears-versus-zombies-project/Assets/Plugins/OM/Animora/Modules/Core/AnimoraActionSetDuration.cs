using System.Collections.Generic;
using OM.Animora.Runtime;
using UnityEngine;

namespace OM.Animora.Modules
{
    [System.Serializable]
    [AnimoraCreate("Set Duration", "Core/Set Duration")]
    [AnimoraIcon("AnimationClip Icon")]
    [AnimoraKeywords("Set", "Duration", "Clip", "Action", "Animora")]
    [AnimoraDescription("This is a set duration action")]
    public class AnimoraActionSetDuration : AnimoraAction
    {
        [SerializeField] private AnimoraSetType animoraSetType;
        [SerializeField] private AnimoraValue<float> duration;

        public override void OnValidate(AnimoraPlayer player, AnimoraClip clip)
        {
            base.OnValidate(player, clip);
            duration.Value = Mathf.Max(duration.Value, 0.001f);
            duration.RandomValue1 = Mathf.Max(duration.RandomValue1, 0.001f);
            duration.RandomValue2 = Mathf.Max(duration.RandomValue2, 0.001f);
        }

        public override void OnTriggerAction<TTarget>(IEnumerable<TTarget> targets, AnimoraPlayer animoraPlayer,
            AnimoraClip clip)
        {
            var newDuration = duration.GetValue(true);
            if (animoraSetType == AnimoraSetType.Add)
            {
                newDuration += clip.GetDuration();
            }

            newDuration = Mathf.Clamp(newDuration, 0.001f, animoraPlayer.GetTimelineDuration() - clip.GetStartTime());
            clip.SetDuration(newDuration);
            animoraPlayer.OnValidate();
        }

        public override bool CanBePreviewed(AnimoraPlayer animoraPlayer) => false;
    }
}