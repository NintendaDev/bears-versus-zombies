using System;
using UnityEngine;

namespace OM
{
    /// <summary>
    /// A simple ticker class that triggers an action at specified intervals.
    /// </summary>
    public class OMTicker : IOMUpdater
    {
        /// <summary>
        /// Creates a new OMTicker instance.
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="onTick"></param>
        /// <param name="timeIndependent"></param>
        /// <param name="persist"></param>
        /// <returns></returns>
        public static OMTicker Create(
            float interval,
            Action<int> onTick,
            bool timeIndependent = false,
            bool persist = false)
        {
            var ticker = new OMTicker(interval,onTick,timeIndependent,persist);
            return ticker;
        }

        private readonly Action<int> _onTick;
        private readonly float _interval;
        private readonly bool _persist;
        private readonly bool _timeIndependent;

        private int _tickCount;
        private float _timer;
        public bool IsRunning { get; private set; } = true;

        /// <summary>
        /// Constructor for the OMTicker class.
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="onTick"></param>
        /// <param name="timeIndependent"></param>
        /// <param name="persist"></param>
        private OMTicker(float interval, Action<int> onTick,bool timeIndependent,bool persist = false)
        {
            _interval = interval;
            _timeIndependent = timeIndependent;
            _onTick = onTick;
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
        /// Checks if the ticker is set to persist across scenes.
        /// </summary>
        /// <returns></returns>
        public bool IsDontDestroyOnLoad()
        {
            return _persist;
        }

        /// <summary>
        /// Checks if the ticker is time-independent.
        /// </summary>
        /// <returns></returns>
        public bool IsUpdaterCompleted()
        {
            return !IsRunning;
        }

        /// <summary>
        /// Updates the ticker and invokes the tick action if the interval has passed.
        /// </summary>
        public void OnUpdate()
        {
            _timer += GetDeltaTime();
            
            if(_timer >= _interval)
            {
                _timer -= _interval;
                _tickCount++;
                _onTick?.Invoke(_tickCount);
            }
        }

        /// <summary>
        /// Stops the ticker from running.
        /// </summary>
        public void Stop()
        {
            IsRunning = false;
        }
    }
}