using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;

namespace Fusion.Addons.FSM
{
	public interface IStateMachineOwner
	{
		void CollectStateMachines(List<IStateMachine> stateMachines);
	}

	[DisallowMultipleComponent]
	public sealed class StateMachineController : NetworkBehaviour, IBeforeAllTicks, IAfterTick
	{
		// PUBLIC MEMBERS

		public bool                EnableLogging   { get { return _enableLogging; } set { _enableLogging = value; } }
		public List<IStateMachine> StateMachines   => _stateMachines;

		// PRIVATE MEMBERS

		[Header("DEBUG")]
		[SerializeField]
		private bool _enableLogging = false;

		private List<IStateMachine> _stateMachines = new(32);
		private List<IState> _statePool;

		private bool _stateMachinesCollected;
		private bool _manualUpdate;

		// PUBLIC METHODS

		public void SetManualUpdate(bool manualUpdate)
		{
			_manualUpdate = manualUpdate;
		}

		public void ManualFixedUpdate()
		{
			if (_manualUpdate == false)
				throw new InvalidOperationException("Manual update is not turned on");

			if (Runner.Stage == default)
				throw new InvalidOperationException("ManualFixedUpdate needs to be called from simulation (from FixedUpdateNetwork call)");

			FixedUpdateInternal();
		}

		public void ManualRender()
		{
			if (_manualUpdate == false)
				throw new InvalidOperationException("Manual update is not turned on");

			if (Runner.Stage != default)
				throw new InvalidOperationException("ManualRender needs to be called outside of simulation (from Render call)");

			RenderInternal();
		}

		// NetworkBehaviour INTERFACE

		public override int? DynamicWordCount => GetNetworkDataWordCount();

		public override void Spawned()
		{
			for (int i = 0; i < _stateMachines.Count; i++)
			{
				_stateMachines[i].Reset();
			}

			if (HasStateAuthority == false)
			{
				ReadNetworkData();
			}

			for (int i = 0; i < _stateMachines.Count; i++)
			{
				_stateMachines[i].Initialize(this, Runner);
			}

			if (HasStateAuthority == true)
			{
				WriteNetworkData();
			}
		}

		public override void Render()
		{
			if (_manualUpdate == true)
				return;

			RenderInternal();
		}

		public override void FixedUpdateNetwork()
		{
			if (_manualUpdate == true)
				return;

			// Some advanced components (e.g. KCC) need to set the whole object as simulated
			// in order to receive all Fusion callbacks. That turns on proxy FUN ticking on all behaviours
			// which is usually undesirable. For now we just don't allow proxy prediction here.
			if (IsProxy == true)
				return;

			FixedUpdateInternal();
		}

		public override void Despawned(NetworkRunner runner, bool hasState)
		{
			if (hasState == false)
				return;

			for (int i = 0; i < _stateMachines.Count; i++)
			{
				_stateMachines[i].Deinitialize(hasState);
			}
		}

		// IBeforeAllTicks INTERFACE

		void IBeforeAllTicks.BeforeAllTicks(bool resimulation, int tickCount)
		{
			// Read before all ticks as state machine properties are used both for Render and FUN.
			// IAfterClientPredictionReset would not be enough.
			ReadNetworkData();
		}

		// IAfterTick INTERFACE

		void IAfterTick.AfterTick()
		{
			WriteNetworkData();
		}

		// PRIVATE METHODS

		private void FixedUpdateInternal()
		{
			for (int i = 0; i < _stateMachines.Count; i++)
			{
				Profiler.BeginSample($"StateMachineController.FixedUpdate ({_stateMachines[i].Name})");
				_stateMachines[i].FixedUpdateNetwork();
				Profiler.EndSample();
			}
		}

		private void RenderInternal()
		{
			if (Interpolate() == false)
				return; // Wait for interpolation data before starting rendering

			for (int i = 0; i < _stateMachines.Count; i++)
			{
				Profiler.BeginSample($"StateMachineController.Render ({_stateMachines[i].Name})");
				_stateMachines[i].Render();
				Profiler.EndSample();
			}
		}

		private void CollectStateMachines()
		{
			_stateMachines.Clear();

			var owners = GetComponentsInChildren<IStateMachineOwner>(true);
			var tempMachines = ListPool.Get<IStateMachine>(32);

			for (int i = 0; i < owners.Length; i++)
			{
				owners[i].CollectStateMachines(tempMachines);

				CheckCollectedMachines(owners[i], tempMachines);

				for (int j = 0; j < tempMachines.Count; j++)
				{
					var stateMachine = tempMachines[j];

					Assert.Check(_stateMachines.Contains(stateMachine) == false, $"Trying to add already collected state machine for second time {stateMachine.Name}");
					CheckDuplicateStates(stateMachine.Name, stateMachine.States);

					_stateMachines.Add(stateMachine);
				}

				tempMachines.Clear();
			}

			_stateMachinesCollected = true;

			ListPool.Return(tempMachines);
		}

		private int GetNetworkDataWordCount()
		{
			if (_stateMachinesCollected == false)
			{
				CollectStateMachines();
			}

			int wordCount = 0;

			for (int i = 0; i < _stateMachines.Count; i++)
			{
				wordCount += _stateMachines[i].WordCount;
			}

			return wordCount;
		}

		private unsafe void ReadNetworkData()
		{
			fixed (int* statePtr = &ReinterpretState<int>())
			{
				int* ptr = statePtr;

				for (int i = 0; i < _stateMachines.Count; i++)
				{
					var stateMachine = _stateMachines[i];

					stateMachine.Read(ptr);
					ptr += stateMachine.WordCount;
				}
			}
		}

		private unsafe void WriteNetworkData()
		{
			fixed (int* statePtr = &ReinterpretState<int>())
			{
				int* ptr = statePtr;

				for (int i = 0; i < _stateMachines.Count; i++)
				{
					var stateMachine = _stateMachines[i];

					stateMachine.Write(ptr);
					ptr += stateMachine.WordCount;
				}
			}
		}

		private unsafe bool Interpolate()
		{
			if (GetInterpolationData(out InterpolationData interpolationData) == false)
				return false;

			for (int i = 0; i < _stateMachines.Count; i++)
			{
				var stateMachine = _stateMachines[i];

				stateMachine.Interpolate(interpolationData);

				interpolationData.From += stateMachine.WordCount;
				interpolationData.To   += stateMachine.WordCount;
			}

			return true;
		}

		private unsafe bool GetInterpolationData(out InterpolationData data)
		{
			bool buffersValid = TryGetSnapshotsBuffers(out NetworkBehaviourBuffer fromBuffer, out NetworkBehaviourBuffer toBuffer, out float alpha);

			data = new InterpolationData
			{
				FromBuffer = fromBuffer,
				ToBuffer = toBuffer,
				Alpha = alpha,
			};

			return buffersValid;
		}

		// DEBUG

		[Conditional("DEBUG")]
		private void CheckCollectedMachines(IStateMachineOwner owner, List<IStateMachine> machines)
		{
			if (machines.Count == 0)
			{
				var ownerObject = (owner as Component).gameObject;
				Debug.LogWarning($"No state machines collected from the state machine owner {ownerObject.name}", ownerObject);
			}
		}

		[Conditional("DEBUG")]
		private void CheckDuplicateStates(string stateMachineName, IState[] states)
		{
			if (_statePool == null)
			{
				_statePool = new List<IState>(128);
				_statePool.AddRange(states);
				return;
			}

			for (int i = 0; i < states.Length; i++)
			{
				var state = states[i];

				if (_statePool.Contains(state) == true)
				{
					throw new InvalidOperationException($"State {state.Name} is used for multiple state machines, this is not allowed! State Machine: {stateMachineName}");
				}
			}

			_statePool.AddRange(states);
		}
	}
}
