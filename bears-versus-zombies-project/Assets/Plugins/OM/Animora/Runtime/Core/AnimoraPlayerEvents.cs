using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace OM.Animora.Runtime
{
    /// <summary>
    /// A serializable class that encapsulates UnityEvents and C# Actions for various
    /// lifecycle events of an <see cref="AnimoraPlayer"/>.
    /// This allows designers to hook up responses to player events directly in the Unity Inspector
    /// using UnityEvents, while also providing C# Actions for programmatic event handling.
    /// </summary>
    [System.Serializable] // Ensures this class can be serialized by Unity when part of AnimoraPlayer.
    public class AnimoraPlayerEvents
    {
        /// <summary>
        /// If false, all events (both UnityEvents and C# Actions) within this instance will be ignored when invoked.
        /// </summary>
        [SerializeField]
        [Tooltip("If disabled, none of the events below will be invoked.")]
        private bool enabled = false; // Default to disabled

        // --- UnityEvents (Exposed in Inspector) ---

        /// <summary>
        /// UnityEvent invoked when the player starts the entire playback sequence (e.g., via PlayAnimation).
        /// Corresponds to the initial call before any loops begin.
        /// </summary>
        [SerializeField] private UnityEvent onStartPlaying;
        /// <summary>
        /// UnityEvent invoked when the player completes the entire playback sequence (e.g., after all loops or if stopped).
        /// </summary>
        [SerializeField] private UnityEvent onCompletePlaying;
        /// <summary>
        /// UnityEvent invoked at the beginning of each timeline loop iteration (including the first one).
        /// </summary>
        [SerializeField] private UnityEvent onStartLoop; // Often considered the start of a loop
        /// <summary>
        /// UnityEvent invoked at the end of each timeline loop iteration.
        /// </summary>
        [SerializeField] private UnityEvent onCompleteLoop; // Often considered the end of a loop
        /// <summary>
        /// UnityEvent invoked when the player is paused via <see cref="AnimoraPlayer.PauseAnimation"/>.
        /// </summary>
        [SerializeField] private UnityEvent onPause;
        /// <summary>
        /// UnityEvent invoked when the player is resumed from a paused state via <see cref="AnimoraPlayer.ResumeAnimation"/>.
        /// </summary>
        [SerializeField] private UnityEvent onResume;
        /// <summary>
        /// UnityEvent invoked when the player is stopped via <see cref="AnimoraPlayer.StopAnimation"/>.
        /// </summary>
        [SerializeField] private UnityEvent onStop;

        // --- C# Actions (For programmatic subscriptions) ---
        // These provide equivalent events for scripting purposes.

        /// <summary> C# Action invoked when playback starts. </summary>
        private event Action StartPlayingAction;
        /// <summary> C# Action invoked when playback completes. </summary>
        private event Action CompletePlayingAction;
        /// <summary> C# Action invoked when a timeline loop starts. </summary>
        private event Action StartLoopAction;
        /// <summary> C# Action invoked when a timeline loop completes. </summary>
        private event Action CompleteLoopAction;
        /// <summary> C# Action invoked when playback is paused. </summary>
        private event Action PauseAction;
        /// <summary> C# Action invoked when playback is resumed. </summary>
        private event Action ResumeAction;
        /// <summary> C# Action invoked when playback is stopped. </summary>
        private event Action StopAction;

        // --- Public Methods for Subscribing to C# Actions ---

        /// <summary> Subscribes a method to the StartPlaying event. </summary>
        public void AddOnStartPlaying(Action action) { StartPlayingAction += action; }
        /// <summary> Unsubscribes a method from the StartPlaying event. </summary>
        public void RemoveOnStartPlaying(Action action) { StartPlayingAction -= action; }

        /// <summary> Subscribes a method to the CompletePlaying event. </summary>
        public void AddOnCompletePlaying(Action action) { CompletePlayingAction += action; }
        /// <summary> Unsubscribes a method from the CompletePlaying event. </summary>
        public void RemoveOnCompletePlaying(Action action) { CompletePlayingAction -= action; }

        /// <summary> Subscribes a method to the StartTimeline (loop start) event. </summary>
        public void AddOnStartLoop(Action action) { StartLoopAction += action; }
        /// <summary> Unsubscribes a method from the StartTimeline (loop start) event. </summary>
        public void RemoveOnStartLoop(Action action) { StartLoopAction -= action; }

        /// <summary> Subscribes a method to the CompleteTimeline (loop end) event. </summary>
        public void AddOnCompleteLoop(Action action) { CompleteLoopAction += action; }
        /// <summary> Unsubscribes a method from the CompleteTimeline (loop end) event. </summary>
        public void RemoveOnCompleteLoop(Action action) { CompleteLoopAction -= action; }

        /// <summary> Subscribes a method to the Pause event. </summary>
        public void AddOnPause(Action action) { PauseAction += action; }
        /// <summary> Unsubscribes a method from the Pause event. </summary>
        public void RemoveOnPause(Action action) { PauseAction -= action; }

        /// <summary> Subscribes a method to the Resume event. </summary>
        public void AddOnResume(Action action) { ResumeAction += action; }
        /// <summary> Unsubscribes a method from the Resume event. </summary>
        public void RemoveOnResume(Action action) { ResumeAction -= action; }

        /// <summary> Subscribes a method to the Stop event. </summary>
        public void AddOnStop(Action action) { StopAction += action; }
        /// <summary> Unsubscribes a method from the Stop event. </summary>
        public void RemoveOnStop(Action action) { StopAction -= action; }

        /// <summary>
        /// Removes all subscribers from all C# Action events.
        /// Does not affect UnityEvent listeners configured in the inspector.
        /// </summary>
        public void ClearActions()
        {
            StartPlayingAction = null;
            CompletePlayingAction = null;
            StartLoopAction = null;
            CompleteLoopAction = null;
            PauseAction = null;
            ResumeAction = null;
            StopAction = null;
        }

        /// <summary>
        /// Enables or disables the invocation of all events (UnityEvents and C# Actions) in this container.
        /// </summary>
        /// <param name="isEnabled">True to enable event invocation, false to disable.</param>
        public void SetEnabled(bool isEnabled)
        {
            enabled = isEnabled;
        }

        // --- Public Methods for Invoking Events (Called by AnimoraPlayer) ---

        /// <summary> Invokes the StartPlaying UnityEvent and C# Action if enabled. </summary>
        public void InvokeOnStartPlaying()
        {
            // Only invoke if the 'enabled' flag is true.
            if(enabled) onStartPlaying?.Invoke(); // Null-conditional invocation for UnityEvent
            // Invoke the C# Action using the null-conditional operator.
            StartPlayingAction?.Invoke();
        }

        /// <summary> Invokes the CompletePlaying UnityEvent and C# Action if enabled. </summary>
        public void InvokeOnCompletePlaying()
        {
            if(enabled) onCompletePlaying?.Invoke();
            CompletePlayingAction?.Invoke();
        }

        /// <summary> Invokes the StartTimeline (loop start) UnityEvent and C# Action if enabled. </summary>
        public void InvokeOnStartLoop()
        {
            if(enabled) onStartLoop?.Invoke();
            StartLoopAction?.Invoke();
        }

        /// <summary> Invokes the CompleteTimeline (loop end) UnityEvent and C# Action if enabled. </summary>
        public void InvokeOnCompleteLoop()
        {
            if(enabled) onCompleteLoop?.Invoke();
            CompleteLoopAction?.Invoke();
        }

        /// <summary> Invokes the Pause UnityEvent and C# Action if enabled. </summary>
        public void InvokeOnPause()
        {
            if(enabled) onPause?.Invoke();
            PauseAction?.Invoke();
        }

        /// <summary> Invokes the Resume UnityEvent and C# Action if enabled. </summary>
        public void InvokeOnResume()
        {
            if(enabled) onResume?.Invoke();
            ResumeAction?.Invoke();
        }

        /// <summary> Invokes the Stop UnityEvent and C# Action if enabled. </summary>
        public void InvokeOnStop()
        {
            if(enabled) onStop?.Invoke();
            StopAction?.Invoke();
        }
    }
}