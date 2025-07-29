using Modules.VFX;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.GameObjects
{
    public sealed class TakeDamageEffectsPresenter : MonoBehaviour
    {
        [SerializeField, Required] private VisualEffect _effect;
        [SerializeField, Required] private HealthComponent _health;

        private void OnEnable()
        {
            _health.Decreased += OnHealthDecrease;
        }

        private void OnDisable()
        {
            _health.Decreased -= OnHealthDecrease;
        }

        private void OnHealthDecrease()
        {
            if (_health.IsAlive == false)
                return;
            
            _effect.Play();
        }
    }
}