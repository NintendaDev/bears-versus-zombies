namespace Fusion.Addons.FSM
{
	public unsafe interface IOwnedState<TState> where TState : class, IState
	{
		public StateMachine<TState> Machine { get; set; }

		public void AddTransition(TransitionData<TState> transition);
	}

	// Transitions

	public delegate bool Transition<in TState>(TState from, TState to) where TState : IState;

	public struct  TransitionData<TState> where TState : IState
	{
		public readonly TState             TargetState;
		public readonly Transition<TState> Transition;
		public readonly bool               IsForced;

		public TransitionData(TState targetState, Transition<TState> transition, bool isForced = false)
		{
			TargetState = targetState;
			Transition = transition;
			IsForced = isForced;
		}
	}
}
