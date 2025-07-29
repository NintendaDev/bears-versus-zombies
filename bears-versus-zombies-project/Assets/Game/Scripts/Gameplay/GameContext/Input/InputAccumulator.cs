using Fusion;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SampleGame.Gameplay.GameContext
{
    public sealed class InputAccumulator : SimulationBehaviour, ISpawned, IDespawned
    {
        [SerializeField, Required] private NetworkEvents _networkEvents;
        
        private GameInput _input;
        private InputAction _moveAction;
        private InputAction _turretAction;
        private InputAction _mineAction;
        
        void ISpawned.Spawned()
        {
            _input = new GameInput();
            _moveAction = _input.Gameplay.Move;
            _turretAction = _input.Gameplay.Turret;
            _mineAction = _input.Gameplay.Mine;
            
            _networkEvents.OnInput.AddListener(OnInput);
            Enable();
        }

        void IDespawned.Despawned(NetworkRunner runner, bool hasState)
        {
            _networkEvents.OnInput.RemoveListener(OnInput);
            Disable();
        }

        public void Enable()
        {
            _input.Enable();
        }

        public void Disable()
        {
            _input.Disable();
        }

        private void OnInput(NetworkRunner runner, NetworkInput input)
        {
            var buttons = new NetworkButtons();
            
            AccumulateAllButtons(ref buttons);
            
            var inputData = new InputData
            {
                MoveDirection = GetMoveDirection(),
                Buttons = buttons
            };

            input.Set(inputData);
        }

        private void AccumulateAllButtons(ref NetworkButtons buttons)
        {
            AccumulateButton(ref buttons, _turretAction, InputKeys.Turret);
            AccumulateButton(ref buttons, _mineAction, InputKeys.Mine);
        }

        private void AccumulateButton(ref NetworkButtons buttons, InputAction action, InputKeys key)
        {
            if (action == null)
                return;
            
            buttons.Set(key, action.IsPressed());
        }

        private Vector3 GetMoveDirection()
        {
            if (_moveAction == null)
                return Vector3.zero;
            
            Vector2 movementInput =_moveAction.ReadValue<Vector2>();

            return new Vector3(movementInput.x, 0, movementInput.y);
        }
    }
}