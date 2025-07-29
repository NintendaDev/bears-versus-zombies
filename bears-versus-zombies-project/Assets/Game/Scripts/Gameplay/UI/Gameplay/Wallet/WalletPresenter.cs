using Fusion;
using Modules.Services;
using Modules.Wallet;
using SampleGame.Gameplay.GameContext;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace SampleGame.Gameplay.UI
{
    public sealed class WalletPresenter : SceneSimulationBehaviour, ISpawned, IDespawned
    {
        [SerializeField, Required] private TMP_Text _view;
        
        private Wallet _wallet;
        
        void ISpawned.Spawned()
        {
            _wallet = ServiceLocator.Instance.Get<Wallet>();
            OnWalletChange(_wallet.CurrentValue);
            
            _wallet.Changed += OnWalletChange;
        }

        void IDespawned.Despawned(NetworkRunner runner, bool hasState)
        {
            _wallet.Changed -= OnWalletChange;
        }

        private void OnWalletChange(int currentValue)
        {
            _view.text = currentValue.ToString();
        }
    }
}