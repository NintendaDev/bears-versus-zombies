using Fusion;
using SampleGame.Gameplay.GameObjects;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace SampleGame.Gameplay.UI
{
    public sealed class HealthPresenter : NetworkBehaviour
    {
        [SerializeField, Required] private Slider _slider;
        
        [SerializeField, RequiredIn(PrefabKind.PrefabInstance)] 
        private HealthComponent _healthComponent;
        
        public override void Spawned()
        {
            _healthComponent.Changed += OnHealthChange;
            UpdateView();
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            _healthComponent.Changed -= OnHealthChange;
        }

        private void OnHealthChange(int currentValue)
        {
            UpdateView();
        }

        private void UpdateView()
        {
            _slider.value = _healthComponent.Percent;
        }
    }
}