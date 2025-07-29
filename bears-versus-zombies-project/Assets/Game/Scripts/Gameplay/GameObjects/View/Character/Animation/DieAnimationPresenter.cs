using SampleGame.GameObjects.View;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.GameObjects
{
    public sealed class DieAnimationPresenter : MonoBehaviour
    {
        [SerializeField, Required] private Animator _animator;
        [SerializeField, Required] private HealthComponent _healthComponent;
        [SerializeField, Required] private AnimationParameters _animationParameters;
        
        private int _parameterHash;

        private void Awake()
        {
            _parameterHash = Animator.StringToHash(_animationParameters.IsDieParameterName);
        }

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
            _animator.SetBool(_parameterHash, true);
        }
    }
}