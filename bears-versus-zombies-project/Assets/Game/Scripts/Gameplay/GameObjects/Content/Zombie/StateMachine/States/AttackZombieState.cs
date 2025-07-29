namespace SampleGame.Gameplay.GameObjects
{
    public sealed class AttackZombieState : ZombieStateBase
    {
        private readonly HealthComponent _health;

        public AttackZombieState(ZombieAI zombieAI) : base(zombieAI)
        {
            _health = zombieAI.Health;
        }

        protected override bool CanEnterState()
        {
            return Blackboard.AttackTarget != null && 
                   Blackboard.AttackTarget.CanTakeDamage && 
                   Blackboard.AttackTarget.IsAlive;
        }

        protected override void OnFixedUpdate()
        {
            if (Blackboard.AttackTarget == null)
                return;

            if (Blackboard.AttackTarget.TryTakeDamage(DamageData.External(Blackboard.AttackDamage,
                    Machine.Runner.Tick.Raw)))
            {
                _health.Kill(DamageData.Self(Machine.Runner.Tick.Raw));
            }
        }
    }
}