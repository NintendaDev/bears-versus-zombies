using System.ComponentModel.DataAnnotations;
using SampleGame.GameObjects.View;
using UnityEngine;

namespace SampleGame.Gameplay.GameObjects
{
    public sealed class TakeDamageAnimationPresenter : MonoBehaviour
    {
        [SerializeField, Required] private Animator _animator;
        [SerializeField, Required] private AnimationParameters _animationParameters;
        [SerializeField, Required] private HealthComponent _healthComponent;
        
        private int _triggerHash;

        private void Awake()
        {
            _triggerHash = Animator.StringToHash(_animationParameters.TakeDamageTriggerName);
        }

        private void OnEnable()
        {
            _healthComponent.Decreased += OnDecreaseHealth;
        }

        private void OnDisable()
        {
            _healthComponent.Decreased -= OnDecreaseHealth;
        }

        private void OnDecreaseHealth()
        {
            _animator.SetTrigger(_triggerHash);
        }
    }
}