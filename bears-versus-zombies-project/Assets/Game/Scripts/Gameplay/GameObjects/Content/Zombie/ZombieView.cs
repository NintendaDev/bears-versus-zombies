using Modules.VFX;
using SampleGame.GameObjects.View;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.GameObjects
{
    public sealed class ZombieView : MonoBehaviour
    {
        [SerializeField, Required, ChildGameObjectsOnly] private Animator _animator;
        [SerializeField, Required] private AnimationParameters _animationParameters;
        [SerializeField, Required] private VisualEffect _dieEffect;
        [SerializeField, Required] private VisualEffect _hitEffect;
        [SerializeField, Required] private VisualEffectLooper _moaningEffect;
        
        private int _isIdleParameterHash;
        private int _isMoveParameterHash;
        private int _takeDamageParameterHash;

        private void Awake()
        {
            _isIdleParameterHash = Animator.StringToHash(_animationParameters.IsIdleParameterName);
            _isMoveParameterHash = Animator.StringToHash(_animationParameters.IsMovingParameterName);
            _takeDamageParameterHash = Animator.StringToHash(_animationParameters.TakeDamageTriggerName);
        }

        public void PlayMoaningEffect() => _moaningEffect.Play();
        
        public void StopMoaningEffect() => _moaningEffect.Stop();

        public void SwitchAnimatorToMoveState()
        {
            ResetAnimator();
            _animator.SetBool(_isIdleParameterHash, false);
            _animator.SetBool(_isMoveParameterHash, true);
        }

        public void ResetAnimator()
        {
            _animator.SetBool(_isIdleParameterHash, false);
            _animator.SetBool(_isMoveParameterHash, false);
        }

        public void SwitchAnimatorToIdleState()
        {
            ResetAnimator();
            _animator.SetBool(_isIdleParameterHash, true);
            _animator.SetBool(_isMoveParameterHash, false);
        }

        public void OnTakeDamage()
        {
            ResetAnimator();
            _animator.SetTrigger(_takeDamageParameterHash);
            _hitEffect.Play();
        }

        public void OnDie()
        {
            _dieEffect.Play();
        }
    }
}