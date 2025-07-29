using SampleGame.GameObjects.View;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.GameObjects
{
    public sealed class FireAnimationPresenter : MonoBehaviour
    {
        [SerializeField, Required] private Animator _animator;
        [SerializeField, Required] private AnimationParameters _animationParameters;
        [SerializeField, Required] private LagFireComponent _fireComponent;

        private int _triggerHash;

        private void Awake()
        {
            _triggerHash = Animator.StringToHash(_animationParameters.FireTriggerName);
        }

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
            _animator.SetTrigger(_triggerHash);
        }
    }
}