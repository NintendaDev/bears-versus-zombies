using System;

namespace OM
{
    /// <summary>
    /// Interface for a state in a state machine.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IOMState<in T>
    {
        /// <summary>
        /// called when the state is about to be entered from another state to check if the state can be entered.
        /// </summary>
        /// <param name="fromState"></param>
        /// <returns></returns>
        bool CanEnterState(T fromState) => true;

        /// <summary>
        /// called when the state is entered from another state.
        /// </summary>
        /// <param name="fromState"></param>
        void OnEnterState(T fromState);

        /// <summary>
        /// called when the state is about to be exited to check if the state can be exited.
        /// </summary>
        /// <param name="toState"></param>
        /// <returns></returns>
        bool CanExitState(T toState) => true;
        
        /// <summary>
        /// called when the state is exited to another state.
        /// </summary>
        /// <param name="toState"></param>
        void OnExitState(T toState);
        
        /// <summary>
        /// called every frame to update the state if the state is active.
        /// </summary>
        void OnUpdateState() { }
    }
    
    /// <summary>
    /// A state machine that can be used to manage states in a generic way.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OMStateMachine<T> where T : IOMState<T>
    {
        /// <summary>
        /// event that is called when the state is changed.
        /// </summary>
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
        /// <param name="initState"></param>
        /// <param name="onStateChanged"></param>
        public OMStateMachine(T initState,Action<T,T> onStateChanged = null)
        {
            SetNewState(initState);
            if(onStateChanged != null) OnStateChanged += onStateChanged;
        }

        /// <summary>
        /// Sets a new state for the state machine and invokes the state changed event if the state has changed.
        /// </summary>
        /// <param name="newState"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public bool SetNewState(T newState,bool force = false)
        {
            if(newState == null) return false;
            if (!force)
            {
                if (!newState.CanEnterState(fromState:CurrentState)) return false;
                if (CurrentState != null && !CurrentState.CanExitState(toState:newState)) return false;
            }
            CurrentState?.OnExitState(toState:newState);
            LastState = CurrentState;
            CurrentState = newState;
            CurrentState?.OnEnterState(fromState:LastState);
            OnStateChanged?.Invoke(CurrentState,LastState);
            return true;
        }

        /// <summary>
        /// Updates the current state by calling the OnUpdateState method of the current state.
        /// </summary>
        public void UpdateStateMachine()
        {
            CurrentState?.OnUpdateState();
        }
        
        /// <summary>
        /// Subscribes to the state changed event.
        /// </summary>
        /// <param name="callback"></param>
        public void OnChange(Action<T,T> callback)
        {
            OnStateChanged += callback;
        }
    }
}