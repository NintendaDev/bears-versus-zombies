using System;
using Fusion;
using Modules.Services;
using Modules.Timers;
using SampleGame.Gameplay.GameObjects;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.GameContext
{
    public sealed class GameInitializeEventer : SceneSimulationBehaviour, ISpawned, IDespawned, IGameInitializeEvent
    {
        [SerializeField, MinValue(0)] private float _delayBeforeInitialize = 1f;
        
        private readonly CountdownTimer _initializeCooldownTimer = new();
        private PlayersService _playersService;
        private PlayerCameraProvider _cameraProvider;
        private InitializeComponent _initializeComponent;
        private Player _localPlayer;
        private bool _isStartedDelayTimer;

        public bool IsInitialized { get; private set; }
        
        public event Action Initialized;
        
        void ISpawned.Spawned()
        {
            _playersService = ServiceLocator.Instance.Get<PlayersService>();
            _cameraProvider = ServiceLocator.Instance.Get<PlayerCameraProvider>();
            _playersService.LocalPlayerJoined += OnLocalPlayerJoin;
        }

        void IDespawned.Despawned(NetworkRunner runner, bool hasState)
        {
            _initializeCooldownTimer.Dispose();
            _playersService.LocalPlayerJoined -= OnLocalPlayerJoin;
        }

        public override void FixedUpdateNetwork()
        {
            if (IsInitialized || _localPlayer == null || Runner.IsForward == false)
                return;
            
            if (_localPlayer.IsSpawned == false || _cameraProvider.Camera.Follow == null)
                return;
            
            if (_isStartedDelayTimer == false)
            {
                _initializeCooldownTimer.Start(_delayBeforeInitialize);
                _isStartedDelayTimer = true;

                return;
            }

            if (_initializeCooldownTimer.IsRunning)
                return;
            
            _initializeComponent.Initialize();
            IsInitialized = true;
            Initialized?.Invoke();
        }

        private void OnLocalPlayerJoin(NetworkObject playerObject)
        {
            _playersService.PlayerJoined -= OnLocalPlayerJoin;
            _localPlayer = playerObject.GetComponent<Player>();
            _initializeComponent = _localPlayer.GetComponent<InitializeComponent>();
        }
    }
}