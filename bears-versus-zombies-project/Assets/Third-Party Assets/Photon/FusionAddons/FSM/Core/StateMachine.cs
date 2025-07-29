using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Fusion.Addons.FSM
{
	public partial class StateMachine<TState> : IStateMachine where TState : class, IState
	{
		// PUBLIC MEMBERS

		public string        Name             { get; private set; }
		public NetworkRunner Runner           { get; private set; }

		public bool?         EnableLogging    { get; set; }

		public TState[]      States           => _states;

		public int           ActiveStateId    => _activeStateId;
		public int           PreviousStateId  => _previousStateId;
		public int           DefaultStateId   => _defaultStateId;
		public int           StateChangeTick  => _stateChangeTick;

		public float         StateTime        => _activeStateId >= 0 ? GetStateTime() : 0f;
		public int           StateTicks       => _activeStateId >= 0 ? GetStateTicks() : 0;

		public TState        ActiveState      => _activeStateId >= 0 ? _states[_activeStateId] : null;
		public TState        PreviousState    => _previousStateId >= 0 ? _states[_previousStateId] : null;

		public bool          IsPaused         { get { return _bitState.IsBitSet(0); } set { _bitState = _bitState.SetBitNoRef(0, value); } }

		// PRIVATE MEMBERS

		private readonly TState[] _states;
		private readonly int _stateCount;
		private const int _noneStateId = -1;

		private int _lastRenderStateId = -1;
		private int _lastRenderStateChangeTick;
		private float _interpolationTick;

		private StateMachineController _controller;

		// CONSTRUCTORS

		public StateMachine(string name, params TState[] states)
		{
			Name = name;

			_states = states;
			_stateCount = _states.Length;

			for (int i = 0; i < _states.Length; i++)
			{
				var state = _states[i];

				state.StateId = i;

				if (state is IOwnedState<TState> ownedState)
				{
					ownedState.Machine = this;
				}
			}
		}

		// PUBLIC METHODS

		public bool TryActivateState(int stateId, bool allowReset = false)
		{
			return TryActivateState(stateId, allowReset, false);
		}

		public bool ForceActivateState(int stateId, bool allowReset = false)
		{
			if (stateId == _activeStateId && allowReset == false)
				return false;

			ChangeState(stateId);
			return true;
		}

		public bool TryDeactivateState(int stateId)
		{
			if (stateId != _activeStateId)
				return true;

			return TryActivateState(_defaultStateId, false, true);
		}

		public bool ForceDeactivateState(int stateId)
		{
			if (stateId != _activeStateId)
				return true;

			return ForceActivateState(_defaultStateId);
		}

		public bool TryToggleState(int stateId, bool value)
		{
			bool stateIsActive = stateId == _activeStateId;

			if (stateIsActive == value)
				return true;

			int targetState = value == true ? stateId : _defaultStateId;
			return TryActivateState(targetState, false, value == false);
		}

		public void ForceToggleState(int stateId, bool value)
		{
			bool stateIsActive = stateId == _activeStateId;

			if (stateIsActive == value)
				return;

			int targetState = value == true ? stateId : _defaultStateId;
			ForceActivateState(targetState, false);
		}

		public bool HasState(TState state)
		{
			for (int i = 0; i < _stateCount; i++)
			{
				if (_states[i].StateId == state.StateId && _states[i] == state)
					return true;
			}

			return false;
		}

		public TState GetState(int stateId)
		{
			if (stateId < 0 || stateId >= _stateCount)
				return default;

			return _states[stateId];
		}

		public T GetState<T>() where T : TState
		{
			for (int i = 0; i < _stateCount; i++)
			{
				if (_states[i] is T state)
					return state;
			}

			return default;
		}

		public void SetDefaultState(int stateId)
		{
			if (stateId < 0 || stateId >= _stateCount)
			{
				Debug.LogError($"SetDefaultState: Invalid state Id {stateId}");
				return;
			}

			_defaultStateId = stateId;
		}

		public void Reset()
		{
			_activeStateId = _noneStateId;
			_previousStateId = _noneStateId;
			_stateChangeTick = 0;
			_bitState = 0;

			_lastRenderStateId = _noneStateId;

			LogReset();

			if (_hasChildMachines == true)
			{
				for (int i = 0; i < _stateCount; i++)
				{
					var state = _states[i];

					for (int j = 0; j < state.ChildMachines.Length; j++)
					{
						state.ChildMachines[j].Reset();
					}
				}
			}
		}

		// IStateMachine INTERFACE

		void IStateMachine.Initialize(StateMachineController controller, NetworkRunner runner)
		{
			Runner = runner;

			_controller = controller;

			for (int i = 0; i < _stateCount; i++)
			{
				var state = _states[i];

				for (int j = 0; j < state.ChildMachines.Length; j++)
				{
					state.ChildMachines[j].Initialize(controller, runner);
				}

				state.Initialize();
			}
		}

		void IStateMachine.FixedUpdateNetwork()
		{
			if (IsPaused == true)
				return;

			if (_activeStateId < 0)
			{
				ChangeState(_defaultStateId);
			}

			int updateStateId = _activeStateId;

			ActiveState.OnFixedUpdate();

			// Active state could be changed in state's fixed update
			// Do not update its child machines in that case
			if (updateStateId == _activeStateId)
			{
				for (int i = 0; i < ActiveState.ChildMachines.Length; i++)
				{
					ActiveState.ChildMachines[i].FixedUpdateNetwork();
				}
			}
		}

		void IStateMachine.Render()
		{
			if (IsPaused == true)
				return;

			if (_lastRenderStateId != _activeStateId || _lastRenderStateChangeTick != _stateChangeTick)
			{
				LogRenderStateChange();

				if (_lastRenderStateId >= 0)
				{
					var lastRenderState = _states[_lastRenderStateId];

					lastRenderState.OnExitStateRender();

					// When transitioning to a new state make sure Render is called one last time on all child machines
					// - this will properly call OnExitStateRender callbacks and save render state change
					if (_lastRenderStateId != _activeStateId)
					{
						for (int i = 0; i < lastRenderState.ChildMachines.Length; i++)
						{
							lastRenderState.ChildMachines[i].Render();
						}
					}
				}

				if (_activeStateId >= 0)
				{
					ActiveState.OnEnterStateRender();
				}

				_lastRenderStateId = _activeStateId;
				_lastRenderStateChangeTick = _stateChangeTick;
			}

			if (_activeStateId < 0)
				return;

			ActiveState.OnRender();

			for (int i = 0; i < ActiveState.ChildMachines.Length; i++)
			{
				ActiveState.ChildMachines[i].Render();
			}
		}

		void IStateMachine.Deinitialize(bool hasState)
		{
			for (int i = 0; i < _stateCount; i++)
			{
				var state = _states[i];

				state.Deinitialize(hasState);

				for (int j = 0; j < state.ChildMachines.Length; j++)
				{
					state.ChildMachines[j].Deinitialize(hasState);
				}
			}

			Runner = null;
		}

		// PRIVATE METHODS

		private bool TryActivateState(int stateId, bool allowReset, bool isExplicitDeactivation)
		{
			if (stateId == _activeStateId && allowReset == false)
				return false;

			var nextState = _states[stateId];

			if (ActiveState != null && ActiveState.CanExitState(nextState, isExplicitDeactivation) == false)
				return false;

			if (nextState.CanEnterState() == false)
				return false;

			ChangeState(stateId);
			return true;
		}

		private void ChangeState(int stateId)
		{
			if (stateId >= _stateCount)
			{
				throw new InvalidOperationException($"State with ID {stateId} not present in the state machine {Name}");
			}

			Assert.Check(Runner.Stage != default, "State changes are not allowed from Render calls");

			_previousStateId = _activeStateId;
			_activeStateId = stateId;

			LogStateChange();

			if (_previousStateId >= 0)
			{
				PreviousState.OnExitState();

				for (int i = 0; i < PreviousState.ChildMachines.Length; i++)
				{
					// When parent state is deactivated, all child states are deactivated as well
					PreviousState.ChildMachines[i].ForceActivateState(_noneStateId);
				}
			}

			_stateChangeTick = Runner.Tick;

			if (_activeStateId >= 0)
			{
				ActiveState.OnEnterState();

				for (int i = 0; i < ActiveState.ChildMachines.Length; i++)
				{
					var childMachine = ActiveState.ChildMachines[i];

					if (childMachine.ActiveState == null && childMachine.PreviousState != null)
					{
						// When parent state is activated, all child states are re-activated as well
						childMachine.ForceActivateState(childMachine.PreviousState.StateId);
					}
				}
			}
		}

		private int GetStateTicks()
		{
			int currentTick = Runner.Stage == default && _interpolationTick != 0f ? (int)_interpolationTick : Runner.Tick;
			return currentTick - StateChangeTick;
		}

		private float GetStateTime()
		{
			if (Runner.Stage != default || _interpolationTick == 0f)
				return (Runner.Tick - StateChangeTick) * Runner.DeltaTime;

			return (_interpolationTick - StateChangeTick) * Runner.DeltaTime;
		}

		// LOGGING

		[Conditional("DEBUG")]
		private void LogStateChange()
		{
			if (EnableLogging.HasValue == false && _controller.EnableLogging == false)
				return; // Global controller logging is disabled

			if (EnableLogging.HasValue == true && EnableLogging.Value == false)
				return; // Logging is specifically disabled for this machine

			var activeStateName = ActiveState != null ? ActiveState.Name : "None";
			var previousStateName = PreviousState != null ? PreviousState.Name : "None";
			Debug.Log($"{_controller.gameObject.name} - <color=#F04C4C>State Machine <b>{Name}</b>: Change State to <b>{activeStateName}</b></color> - Previous: {previousStateName}, Tick: {Runner.Tick.Raw}", _controller);
		}

		[Conditional("DEBUG")]
		private void LogReset()
		{
			if (Runner == null)
				return;

			if (EnableLogging.HasValue == false && _controller.EnableLogging == false)
				return; // Global controller logging is disabled

			if (EnableLogging.HasValue == true && EnableLogging.Value == false)
				return; // Logging is specifically disabled for this machine

			Debug.Log($"{_controller.gameObject.name} - <color=#F04C4C>State Machine <b>{Name}</b>: Machine <b>RESET</b></color> - Tick: {Runner.Tick.Raw}", _controller);
		}

		[Conditional("DEBUG")]
		private void LogRenderStateChange()
		{
			if (EnableLogging.HasValue == false && _controller.EnableLogging == false)
				return; // Global controller logging is disabled

			if (EnableLogging.HasValue == true && EnableLogging.Value == false)
				return; // Logging is specifically disabled for this machine

			var activeStateName = ActiveState != null ? ActiveState.Name : "None";
			var previousStateName = _lastRenderStateId >= 0 ? _states[_lastRenderStateId].Name : "None";
			Debug.Log($"{_controller.gameObject.name} - <color=#467DE7>State Machine <b>{Name}</b>: Change RENDER State to <b>{activeStateName}</b></color> - Previous: {previousStateName}, StateChangeTick: {_stateChangeTick}, RenderFrame: {Time.frameCount}", _controller);
		}
	}
}
