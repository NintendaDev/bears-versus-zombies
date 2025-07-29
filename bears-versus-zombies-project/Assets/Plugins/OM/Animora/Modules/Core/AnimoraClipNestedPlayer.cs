using System;
using System.Collections.Generic;
using OM.Animora.Runtime;
using UnityEngine;

namespace OM.Animora.Modules
{
    [System.Serializable]
    [AnimoraCreate("Nested Player","Core/Nested Player")]
    [AnimoraDescription("Plays a nested player")]
    [AnimoraIcon("AnimationClip Icon")]
    [AnimoraKeywords("Nested Player")]
    public class AnimoraClipNestedPlayer : AnimoraClip
    {
        [OM_StartGroup("Nested Player Settings","Settings")]
        [SerializeField] private AnimoraPlayer nestedPlayer;
        
        public override void OnPreviewChanged(AnimoraPlayer animoraPlayer, bool isOn)
        {
            base.OnPreviewChanged(animoraPlayer, isOn);
            nestedPlayer.OnPreviewStateChanged(isOn);
        }

        public override void OnEvaluate(float time, float clipTime, float normalizedTime, bool isPreviewing)
        {
            base.OnEvaluate(time, clipTime, normalizedTime, isPreviewing);
            var speed = nestedPlayer.GetTimelineDuration() / GetDuration();
            AnimoraClipsPlayUtility.EvaluateForce(nestedPlayer.ClipsToPlay, clipTime * speed);
        }

        public override void Enter()
        {
            base.Enter();
            nestedPlayer.StartPlayingAndStartFirstLoop(CurrentPlayDirection, IsPreviewing);
        }

        public override void Exit()
        {
            base.Exit();

            if (!IsPreviewing)
            {
                nestedPlayer.CompleteLoop();
                nestedPlayer.CompletePlaying();
            }
        }

        public override Type GetTargetType()
        {
            return typeof(AnimoraPlayer);
        }

        public override List<Component> GetTargets()
        {
            return new List<Component>() {nestedPlayer};
        }

        public override bool HasError(out string error)
        {
            error = string.Empty;
            if (nestedPlayer == null)
            {
                error = "Nested Player is not set";
                return true;
            }

            return false;
        }
    }
}