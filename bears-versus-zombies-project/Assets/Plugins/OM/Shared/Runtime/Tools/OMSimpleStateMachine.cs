using System;

namespace OM
{
    /// <summary>
    /// a simple state machine that can be used to manage states in a generic way.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OMSimpleStateMachine<T>
    {
        public event Action<T, T> OnStateChanged; 
        
        /// <summary>
        /// Last state before the current state.
        /// </summary>
        public T LastState { get; private set; }
        /// <summary>
        /// Current state of the state machine.
        /// </summary>
        public T CurrentState { get; private set; }
        
        /// <summary>
        /// Constructor for the state machine.
        /// </summary>
        /// <param name="initialState"></param>
        /// <param name="onStateChanged"></param>
        public OMSimpleStateMachine(T initialState, Action<T, T> onStateChanged = null)
        {
            if (onStateChanged != null)
                OnStateChanged += onStateChanged;

            SetNewState(initialState, force: true);
        }

        /// <summary>
        /// Sets a new state for the state machine.
        /// </summary>
        /// <param name="newState"></param>
        /// <param name="force"></param>
        public void SetNewState(T newState, bool force = false)
        {
            if (!force && CurrentState != null && CurrentState.Equals(newState)) return;

            LastState = CurrentState;
            CurrentState = newState;
            OnStateChanged?.Invoke(CurrentState, LastState);
        }

        /// <summary>
        /// Subscribes to the state changed event.
        /// </summary>
        /// <param name="callback"></param>
        public void OnChange(Action<T, T> callback)
        {
            OnStateChanged += callback;
        }
    }
}