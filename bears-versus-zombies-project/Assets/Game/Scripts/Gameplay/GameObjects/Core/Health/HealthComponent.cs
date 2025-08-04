using System;
using Fusion;
using Sirenix.OdinInspector; 
using UnityEngine;

namespace SampleGame.Gameplay.GameObjects
{
    public class HealthComponent : NetworkBehaviour
    {
        [SerializeField, MinValue(0)] private int _maxValue = 100;
        [SerializeField, MinValue(0)] private float _damageCooldown = 1f;
        
        private Action _dieHandler;

        [Networked]
        [OnChangedRender(nameof(OnCurrentValueChangeRender))]
        public int CurrentValue { get; private set; }

        [Networked]
        public NetworkBool IsDie { get; private set; }

        public int MaxValue => _maxValue;

        public float Percent => CurrentValue / (float) _maxValue;

        public bool IsAlive => Object != null && CurrentValue > 0;

        [Networked]
        public DamageData LastDamageData { get; private set; }
        
        public bool CanTakeDamage => DamageCooldownTickTimer.ExpiredOrNotRunning(Runner);

        [Networked]
        private TickTimer DamageCooldownTickTimer { get; set; }

        public event Action<int> Changed;

        public event Action Decreased;

        public event Action Die;

        public override void Spawned()
        {
            CurrentValue = MaxValue;
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            _dieHandler = null;
        }

        public override void FixedUpdateNetwork()
        {
            if (CurrentValue == 0 && IsDie == false)
            {
                _dieHandler?.Invoke();
                IsDie = true;
            }
        }

        public void AddDieHandler(Action handler)
        {
            _dieHandler += handler;
        }
        
        public bool TryTakeDamage(DamageData damageData)
        {
            if (CanTakeDamage == false)
                return false;
            
            if (damageData.Value < 0)
                throw new ArgumentException("Damage must be positive");
            
            if (CurrentValue == 0)
                return false;
            
            CurrentValue = Mathf.Max(0, CurrentValue - damageData.Value);
            LastDamageData = damageData;
            DamageCooldownTickTimer = TickTimer.CreateFromSeconds(Runner, _damageCooldown);
            
            return true;
        }

        public void Kill(DamageData damageData)
        {
            if (CurrentValue == 0)
                return;
            
            LastDamageData = damageData;
            CurrentValue = 0;
        }

        private void OnCurrentValueChangeRender(NetworkBehaviourBuffer previous)
        {
            Changed?.Invoke(CurrentValue);
            
            int previousValue = GetPropertyReader<int>(nameof(CurrentValue)).Read(previous);

            if (previousValue > CurrentValue)
                Decreased?.Invoke();

            if (CurrentValue == 0)
                Die?.Invoke();
        }
    }
}