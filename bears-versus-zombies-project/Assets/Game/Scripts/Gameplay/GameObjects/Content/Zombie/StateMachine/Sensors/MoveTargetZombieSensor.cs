using Fusion;

namespace SampleGame.Gameplay.GameObjects
{
    public sealed class MoveTargetZombieSensor : ZombieSensorBase
    {
        private HealthComponent _targetHealth;

        public override void Spawned()
        {
            Bus bus = FindObjectOfType<Bus>();

            if (bus == null)
                return;
            
            Blackboard.MoveTarget = bus.GetTarget();
            _targetHealth = bus.GetComponent<HealthComponent>();

            if (_targetHealth != null)
                _targetHealth.Die += OnTargetDie;
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (_targetHealth != null)
                _targetHealth.Die -= OnTargetDie;
        }

        private void OnTargetDie()
        {
            Blackboard.MoveTarget = null;
        }
    }
}