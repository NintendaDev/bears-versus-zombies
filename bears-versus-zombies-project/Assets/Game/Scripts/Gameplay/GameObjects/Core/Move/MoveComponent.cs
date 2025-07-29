using System;
using Fusion;
using Fusion.Addons.SimpleKCC;
using Modules.Conditions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.GameObjects
{
    [RequireComponent(typeof(SimpleKCC))]
    public class MoveComponent : NetworkBehaviour
    {
        [SerializeField, MinValue(0)] private float _speed = 5;
        
        private const int SpeedMultiplier = 100;
        private readonly AndCondition _condition = new();
        private SimpleKCC _characterController;

        [Networked]
        [OnChangedRender(nameof(OnRenderCurrentDirection))]
        private Vector3 CurrentDirection { get; set; }

        public event Action<Vector3> Moved;
        
        public event Action Stopped;

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            _condition.Dispose();
        }

        public void Initialize()
        {
            _characterController = GetComponent<SimpleKCC>();
            AddCondition(() => CurrentDirection != Vector3.zero);
        }

        public void AddCondition(Func<bool> condition)
        {
            _condition.AddCondition(condition);
        }

        public bool CanMove()
        {
            return _condition.IsTrue();
        }

        public void MoveStep(Vector3 direction, float deltaTime)
        {
            CurrentDirection = direction;

            if (CanMove() == false)
                return;

            _characterController.Move(CurrentDirection * _speed * SpeedMultiplier * deltaTime);
        }
        
        private void OnRenderCurrentDirection(NetworkBehaviourBuffer previous)
        {
            Vector3 previousDirection = GetPropertyReader<Vector3>(nameof(CurrentDirection)).Read(previous);
            
            if (CurrentDirection != Vector3.zero)
                Moved?.Invoke(CurrentDirection);
            else if (previousDirection != CurrentDirection)
                Stopped?.Invoke();
        }
    }
}