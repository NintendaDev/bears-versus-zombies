using System.Collections.Generic;
using Fusion;
using Fusion.Addons.FSM;
using Modules.Services;
using SampleGame.Gameplay.GameContext;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.GameObjects
{
    public sealed class ZombieAI : NetworkBehaviour, IStateMachineOwner
    {
        [SerializeField, Required] private ZombieView _view;
        [SerializeField, Required, ChildGameObjectsOnly] private HealthComponent _health;
        [SerializeField, Required, ChildGameObjectsOnly] private PathFinderMoveComponent _moveComponent;
        [SerializeField, Required, ChildGameObjectsOnly] private ZombieBlackboard _blackboard;

        private readonly int _damageTickShift = 1;
        private StateMachine<ZombieStateBase> _stateMachine;
        private IGameCycleState _gameCycleState;
        private bool _isInitializedDefaultState;

        internal ZombieBlackboard Blackboard => _blackboard;
        
        internal HealthComponent Health => _health;
        
        internal PathFinderMoveComponent MoveComponent => _moveComponent;
        
        internal Transform Transform { get; private set; }

        private void Awake()
        {
            _gameCycleState = ServiceLocator.Instance.Get<IGameCycleState>();
        }

        public override void Spawned()
        {
            Transform = transform;
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            _blackboard.Reset();
        }

        public override void FixedUpdateNetwork()
        {
            if (_isInitializedDefaultState == false)
            {
                _stateMachine.ForceActivateState<IdleZombieState>();
                _isInitializedDefaultState = true;
            }
        }

        void IStateMachineOwner.CollectStateMachines(List<IStateMachine> stateMachines)
        {
            IdleZombieState idleZombieState = new(zombieAI: this, _view, _gameCycleState);
            MoveZombieState moveZombieState = new(zombieAI: this, _view);
            TakeDamageZombieState takeDamageZombieState = new(zombieAI: this, _health, _view);
            AttackZombieState attackZombieState = new(zombieAI: this);
            DieZombieState dieZombieState = new(zombieAI: this, _view);

            idleZombieState.AddTransition(moveZombieState, () => _blackboard.MoveTarget != null);
            idleZombieState.AddTransition(attackZombieState, () => _blackboard.AttackTarget != null);
            idleZombieState.AddTransition(dieZombieState, () => _health.IsAlive == false);
            idleZombieState.AddTransition(takeDamageZombieState, IsTakeDamage);
            
            moveZombieState.AddTransition(idleZombieState, () => _blackboard.MoveTarget == null || 
                                                                 _gameCycleState.IsFinished);
            
            moveZombieState.AddTransition(attackZombieState, () => _blackboard.AttackTarget != null);
            moveZombieState.AddTransition(dieZombieState, () => _health.IsAlive == false);
            moveZombieState.AddTransition(takeDamageZombieState, IsTakeDamage);
            
            attackZombieState.AddTransition(dieZombieState, () => _health.IsAlive == false);
            
            takeDamageZombieState.AddTransition(idleZombieState, () => _blackboard.MoveTarget == null || 
                                                                       _gameCycleState.IsFinished);
            
            takeDamageZombieState.AddTransition(moveZombieState, () => _blackboard.MoveTarget != null);
            takeDamageZombieState.AddTransition(attackZombieState, () => _blackboard.AttackTarget != null,
                isForced: true);
            
            takeDamageZombieState.AddTransition(dieZombieState, () => _health.IsAlive == false);
            
            takeDamageZombieState.AddTransition(takeDamageZombieState, () =>
            {
                return IsTakeDamage() && _health.IsAlive;
            });
            
            _stateMachine = new StateMachine<ZombieStateBase>(nameof(ZombieAI), 
                idleZombieState, 
                moveZombieState,
                takeDamageZombieState,
                dieZombieState,
                attackZombieState);
            
            stateMachines.Add(_stateMachine);
        }

        private bool IsTakeDamage()
        {
            return _health.LastDamageData.Tick + _damageTickShift == Runner.Tick.Raw;
        }
    }
}