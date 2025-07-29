using System;
using UnityEngine;

namespace OM
{
    /// <summary>
    /// A simple timer class that triggers an action after a specified duration.
    /// </summary>
    public class OMTimer : IOMUpdater
    {
        /// <summary>
        /// Creates a new OMTimer instance.
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="onComplete"></param>
        /// <param name="timeIndependent"></param>
        /// <param name="persist"></param>
        /// <returns></returns>
        public static OMTimer Create(float duration,Action onComplete,bool timeIndependent = false,bool persist = false)
        {
            return new OMTimer(duration,onComplete,timeIndependent,persist);
        }

        private float _duration;
        private readonly bool _persist;
        private readonly Action _onComplete;
        private readonly bool _timeIndependent = false;

        /// <summary>
        /// Constructor for the OMTimer class.
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="onComplete"></param>
        /// <param name="timeIndependent"></param>
        /// <param name="persist"></param>
        private OMTimer(float duration,
            Action onComplete,
            bool timeIndependent,
            bool persist)
        {
            _duration = duration;
            _timeIndependent = timeIndependent;
            _onComplete = onComplete;
            _persist = persist;
            OMUpdaterRuntime.AddUpdater(this);
        }

        /// <summary>
        /// Gets the delta time based on the time-independent setting.
        /// </summary>
        /// <returns></returns>
        private float GetDeltaTime()
        {
            return _timeIndependent ? Time.unscaledDeltaTime : Time.deltaTime;
        }
        
        /// <summary>
        /// Checks if the timer is set to persist across scenes.
        /// </summary>
        /// <returns></returns>
        public bool IsDontDestroyOnLoad()
        {
            return _persist;
        }

        /// <summary>
        /// Checks if the timer has completed its duration.
        /// </summary>
        /// <returns></returns>
        public bool IsUpdaterCompleted()
        {
            return _duration <= 0;
        }

        /// <summary>
        /// Updates the timer by subtracting the delta time from the duration.
        /// </summary>
        public void OnUpdate()
        {
            if(IsUpdaterCompleted()) return;
            
            _duration -= GetDeltaTime();
            if (_duration <= 0)
            {
                _onComplete?.Invoke();
                OMUpdaterRuntime.RemoveUpdater(this);
            }
        }

        /// <summary>
        /// Stops the timer and resets the duration to zero.
        /// </summary>
        public void Stop()
        {
            _duration = 0;
        }
    }
}