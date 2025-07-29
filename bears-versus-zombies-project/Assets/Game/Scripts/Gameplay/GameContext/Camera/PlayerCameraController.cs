using Fusion;
using Modules.Services;
using SampleGame.Gameplay.GameObjects;
using UnityEngine;

namespace SampleGame.Gameplay.GameContext
{
    public sealed class PlayerCameraController : SceneSimulationBehaviour, ISpawned, IDespawned
    {
        private PlayerCameraProvider _cameraProvider;
        private PlayersService _playersService;
        private HealthComponent _playerHealth;
        
        void ISpawned.Spawned()
        {
            _cameraProvider = ServiceLocator.Instance.Get<PlayerCameraProvider>();
            _playersService = ServiceLocator.Instance.Get<PlayersService>();

            _playersService.LocalPlayerJoined += OnLocalPlayerJoin;
        }

        void IDespawned.Despawned(NetworkRunner runner, bool hasState)
        {
            _playersService.LocalPlayerJoined -= OnLocalPlayerJoin;

            if (_playerHealth != null)
                _playerHealth.Die -= OnDie;
        }

        private void OnLocalPlayerJoin(NetworkObject localPlayer)
        {
            _playerHealth = localPlayer.GetComponent<HealthComponent>();
            _playerHealth.Die += OnDie;
            _cameraProvider.Camera.Follow = localPlayer.transform;
        }

        private void OnDie()
        {
            _cameraProvider.Camera.Follow = null;
        }
    }
}