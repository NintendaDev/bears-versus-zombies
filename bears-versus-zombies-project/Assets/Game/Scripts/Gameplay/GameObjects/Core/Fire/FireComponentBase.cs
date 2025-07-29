using System;
using Fusion;
using Modules.Conditions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.GameObjects
{
    public abstract class FireComponent : NetworkBehaviour
    {
        [SerializeField, MinValue(0)] private int _damage = 1;
        [SerializeField, MinValue(0)] private float _delaySeconds;
        [SerializeField, MinValue(0)] private float _cooldownSeconds = 1f;
        
        private readonly AndCondition _condition = new();
        private HealthComponent _target;

        [Networked, UnityNonSerialized]
        [OnChangedRender(nameof(OnFireCountRender))]
        private int FireCount { get; set; }
        
        [Networked]
        private TickTimer Cooldown { get; set; }
        
        [Networked]
        private TickTimer Delay { get; set; }
        
        [Networked]
        private NetworkBool HasFireRequest { get; set; }

        public event Action Fired;

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            _condition.Dispose();
        }

        public override void FixedUpdateNetwork()
        {
            if (HasFireRequest && Delay.ExpiredOrNotRunning(Runner))
            {
                if (TryGetTarget(out _target) == false)
                    _target = null;
                
                HasFireRequest = false;
                Cooldown = TickTimer.CreateFromSeconds(Runner, _cooldownSeconds);
            }
            
            if (_target != null && _target.IsAlive)
            {
                _target.TryTakeDamage(DamageData.External(_damage, Runner.Tick.Raw));
                FireCount++;
                _target = null;
            }
        }

        public bool CanFire()
        {
            return _condition.IsTrue();
        }

        public void AddCondition(Func<bool> condition)
        {
            _condition.AddCondition(condition);
        }

        public void Fire()
        {
            if (CanFire() == false || Cooldown.ExpiredOrNotRunning(Runner) == false || HasFireRequest)
                return;

            HasFireRequest = true;
            Delay = TickTimer.CreateFromSeconds(Runner, _delaySeconds);
        }

        protected abstract bool TryGetTarget(out HealthComponent target);

        private void OnFireCountRender()
        {
            Fired?.Invoke();
        }
    }
}