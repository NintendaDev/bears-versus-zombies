using Modules.VFX;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.GameObjects
{
    public sealed class DieEffectsPresenter : MonoBehaviour
    {
        [SerializeField, Required] private VisualEffect _effects;
        [SerializeField, Required] private HealthComponent _healthComponent;
        
        private int _parameterHash;

        private void OnEnable()
        {
            _healthComponent.Die += OnDie;
        }

        private void OnDisable()
        {
            _healthComponent.Die -= OnDie;
        }

        private void OnDie()
        {
            _effects.Play();
        }
    }
}