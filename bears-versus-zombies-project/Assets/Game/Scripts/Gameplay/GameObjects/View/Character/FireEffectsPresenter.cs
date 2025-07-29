using Modules.VFX;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.GameObjects
{
    public sealed class FireEffectsPresenter : MonoBehaviour
    {
        [SerializeField, Required] private VisualEffect _effect;
        [SerializeField, Required] private FireComponent _fireComponent;

        private void OnEnable()
        {
            _fireComponent.Fired += OnFire;
        }

        private void OnDisable()
        {
            _fireComponent.Fired -= OnFire;
        }

        private void OnFire()
        {
            _effect.Play();
        }
    }
}