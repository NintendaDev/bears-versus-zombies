using System;
using System.Collections.Generic;
using OM.TimelineCreator.Runtime;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace OM.Animora.Runtime
{
    /// <summary>
    /// Abstract base class for all playable clips within the Animora timeline system.
    /// Defines the core properties, lifecycle methods, and target handling for an animation segment.
    /// </summary>
    [System.Serializable]
    public abstract class AnimoraClip : OM_ClipBase // Inherits from a base timeline clip class
    {
        /// <summary>
        /// Flags for enabling specific debug logs related to this clip's lifecycle events.
        /// </summary>
        [Flags]
        private enum AnimoraClipDebugFlag
        {
            None = 0,
            Enter = 1 << 0,              // Log when Enter() is called
            Exit = 1 << 1,               // Log when Exit() is called
            StartAnimoraPlayer = 1 << 2, // Log when the player starts overall playback involving this clip
            CompleteAnimoraPlayer = 1 << 3,// Log when the player completes overall playback
            StartTimeline = 1 << 4,      // Log when a timeline loop starts involving this clip
            CompleteTimeline = 1 << 5,   // Log when a timeline loop completes
        }

        [SerializeField] private AnimoraClipDebugFlag debugFlags; // Debug flags configurable in the inspector
        [SerializeField, Range(0, 100)] private int playChance = 100; // Percentage chance (0-100) this clip will play if conditions met

        [OM_EndGroup]
        [OM_Group("Events", "Events","isEnabled")]
        [SerializeField] private AnimoraClipEvents events;

        /// <summary>
        /// Gets the current playback direction (Forward/Backward) as determined by the AnimoraPlayer.
        /// Set internally by the player during evaluation setup.
        /// </summary>
        public OM_PlayDirection CurrentPlayDirection { get; private set; }

        /// <summary>
        /// Gets the AnimoraPlayer instance currently evaluating this clip.
        /// Set internally when playback starts.
        /// </summary>
        public AnimoraPlayer Player { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the clip's Enter() method has been called during the current evaluation cycle.
        /// Reset before playback starts and before each loop.
        /// </summary>
        public bool HasEntered { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the clip's Exit() method has been called during the current evaluation cycle.
        /// Reset before playback starts and before each loop.
        /// </summary>
        public bool HasExited { get; set; }

        /// <summary>
        /// Gets a value indicating whether this clip is currently being evaluated in editor preview mode.
        /// Set internally during preview state changes.
        /// </summary>
        public bool IsPreviewing { get; private set; }

        /// <summary>
        /// Gets or sets a value used by the editor preview system to track if the preview evaluation for this clip reached its end.
        /// </summary>
        public bool IsPreviewingCompleted { get; set; }

        // Internal flag to store whether the play chance check passed for the current playback instance.
        private bool _playChancePassed;

        /// <summary>
        /// Called when the script is loaded or a value is changed in the Inspector (Editor only).
        /// Can be used for validation or initialization logic that depends on clip data.
        /// </summary>
        /// <param name="player">The associated AnimoraPlayer.</param>
        public virtual void OnValidate(AnimoraPlayer player)
        {
            // Base implementation does nothing. Subclasses can override for validation.
        }

        /// <summary>
        /// Resets the clip's state before the entire playback sequence begins (PlayAnimation is called).
        /// </summary>
        /// <param name="isPreviewing">True if called during editor preview setup.</param>
        /// <param name="playDirection">The initial play direction.</param>
        public virtual void ResetBeforePlay(bool isPreviewing, OM_PlayDirection playDirection)
        {
            // Default implementation can be empty or provide basic reset logic.
            // Specific clip types might override this.
         }

        /// <summary>
        /// Resets the clip's state immediately before a loop iteration begins.
        /// Ensures flags like HasEntered/HasExited are reset for the new loop pass.
        /// Determines if the clip passes its play chance for this playback instance.
        /// </summary>
        /// <param name="isPreviewing">True if called during editor preview loop setup.</param>
        /// <param name="playDirection">The play direction for the upcoming loop.</param>
        public virtual void ResetClipBeforeStartLoop(bool isPreviewing, OM_PlayDirection playDirection)
        {
            HasEntered = false;
            HasExited = false;
            IsPreviewing = isPreviewing; // Update preview state

            // Determine if the clip passes its random play chance for this run
            if (isPreviewing)
            {
                _playChancePassed = true; // Always allow play during preview
            }
            else
            {
                _playChancePassed = Random.Range(0, 100) < playChance; // Use '<' as Range(0,100) is inclusive max
            }
        }

        /// <summary>
        /// Called once when the AnimoraPlayer begins overall playback (PlayAnimation).
        /// </summary>
        /// <param name="player">The player instance.</param>
        /// <param name="isPreviewing">True if playback is starting in preview mode.</param>
        /// <param name="playDirection">The initial playback direction.</param>
        public virtual void OnStartPlaying(AnimoraPlayer player, bool isPreviewing, OM_PlayDirection playDirection)
        {
            // Cache the player reference if it's new or null
            if (Player == null || Player != player) Player = player;

            OM_Debug.Log($"Start Playing Clip: {GetType().Name}", debugFlags.HasFlag(AnimoraClipDebugFlag.StartAnimoraPlayer), this);

            // Store current playback state
            IsPreviewing = isPreviewing;
            CurrentPlayDirection = playDirection;
            
            events.InvokeOnStartPlaying();
        }

        /// <summary>
        /// Called once when the AnimoraPlayer completes its entire playback sequence (including all loops).
        /// </summary>
        public virtual void OnCompletePlaying()
        {
            OM_Debug.Log($"Complete Playing Clip: {GetType().Name}", debugFlags.HasFlag(AnimoraClipDebugFlag.CompleteAnimoraPlayer), this);
            // Cleanup or final state changes for the clip can go here.
            
            events.InvokeOnCompletePlaying();
        }

        /// <summary>
        /// Called at the beginning of each timeline loop iteration.
        /// </summary>
        /// <param name="isPreviewing">True if the loop is starting in preview mode.</param>
        /// <param name="playDirection">The playback direction for this loop iteration.</param>
        public virtual void OnStartLoop(bool isPreviewing, OM_PlayDirection playDirection)
        {
            // Update the current play direction for this loop
            CurrentPlayDirection = playDirection;
            OM_Debug.Log($"Start Loop Clip: {GetType().Name}", debugFlags.HasFlag(AnimoraClipDebugFlag.StartTimeline), this);
            
            events.InvokeOnStartOneLoop();
        }

        /// <summary>
        /// Called at the end of each timeline loop iteration.
        /// </summary>
        public virtual void OnCompleteLoop()
        {
            OM_Debug.Log($"Complete Loop Clip: {GetType().Name}", debugFlags.HasFlag(AnimoraClipDebugFlag.CompleteTimeline), this);
            
            events.InvokeOnCompleteOneLoop();
        }

        /// <summary>
        /// Called every frame by the evaluation utility while the player's time is within this clip's duration [StartTime, EndTime),
        /// *after* Enter() has been called and *before* Exit() has been called.
        /// </summary>
        /// <param name="time">The current absolute time of the AnimoraPlayer.</param>
        /// <param name="clipTime">The current time relative to the start of this clip (time - GetStartTime()).</param>
        /// <param name="normalizedTime">The current time normalized between 0 (start of clip) and 1 (end of clip).</param>
        /// <param name="isPreviewing">True if being evaluated during editor preview.</param>
        public virtual void OnEvaluate(float time, float clipTime, float normalizedTime, bool isPreviewing)
        {
            // Base implementation does nothing. Subclasses implement animation logic here.
            events.InvokeOnUpdate(normalizedTime);
        }

        /// <summary>
        /// Called by the evaluation utility for *every* frame the clip CanBePlayed, regardless of whether the current time
        /// is within the clip's bounds or if Enter/Exit/OnEvaluate were called.
        /// Its purpose is less common; might be for effects that need the absolute time continuously.
        /// </summary>
        /// <param name="time">The current absolute time of the AnimoraPlayer.</param>
        /// <param name="isPreviewing">True if being evaluated during editor preview.</param>
        public virtual void OnEvaluateAllTime(float time, bool isPreviewing) { }

        /// <summary>
        /// Called by the evaluation utility when the player's time leaves the clip's duration.
        /// Sets the HasExited flag.
        /// </summary>
        public virtual void Exit()
        {
            HasExited = true;
            OM_Debug.Log($"On Exit Clip: {GetType().Name}", debugFlags.HasFlag(AnimoraClipDebugFlag.Exit), this);
            // Cleanup logic when the clip finishes its evaluation cycle.
            
            events.InvokeOnExit();
        }

        /// <summary>
        /// Called by the evaluation utility when the player's time enters the clip's duration [StartTime, EndTime).
        /// Sets the HasEntered flag.
        /// </summary>
        public virtual void Enter()
        {
            HasEntered = true;
            OM_Debug.Log($"On Enter Clip: {GetType().Name}", debugFlags.HasFlag(AnimoraClipDebugFlag.Enter), this);
            // Initialization logic when the clip starts its evaluation cycle. Often used to setup interpolation.
            
            events.InvokeOnEnter();
        }

        /// <summary>
        /// Determines if this clip is allowed to be previewed in the editor.
        /// Default implementation returns true. Subclasses can override for specific conditions.
        /// </summary>
        /// <param name="player">The player instance requesting the preview.</param>
        /// <returns>True if the clip can be previewed, false otherwise.</returns>
        public virtual bool CanBePreviewed(AnimoraPlayer player) => true;

        /// <summary>
        /// Determines if the clip should be played and evaluated in the current context.
        /// Checks if the clip IsActive, if it passed its playChance for this run, and if it has any errors.
        /// </summary>
        /// <returns>True if the clip can be played, false otherwise.</returns>
        public override bool CanBePlayed() // Assumes OM_ClipBase has a virtual CanBePlayed
        {
            // Check base conditions (IsActive, HasError - from OM_ClipBase presumably)
            // And check if the play chance roll succeeded for this playback instance
            return base.CanBePlayed() && _playChancePassed;
            // Simplified version if OM_ClipBase doesn't have CanBePlayed:
            // return IsActive && _playChancePassed && !HasError(out _);
        }

        /// <summary>
        /// Called when the editor preview state changes (starts or stops).
        /// Used to trigger state recording/restoration via AnimoraPreviewManager.
        /// </summary>
        /// <param name="animoraPlayer">The player instance.</param>
        /// <param name="isOn">True if preview is starting, false if stopping.</param>
        public virtual void OnPreviewChanged(AnimoraPlayer animoraPlayer, bool isOn) { }

        /// <summary>
        /// Called specifically when editor preview mode is being entered for this clip.
        /// Resets the preview completion flag.
        /// </summary>
        /// <param name="animoraPlayer">The player instance.</param>
        public virtual void StartPreview(AnimoraPlayer animoraPlayer)
        {
            IsPreviewingCompleted = false;
        }

        /// <summary>
        /// Called specifically when editor preview mode is being exited for this clip.
        /// </summary>
        /// <param name="animoraPlayer">The player instance.</param>
        public virtual void StopPreview(AnimoraPlayer animoraPlayer) { }

        /// <summary>
        /// Called when the AnimoraPlayer is paused.
        /// </summary>
        public virtual void OnPause() { }

        /// <summary>
        /// Called when the AnimoraPlayer is resumed from a paused state.
        /// </summary>
        public virtual void OnResume() { }

        /// <summary>
        /// Called when the AnimoraPlayer is stopped completely.
        /// </summary>
        public virtual void OnStop() { }

        // --- Abstract Methods ---

        /// <summary>
        /// Gets the Type of component this clip is primarily designed to target (e.g., typeof(Transform)).
        /// Used for editor validation and potentially type filtering.
        /// </summary>
        /// <returns>The target component Type.</returns>
        public abstract Type GetTargetType();

        /// <summary>
        /// Gets the actual list of target Components for this clip instance.
        /// </summary>
        /// <returns>A list of Components targeted by this clip.</returns>
        public abstract List<Component> GetTargets();

        // --- Editor-Specific ---

        /// <summary>
        /// Handles drag-and-drop operations onto this clip in the editor, typically for assigning targets.
        /// Default returns false. Should be overridden by clips managing targets (like AnimoraClipWithTarget).
        /// </summary>
        /// <param name="objectReferences">The objects being dropped.</param>
        /// <param name="player">The timeline player instance.</param>
        /// <returns>True if the drop was handled, false otherwise.</returns>
        public virtual bool OnDrop(Object[] objectReferences, IOM_TimelinePlayer<AnimoraClip> player) => false;

        /// <summary>
        /// Called when a clip of this type is first created and added to the timeline (Editor only).
        /// Can be used to add default actions or setup initial state.
        /// </summary>
        /// <param name="player">The player instance.</param>
        public virtual void OnCreate(AnimoraPlayer player) { }
    }
}