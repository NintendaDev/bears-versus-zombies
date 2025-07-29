using Fusion;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.GameObjects
{
    public sealed class LifetimeComponent : NetworkBehaviour
    {
        [SerializeField, Required] private HealthComponent _health;
        [SerializeField, MinValue(0)] private float _killDelaySeconds = 15f;
        
        private readonly ReactiveProperty<float> _currentTimer = new();

        public float KillDelaySeconds => _killDelaySeconds;
        
        public ReadOnlyReactiveProperty<float> CurrentTimer => _currentTimer;
        
        [Networked]
        private TickTimer KillTimer { get; set; }

        public override void Spawned()
        {
            KillTimer = TickTimer.CreateFromSeconds(Runner, _killDelaySeconds);
        }

        public override void FixedUpdateNetwork()
        {
            if (KillTimer.ExpiredOrNotRunning(Runner) == false)
                return;
            
            _health.Kill(DamageData.Self(Runner.Tick.Raw));
        }

        public override void Render()
        {
            _currentTimer.Value = KillTimer.RemainingTime(Runner) ?? 0;
        }
    }
}