using Fusion;
using Fusion.Addons.FSM;

namespace SampleGame.Gameplay.GameObjects
{
    public sealed class DieZombieState : ZombieStateBase
    {
        private readonly ZombieView _view;
        private readonly NetworkObject _networkObject;
        private bool _isDisabled;

        public DieZombieState(ZombieAI zombieAI, ZombieView view) 
            : base(zombieAI)
        {
            _view = view;
            _networkObject = zombieAI.Object;
        }

        protected override bool CanExitState(IState nextState)
        {
            return false;
        }

        protected override void OnEnterState()
        {
            _isDisabled = false;
        }

        protected override void OnFixedUpdate()
        {
            if (_isDisabled == false && Blackboard.DespawnDelay < Machine.StateTime)
            {
                Machine.Runner.Despawn(_networkObject);
                _isDisabled = true;
            }
        }

        protected override void OnEnterStateRender()
        {
            _view.OnDie();
        }
    }
}