using Fusion.Addons.FSM;
using SampleGame.Gameplay.Context;

namespace SampleGame.Gameplay.GameObjects
{
    public sealed class IdleZombieState : ZombieStateBase
    {
        private readonly ZombieView _view;
        private readonly IGameCycleState _gameCycleState;

        public IdleZombieState(ZombieAI zombieAI, ZombieView view, IGameCycleState gameCycleState) 
            : base(zombieAI)
        {
            _view = view;
            _gameCycleState = gameCycleState;
        }

        protected override bool CanExitState(IState nextState)
        {
            return _gameCycleState.State != GameState.Finished || nextState is DieZombieState;
        }

        protected override void OnEnterStateRender()
        {
            _view.SwitchAnimatorToIdleState();
            _view.PlayMoaningEffect();
        }
        
        protected override void OnExitStateRender()
        {
            _view.ResetAnimator();
            _view.StopMoaningEffect();
        }
    }
}