using SampleGame.Gameplay.Context;
using SampleGame.Gameplay.GameObjects;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.PlayerContext
{
    public class CharacterMoveController : MonoBehaviour
    {
        [SerializeField, Required]
        private InputReceiver _inputReceiver;
        
        [SerializeField, Required]
        private MoveComponent _movementComponent;
        
        [SerializeField, Required]
        private RotationComponent _rotationComponent;
        
        [SerializeField]
        private AutoFireComponent _autoFireComponent;

        private void OnEnable()
        {
            _inputReceiver.OnMove += OnMove;
        }

        private void OnDisable()
        {
            _inputReceiver.OnMove -= OnMove;
        }

        private void OnMove(Vector3 direction, float deltaTime)
        {
            _movementComponent.MoveStep(direction, deltaTime);
            
            if (_autoFireComponent != null && _autoFireComponent.HasTarget == false)
                _rotationComponent.TryRotate(direction, deltaTime);
        }
    }
}