using UnityEngine;
using UnityEngine.Events;

namespace OM.Animora.Runtime
{
   /// <summary>
    /// A serializable container for UnityEvents associated with specific lifecycle moments of an AnimoraClip.
    /// This allows designers to hook up custom actions in the Unity Inspector without writing code.
    /// </summary>
    [System.Serializable]
    public class AnimoraClipEvents
    {
        // --- Serialized Fields ---

        /// <summary>
        /// Determines if any of the events within this container should be invoked.
        /// If false, all Invoke methods will do nothing.
        /// </summary>
        [SerializeField]
        private bool isEnabled = false;

        /// <summary>
        /// Event triggered when the associated clip's OnStartPlaying method is called by the AnimoraPlayer.
        /// This happens once when the overall playback sequence begins.
        /// </summary>
        [SerializeField]
        private UnityEvent onStartPlaying;

        /// <summary>
        /// Event triggered when the associated clip's OnCompletePlaying method is called by the AnimoraPlayer.
        /// This happens once when the overall playback sequence finishes (including all loops).
        /// </summary>
        [SerializeField]
        private UnityEvent onCompletePlaying;

        /// <summary>
        /// Event triggered when the associated clip's OnStartLoop method is called by the AnimoraPlayer.
        /// This happens at the beginning of each loop iteration.
        /// </summary>
        [SerializeField]
        private UnityEvent onStartOneLoop; // Note: Field name might be slightly ambiguous, consider renaming to onStartLoop if appropriate

        /// <summary>
        /// Event triggered when the associated clip's OnCompleteLoop method is called by the AnimoraPlayer.
        /// This happens at the end of each loop iteration.
        /// </summary>
        [SerializeField]
        private UnityEvent onCompleteOneLoop; // Note: Field name might be slightly ambiguous, consider renaming to onCompleteLoop if appropriate

        /// <summary>
        /// Event triggered when the associated clip's Enter method is called by the evaluation system.
        /// This happens when the player's time enters the clip's duration.
        /// </summary>
        [SerializeField]
        private UnityEvent onEnter;

        [SerializeField]
        private UnityEvent<float> onUpdate;
        
        /// <summary>
        /// Event triggered when the associated clip's Exit method is called by the evaluation system.
        /// This happens when the player's time leaves the clip's duration.
        /// </summary>
        [SerializeField]
        private UnityEvent onExit;

        // --- Public Methods ---

        /// <summary>
        /// Invokes the 'onStartPlaying' UnityEvent if this event container is enabled.
        /// </summary>
        public void InvokeOnStartPlaying()
        {
            // Only invoke if the container itself is enabled
            if (isEnabled)
            {
                onStartPlaying?.Invoke(); // Safely invoke the UnityEvent
            }
        }

        /// <summary>
        /// Invokes the 'onCompletePlaying' UnityEvent if this event container is enabled.
        /// </summary>
        public void InvokeOnCompletePlaying()
        {
            if (isEnabled)
            {
                onCompletePlaying?.Invoke();
            }
        }

        /// <summary>
        /// Invokes the 'onStartTimeline' (OnStartLoop) UnityEvent if this event container is enabled.
        /// </summary>
        public void InvokeOnStartOneLoop()
        {
            if (isEnabled)
            {
                onStartOneLoop?.Invoke();
            }
        }

        /// <summary>
        /// Invokes the 'onCompleteTimeline' (OnCompleteLoop) UnityEvent if this event container is enabled.
        /// </summary>
        public void InvokeOnCompleteOneLoop()
        {
            if (isEnabled)
            {
                onCompleteOneLoop?.Invoke();
            }
        }

        /// <summary>
        /// Invokes the 'onEnter' UnityEvent if this event container is enabled.
        /// </summary>
        public void InvokeOnEnter()
        {
            if (isEnabled)
            {
                onEnter?.Invoke();
            }
        }

        /// <summary>
        /// Invokes the 'onUpdate' UnityEvent if this event container is enabled.
        /// </summary>
        /// <param name="time"></param>
        public void InvokeOnUpdate(float time)
        {
            if (isEnabled)
            {
                onUpdate?.Invoke(time);
            }
        }

        /// <summary>
        /// Invokes the 'onExit' UnityEvent if this event container is enabled.
        /// </summary>
        public void InvokeOnExit()
        {
            if (isEnabled)
            {
                onExit?.Invoke();
            }
        }

        /// <summary>
        /// Enables or disables this event container.
        /// </summary>
        /// <param name="enable">True to enable, false to disable.</param>
        public void SetEnabled(bool enable)
        {
            isEnabled = enable;
        }

        /// <summary>
        /// Checks if this event container is currently enabled.
        /// </summary>
        /// <returns>True if enabled, false otherwise.</returns>
        public bool IsEnabled()
        {
            return isEnabled;
        }
    }
}