using UnityEngine;

namespace SampleGame.Gameplay.GameObjects
{
    public sealed class MoveZombieState : ZombieStateBase
    {
        private readonly ZombieView _view;
        private readonly PathFinderMoveComponent _moveComponent;
        private Vector3 _lastTargetPosition;

        public MoveZombieState(ZombieAI zombieAI, ZombieView view)
            : base(zombieAI)
        {
            _view = view;
            _moveComponent = zombieAI.MoveComponent;
        }

        protected override bool CanEnterState() => IsExistMoveTarget();

        protected override void OnEnterState()
        {
            _moveComponent.StartAgent();
        }
        
        protected override void OnEnterStateRender()
        {
            _view.SwitchAnimatorToMoveState();
            _view.PlayMoaningEffect();
        }
        
        protected override void OnFixedUpdate()
        {
            if (IsExistMoveTarget() == false)
                return;
            
            _moveComponent.Move(GetTargetPosition());
        }
        
        protected override void OnExitState()
        {
            _moveComponent.StopAgent();
        }

        protected override void OnExitStateRender()
        {
            _view.ResetAnimator();
            _view.StopMoaningEffect();
        }

        private bool IsExistMoveTarget()
        {
            return Blackboard.MoveTarget != null;
        }

        private Vector3 GetTargetPosition()
        {
            return Blackboard.MoveTarget.position;
        }
    }
}