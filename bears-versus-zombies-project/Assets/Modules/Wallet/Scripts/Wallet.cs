using System;
using Fusion;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Modules.Wallet
{
    public sealed class Wallet : NetworkBehaviour
    {
        [SerializeField, Required, InlineEditor] 
        private WalletSettings _settings;
        
        public int CurrentValue => (Object != null) ? Value : 0;
        
        [Networked]
        [OnChangedRender(nameof(OnChangeValue))]
        private int Value { get; set; }

        public event Action<int> Changed;

        public override void Spawned()
        {
            if (Runner.IsServer)
                Value = _settings.StartValue;

            OnChangeValue();
        }

        [Button, HideInEditorMode]
        public void Add(int amount)
        {
            if (Runner.IsServer == false)
                return;

            Value += amount;
        }
        
        [Button, HideInEditorMode]
        public bool TrySpend(int amount)
        {
            if (Runner.IsServer == false)
                return false;

            if (Value < amount)
                return false;
            
            Value -= amount;
            
            return true;
        }

        private void OnChangeValue()
        {
            Changed?.Invoke(Value);
        }
    }
}