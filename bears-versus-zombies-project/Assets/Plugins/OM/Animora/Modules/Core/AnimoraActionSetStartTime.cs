using System.Collections.Generic;
using OM.Animora.Runtime;
using UnityEngine;

namespace OM.Animora.Modules
{
    [System.Serializable]
    [AnimoraCreate("Set Start Time", "Core/Set Start Time")]
    [AnimoraIcon("AnimationClip Icon")]
    [AnimoraKeywords("Set Start Time")]
    [AnimoraDescription("This is a set Start Time action")]
    public class AnimoraActionSetStartTime : AnimoraAction
    {
        [SerializeField] private AnimoraSetType setType;
        [SerializeField] private AnimoraValue<float> startTime;

        public override void OnValidate(AnimoraPlayer player, AnimoraClip clip)
        {
            base.OnValidate(player, clip);
            startTime.Value = Mathf.Max(startTime.Value, 0.001f);
            startTime.RandomValue1 = Mathf.Max(startTime.RandomValue1, 0.001f);
            startTime.RandomValue2 = Mathf.Max(startTime.RandomValue2, 0.001f);
        }

        public override void OnTriggerAction<TTarget>(IEnumerable<TTarget> targets, AnimoraPlayer animoraPlayer, AnimoraClip clip)
        {
            var newStartTime = startTime.GetValue(true);
            if (setType == AnimoraSetType.Add)
            {
                newStartTime += clip.GetStartTime();
            }

            newStartTime = Mathf.Clamp(newStartTime,0.001f,animoraPlayer.GetTimelineDuration() - clip.GetDuration());
            clip.SetStartTime(newStartTime);
            animoraPlayer.OnValidate();
        }

        public override bool CanBePreviewed(AnimoraPlayer animoraPlayer) => false;
    }
}