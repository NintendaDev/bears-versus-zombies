using Fusion.Addons.FSM;

namespace SampleGame.Gameplay.GameObjects
{
    public sealed class TakeDamageZombieState : ZombieStateBase
    {
        private readonly HealthComponent _health;
        private readonly ZombieView _view;

        public TakeDamageZombieState(ZombieAI zombieAI, HealthComponent health, ZombieView view) 
            : base(zombieAI)
        {
            _health = health;
            _view = view;
        }

        protected override bool CanExitState(IState nextState)
        {
            return nextState.StateId == StateId || 
                   _health.IsAlive == false || 
                   Blackboard.HitStunSeconds < Machine.StateTime;
        }

        protected override void OnEnterStateRender()
        {
            _view.OnTakeDamage();
        }
    }
}