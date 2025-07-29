using System.Collections.Generic;
using UnityEngine;

namespace OM.Animora.Runtime
{
    /// <summary>
    /// Provides static utility methods for managing the playback lifecycle and evaluation
    /// of collections of <see cref="AnimoraClip"/> instances. This class centralizes the logic
    /// for iterating over clips and calling their respective lifecycle methods (Reset, Start, Complete, Evaluate),
    /// simplifying the implementation within the <see cref="AnimoraPlayer"/>.
    /// </summary>
    public static class AnimoraClipsPlayUtility
    {
        /// <summary>
        /// Calls the <see cref="AnimoraClip.ResetBeforePlay"/> method on each clip in the provided list.
        /// Typically used once before starting the entire timeline playback sequence from the beginning.
        /// </summary>
        /// <param name="clips">The list of <see cref="AnimoraClip"/> instances to reset.</param>
        /// <param name="isPreviewing">Indicates if the reset is happening in preview mode.</param>
        /// <param name="playDirection">The intended playback direction for the upcoming playback sequence.</param>
        public static void ResetBeforePlay(List<AnimoraClip> clips, bool isPreviewing, OM_PlayDirection playDirection)
        {
            // Check if the provided list is null to prevent errors.
            if (clips == null) return;
            // Iterate through each clip in the list.
            foreach (var clip in clips)
            {
                // If a clip instance is not null, call its reset method.
                clip?.ResetBeforePlay(isPreviewing, playDirection);
            }
        }

        /// <summary>
        /// Calls the <see cref="AnimoraClip.ResetClipBeforeStartLoop"/> method on each clip in the provided collection.
        /// This is used before starting each loop iteration (including the first one if called after ResetBeforePlay)
        /// or when changing direction in PingPong mode. It resets loop-specific state like Enter/Exit flags.
        /// </summary>
        /// <param name="clips">The collection of <see cref="AnimoraClip"/> instances to reset for the loop.</param>
        /// <param name="isPreviewing">Indicates if the reset is happening in preview mode.</param>
        /// <param name="playDirection">The intended playback direction for the upcoming loop.</param>
        public static void ResetBeforeStartLoop(IEnumerable<AnimoraClip> clips, bool isPreviewing, OM_PlayDirection playDirection)
        {
            // Check if the provided collection is null.
            if (clips == null) return;
            // Iterate through each clip in the collection.
            foreach (var clip in clips)
            {
                // If a clip instance is not null, call its loop reset method.
                clip?.ResetClipBeforeStartLoop(isPreviewing, playDirection);
            }
        }

        /// <summary>
        /// Calls the <see cref="AnimoraClip.OnStartPlaying"/> method on each clip in the provided collection.
        /// This signals the beginning of the entire timeline playback sequence.
        /// </summary>
        /// <param name="clips">The collection of <see cref="AnimoraClip"/> instances to notify.</param>
        /// <param name="player">The <see cref="AnimoraPlayer"/> instance initiating the playback.</param>
        /// <param name="isPreviewing">Indicates if playback is starting in preview mode.</param>
        /// <param name="playDirection">The initial playback direction.</param>
        public static void StartPlaying(IEnumerable<AnimoraClip> clips, AnimoraPlayer player, bool isPreviewing, OM_PlayDirection playDirection)
        {
            // Check if the provided collection is null.
            if (clips == null) return;
            // Iterate through each clip in the collection.
            foreach (var clip in clips)
            {
                // If a clip instance is not null, call its start playing method.
                clip?.OnStartPlaying(player, isPreviewing, playDirection);
            }
        }

        /// <summary>
        /// Calls the <see cref="AnimoraClip.OnCompletePlaying"/> method on each clip in the provided collection.
        /// This signals the end of the entire timeline playback sequence (e.g., after all loops are finished or playback is stopped).
        /// </summary>
        /// <param name="clips">The collection of <see cref="AnimoraClip"/> instances to notify.</param>
        public static void CompletePlaying(IEnumerable<AnimoraClip> clips)
        {
            // Check if the provided collection is null.
            if (clips == null) return;
            // Iterate through each clip in the collection.
            foreach (var clip in clips)
            {
                // If a clip instance is not null, call its complete playing method.
                clip?.OnCompletePlaying();
            }
        }

        /// <summary>
        /// Calls the <see cref="AnimoraClip.OnStartLoop"/> method on each clip in the provided collection.
        /// Signals the beginning of a new loop iteration.
        /// </summary>
        /// <param name="clips">The collection of <see cref="AnimoraClip"/> instances to notify.</param>
        /// <param name="isPreviewing">Indicates if the loop is starting in preview mode.</param>
        /// <param name="playDirection">The playback direction for this specific loop iteration.</param>
        public static void StartLoop(IEnumerable<AnimoraClip> clips, bool isPreviewing, OM_PlayDirection playDirection)
        {
            // Check if the provided collection is null.
            if (clips == null) return;
            // Iterate through each clip in the collection.
            foreach (var clip in clips)
            {
                // If a clip instance is not null, call its start loop method.
                clip?.OnStartLoop(isPreviewing, playDirection);
            }
        }

        /// <summary>
        /// Calls the <see cref="AnimoraClip.OnCompleteLoop"/> method on each clip in the provided collection.
        /// Signals the end of the current loop iteration.
        /// </summary>
        /// <param name="clips">The collection of <see cref="AnimoraClip"/> instances to notify.</param>
        public static void CompleteLoop(IEnumerable<AnimoraClip> clips)
        {
            // Check if the provided collection is null.
            if (clips == null) return;
            // Iterate through each clip in the collection.
            foreach (var clip in clips)
            {
                // If a clip instance is not null, call its complete loop method.
                clip?.OnCompleteLoop();
            }
        }

        /// <summary>
        /// Evaluates the state of each clip in the collection based on the current timeline time and direction during normal playback.
        /// Handles calling Enter, Exit, and OnEvaluate on individual clips based on their start/end times and current state.
        /// </summary>
        /// <param name="clips">The collection of <see cref="AnimoraClip"/> instances to evaluate.</param>
        /// <param name="time">The current playback time in seconds.</param>
        /// <param name="playDirection">The current playback direction (Forward or Backward).</param>
        public static void Evaluate(IEnumerable<AnimoraClip> clips, float time, OM_PlayDirection playDirection)
        {
            // Check if the provided collection is null.
            if (clips == null) return;

            // Determine if playback is currently moving forward.
            var isForward = playDirection == OM_PlayDirection.Forward;

            // Iterate through each clip in the collection.
            foreach (var clip in clips)
            {
                // If a clip is null or cannot be played (e.g., inactive, failed play chance), skip it.
                if (clip == null || !clip.CanBePlayed()) continue;

                // Get the clip's start and end times.
                var startTime = clip.GetStartTime();
                var endTime = clip.GetEndTime();
                // Calculate the clip's duration.
                var duration = clip.GetDuration();
                // Ensure duration is not zero to avoid division by zero errors later.
                var safeDuration = duration <= 0 ? 1f : duration; // Use 1f as a fallback if duration is zero or negative

                // --- Forward Playback Logic ---
                if (isForward)
                {
                    // Check if the clip should Enter (first time entering its time range).
                    if (clip.HasEntered == false && time >= startTime && time <= endTime)
                    {
                        // Mark as entered and call the Enter method.
                        clip.HasEntered = true;
                        clip.Enter();
                        // Immediately evaluate at the entry point.
                        // Calculate normalized time carefully, ensuring it's clamped between 0 and 1.
                        var normalizedTime = Mathf.Clamp01((time - startTime) / safeDuration);
                        clip.OnEvaluate(time, time - startTime, normalizedTime, false);
                    }
                    // Check if the clip should Exit (was entered, hasn't exited, and time is now past its end).
                    else if (clip.HasEntered && clip.HasExited == false && time > endTime)
                    {
                        // Evaluate one last time exactly at the end of the clip (normalized time 1).
                        clip.OnEvaluate(endTime, duration, 1, false);
                        // Mark as exited and call the Exit method.
                        clip.HasExited = true;
                        clip.Exit();
                    }
                    // Check if the clip is currently active (entered, not exited).
                    else if (clip.HasEntered && clip.HasExited == false)
                    {
                        // Calculate current time relative to the clip's start.
                        var clipTime = time - startTime;
                        // Calculate normalized time (progress within the clip, 0 to 1).
                        var normalizedTime = Mathf.Clamp01(clipTime / safeDuration);
                        // Call the regular evaluation method.
                        clip.OnEvaluate(time, clipTime, normalizedTime, false);
                    }
                }
                // --- Backward Playback Logic ---
                else // isForward == false
                {
                    // Check if the clip should Enter (when moving backward, entry happens when time <= endTime).
                    // Note: For backward playback, 'Enter' usually means re-entering the clip's active range.
                    // Resetting HasEntered/HasExited typically happens in ResetClipBeforeStartLoop.
                    if (clip.HasEntered == false && time <= endTime && time >= startTime) // Condition modified for backward entry
                    {
                        // Mark as entered and call Enter.
                        clip.HasEntered = true;
                        clip.Enter();
                         // Immediately evaluate at the entry point.
                        // Calculate normalized time carefully for backward play.
                        var normalizedTime = Mathf.Clamp01((time - startTime) / safeDuration);
                        clip.OnEvaluate(time, time - startTime, normalizedTime, false);
                    }
                    // Check if the clip should Exit (was entered, hasn't exited, and time is now before its start).
                    else if (clip.HasEntered && clip.HasExited == false && time < startTime) // Condition modified for backward exit
                    {
                        // Evaluate one last time exactly at the beginning of the clip (normalized time 0).
                        clip.OnEvaluate(startTime, 0, 0, false);
                        // Mark as exited and call Exit.
                        clip.HasExited = true;
                        clip.Exit();
                    }
                    // Check if the clip is currently active (entered, not exited).
                    else if (clip.HasEntered && clip.HasExited == false)
                    {
                        // Calculate current time relative to the clip's start.
                        var clipTime = time - startTime;
                        // Calculate normalized time (progress within the clip, 0 to 1).
                        var normalizedTime = Mathf.Clamp01(clipTime / safeDuration);
                        // Call the regular evaluation method.
                        clip.OnEvaluate(time, clipTime, normalizedTime, false);
                    }
                }
                 // Call the method that evaluates regardless of enter/exit state (e.g., for continuous updates if needed).
                 // Pass 'false' as this is normal playback, not preview scrubbing.
                clip.OnEvaluateAllTime(time, false);
            }
        }

        /// <summary>
        /// Forces evaluation of all clips at a specific time, typically used for preview scrubbing in the editor.
        /// It bypasses the normal Enter/Exit logic and directly calls evaluation methods, marking preview state.
        /// Handles setting the `IsPreviewingCompleted` flag when scrubbing past the end.
        /// </summary>
        /// <param name="clips">The collection of <see cref="AnimoraClip"/> instances to evaluate.</param>
        /// <param name="time">The specific time to evaluate at (e.g., the scrub head position).</param>
        public static void EvaluateForce(IEnumerable<AnimoraClip> clips, float time)
        {
            // Check if the provided collection is null.
            if (clips == null) return;

            // Iterate through each clip in the collection.
            foreach (var clip in clips)
            {
                 // If a clip is null, skip it.
                 if (clip == null) continue;

                // Call the evaluation method that runs always, indicating it's a preview update.
                clip.OnEvaluateAllTime(time, true);

                // Get clip timings.
                var startTime = clip.GetStartTime();
                var endTime = clip.GetEndTime();
                var duration = clip.GetDuration();
                var safeDuration = duration <= 0 ? 1f : duration; // Avoid division by zero
                
                 // --- Logic for Preview State ---

                 if (clip.IsPreviewingCompleted)
                 {
                     var clipTime = time - startTime;
                     var normalizedTime = Mathf.Clamp01(clipTime / safeDuration);
                     // Evaluate the clip at the current scrub time.
                     clip.OnEvaluate(time, clipTime, normalizedTime, true); // isPreviewing = true
                     continue;
                 }

                 // Check if scrub time is beyond the clip's end time.
                 if (time >= endTime)
                 {
                     // If the clip was active (entered but not exited) during the scrub,
                     // ensure it gets a final evaluation at its end point and Exit is called conceptually.
                     if (clip.HasEntered && clip.HasExited == false)
                     {
                         // Evaluate at the end state (normalized time 1).
                         clip.OnEvaluate(endTime, duration, 1, true); // isPreviewing = true
                         // Conceptually exit for the preview state.
                         clip.Exit();
                         // Mark that this specific clip's preview evaluation is completed (reached or passed the end).
                         clip.IsPreviewingCompleted = true;
                     }
                     // Ensure state flags are reset if scrubbing past the end,
                     // regardless of previous state (allows re-entry if scrubbing back).
                     clip.HasExited = false; // Reset for potential re-entry when scrubbing back
                     clip.HasEntered = false; // Reset for potential re-entry when scrubbing back
                 }
                 // Check if scrub time is within the clip's duration.
                 else if (time >= startTime) // time < endTime is implied by the 'else' from the above 'if'
                 {
                     // If clip hasn't been marked as entered during this preview scrub...
                     if (clip.HasEntered == false)
                     {
                         // Mark as entered and call Enter.
                         clip.HasEntered = true;
                         // Reset exit flag in case we scrubbed back past the end.
                         clip.HasExited = false;
                         // Reset completion flag.
                         clip.IsPreviewingCompleted = false;
                         clip.Enter();
                     }
                     // Calculate relative clip time and normalized time.
                     var clipTime = time - startTime;
                     var normalizedTime = Mathf.Clamp01(clipTime / safeDuration);
                     // Evaluate the clip at the current scrub time.
                     clip.OnEvaluate(time, clipTime, normalizedTime, true); // isPreviewing = true
                 }
                 // Check if scrub time is before the clip's start time.
                 else // time < startTime
                 {
                     // If the clip was previously entered during this scrub...
                     if (clip.HasEntered)
                     {
                         // Evaluate at the beginning state (normalized time 0).
                         clip.OnEvaluate(startTime, 0, 0, true); // isPreviewing = true
                         // Mark as exited conceptually for the preview.
                         clip.HasExited = true;
                         // Reset entered flag.
                         clip.HasEntered = false;
                         // Reset completion flag.
                         clip.IsPreviewingCompleted = false;
                         clip.Exit();
                     }
                      // Ensure flags remain reset if scrubbing before the start.
                     clip.HasEntered = false;
                     clip.HasExited = false; // Keep HasExited false? Or true? Depends on desired scrub-back behavior. Usually false.
                     clip.IsPreviewingCompleted = false;
                 }
            }
        }


        /// <summary>
        /// Notifies each clip in the collection about a change in the editor's preview state (enabled/disabled).
        /// Calls the appropriate methods on the clip (<see cref="AnimoraClip.OnPreviewChanged"/>,
        /// <see cref="AnimoraClip.StartPreview"/>, <see cref="AnimoraClip.StopPreview"/>).
        /// </summary>
        /// <param name="clips">The collection of <see cref="AnimoraClip"/> instances to notify.</param>
        /// <param name="player">The <see cref="AnimoraPlayer"/> instance (used for context, e.g., CanBePreviewed check).</param>
        /// <param name="isOn">True if preview mode is being activated, false if deactivated.</param>
        public static void OnPreviewStateChanged(IEnumerable<AnimoraClip> clips, AnimoraPlayer player, bool isOn)
        {
            // Check if the provided collection is null.
            if (clips == null) return;
            // Iterate through each clip in the collection.
            foreach (var clip in clips)
            {
                // If clip is null, inactive, has errors, or cannot be previewed, skip it.
                if (clip == null || !clip.IsActive || clip.HasError(out _) || !clip.CanBePreviewed(player)) continue;

                // Notify the clip about the general preview state change.
                clip.OnPreviewChanged(player, isOn);

                // Call specific Start/Stop preview methods.
                if (isOn)
                {
                    clip.StartPreview(player);
                }
                else
                {
                    clip.StopPreview(player);
                }
            }
        }
    }
}