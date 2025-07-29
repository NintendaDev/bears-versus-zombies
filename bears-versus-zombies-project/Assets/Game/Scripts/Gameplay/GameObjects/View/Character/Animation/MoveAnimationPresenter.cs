using SampleGame.GameObjects.View;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.GameObjects
{
    public sealed class MoveAnimationPresenter : MonoBehaviour
    {
        [SerializeField, Required] private Animator _animator;
        [SerializeField, Required] private AnimationParameters _animationParameters;
        [SerializeField, Required] private MoveComponent _moveComponent;
        [SerializeField, Required] private HealthComponent _health;
        
        private int _isMoveParameterHash;
        private int _isIdleParameterHash;

        private void Awake()
        {
            _isMoveParameterHash = Animator.StringToHash(_animationParameters.IsMovingParameterName);
            _isIdleParameterHash = Animator.StringToHash(_animationParameters.IsIdleParameterName);
        }

        private void OnEnable()
        {
            _moveComponent.Moved += OnMove;
            _moveComponent.Stopped += OnStopMove;
            _health.Die += OnDie;
        }

        private void OnDisable()
        {
            _moveComponent.Moved -= OnMove;
            _moveComponent.Stopped -= OnStopMove;
            _health.Die -= OnDie;
        }

        private void OnMove(Vector3 direction)
        {
            _animator.SetBool(_isMoveParameterHash, true);
            _animator.SetBool(_isIdleParameterHash, false);
        }

        private void OnStopMove()
        {
            _animator.SetBool(_isMoveParameterHash, false);
            _animator.SetBool(_isIdleParameterHash, true);
        }

        private void OnDie()
        {
            _animator.SetBool(_isMoveParameterHash, false);
            _animator.SetBool(_isIdleParameterHash, false);
        }
    }
}