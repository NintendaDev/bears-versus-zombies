namespace OM
{
    /// <summary>
    /// Represents the current play state.
    /// </summary>
    public enum OM_PlayState
    {
        Playing = 0,
        Paused = 1,
        Stopped = 2,
    }

    /// <summary>
    /// A controller for managing and switching between play states.
    /// </summary>
    public class OM_PlayStateController
    {
        /// <summary>
        /// Invoked whenever the play state changes. 
        /// First param is the new state, second is the old state.
        /// </summary>
        public event System.Action<OM_PlayState, OM_PlayState> OnPlayStateChanged;

        /// <summary>
        /// Gets the current play state.
        /// </summary>
        public OM_PlayState State { get; private set; } = OM_PlayState.Stopped;

        /// <summary>
        /// Returns true if the current state is Playing.
        /// </summary>
        public bool IsPlaying()
        {
            return State == OM_PlayState.Playing;
        }

        /// <summary>
        /// Returns true if the current state is Paused.
        /// </summary>
        public bool IsPaused()
        {
            return State == OM_PlayState.Paused;
        }

        /// <summary>
        /// Sets the state to Playing if it's not already playing.
        /// </summary>
        public void Play()
        {
            if (IsPlaying()) return;
            SetState(OM_PlayState.Playing);
        }

        /// <summary>
        /// Sets the state to Paused.
        /// </summary>
        public void Pause()
        {
            SetState(OM_PlayState.Paused);
        }

        /// <summary>
        /// Sets the state to Stopped.
        /// </summary>
        public void Stop()
        {
            SetState(OM_PlayState.Stopped);
        }

        /// <summary>
        /// Changes the state and invokes the state changed event if the state has changed.
        /// </summary>
        /// <param name="newState">The new state to apply.</param>
        private void SetState(OM_PlayState newState)
        {
            if (State == newState) return;
            var oldState = State;
            State = newState;
            OnPlayStateChanged?.Invoke(newState, oldState);
        }
    }
}
