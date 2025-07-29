using System.Collections.Generic;
using UnityEngine;

namespace Fusion.Addons.FSM
{
	public abstract class StateBehaviour : StateBehaviour<StateBehaviour>
	{
	}

	public abstract class StateBehaviour<TState> : NetworkBehaviour, IState, IOwnedState<TState> where TState : StateBehaviour<TState>
	{
		// PUBLIC MEMBERS

		public int                  StateId    { get; set; }
		public StateMachine<TState> Machine    { get; set; }
		public virtual string       Name       => gameObject.name;
		public int                  Priority   => _priority;

		//  PRIVATE MEMBERS

		[SerializeField]
		private int _priority = 0;
		[SerializeField]
		private bool _checkPriorityOnExit = true;

		private List<TransitionData<TState>> _transitions;

		// PUBLIC METHODS

		public void AddTransition(TransitionData<TState> transition)
		{
			if (_transitions == null)
			{
				_transitions = new List<TransitionData<TState>>(16);
			}

			_transitions.Add(transition);
		}

		// PROTECTED METHODS

		protected virtual void OnInitialize() {}
		protected virtual void OnDeinitialize(bool hasState) {}

		protected virtual bool CanEnterState() => true;
		protected virtual bool CanExitState(TState nextState) => true;

		protected virtual void OnEnterState() {}
		protected virtual void OnFixedUpdate() {}
		protected virtual void OnExitState() {}

		protected virtual void OnEnterStateRender() {}
		protected virtual void OnRender() {}
		protected virtual void OnExitStateRender() {}

		protected virtual void OnCollectChildStateMachines(List<IStateMachine> stateMachines) {}

		// IState INTERFACE

		void IState.OnFixedUpdate()
		{
			if (_transitions != null)
			{
				for (int i = 0; i < _transitions.Count; i++)
				{
					var transition = _transitions[i];

					if (TryTransition(ref transition) == true)
					{
						Machine.ForceActivateState(transition.TargetState);
						return;
					}
				}
			}

			OnFixedUpdate();
		}

		bool IState.CanExitState(IState nextState, bool isExplicitDeactivation)
		{
			// During explicit deactivation (e.g. when user specifically calls TryDeactivateState) priority is not checked
			if (isExplicitDeactivation == false && _checkPriorityOnExit == true && (nextState as TState).Priority < _priority)
				return false;

			return CanExitState(nextState as TState);
		}

		void IState.Initialize()                => OnInitialize();
		void IState.Deinitialize(bool hasState) => OnDeinitialize(hasState);
		bool IState.CanEnterState()             => CanEnterState();
		void IState.OnEnterState()              => OnEnterState();
		void IState.OnExitState()               => OnExitState();
		void IState.OnEnterStateRender()        => OnEnterStateRender();
		void IState.OnRender()                  => OnRender();
		void IState.OnExitStateRender()         => OnExitStateRender();

		IStateMachine[] IState.ChildMachines { get; set; }
		void IState.CollectChildStateMachines(List<IStateMachine> stateMachines) => OnCollectChildStateMachines(stateMachines);

		// NetworkBehaviour INTERFACE

		public override sealed void FixedUpdateNetwork()
		{
			// Seal method to prevent unwanted usage. OnFixedUpdate should be used instead.
		}

		public override sealed void Render()
		{
			// Seal method to prevent unwanted usage. OnRender should be used instead.
		}

		// PRIVATE METHODS

		private bool TryTransition(ref TransitionData<TState> transition)
		{
			if (transition.Transition(this as TState, transition.TargetState) == false)
				return false;

			if (transition.IsForced == true)
				return true;

			if (CanExitState(transition.TargetState) == false)
				return false;

			if (transition.TargetState.CanEnterState() == false)
				return false;

			return true;
		}
	}
}
