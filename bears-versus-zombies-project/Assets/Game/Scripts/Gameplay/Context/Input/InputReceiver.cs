using System;
using Fusion;
using UnityEngine;

namespace SampleGame.Gameplay.Context
{
    public class InputReceiver : NetworkBehaviour, IBeforeTick
    {
        private Vector3 _previousMoveDirection;
        
        [Networked]
        private NetworkButtons _previousButtons { get; set; }

        public event Action<Vector3, float> OnMove;
        
        public event Action<TrapType> TrapRequested;

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out InputData input) == false) 
                return;
            
            float deltaTime = Runner.DeltaTime;
            
            ProcessMove(input, deltaTime);
            ProcessButtons(input);
            
            _previousButtons = input.Buttons;
        }

        public void BeforeTick()
        {
        }

        private void ProcessMove(InputData input, float deltaTime)
        {
            Vector3 moveDirection = input.MoveDirection.normalized;
            
            if (moveDirection != Vector3.zero || _previousMoveDirection != moveDirection)
                OnMove?.Invoke(moveDirection, deltaTime);

            _previousMoveDirection = moveDirection;
        }

        private void ProcessButtons(InputData input)
        {
            if (input.Buttons.WasPressed(_previousButtons, InputKeys.Turret))
                TrapRequested?.Invoke(TrapType.Turret);
            
            if (input.Buttons.WasPressed(_previousButtons, InputKeys.Mine))
                TrapRequested?.Invoke(TrapType.Mine);
        }
    }
}