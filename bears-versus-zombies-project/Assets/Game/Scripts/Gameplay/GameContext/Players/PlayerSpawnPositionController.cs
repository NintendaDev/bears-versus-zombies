using Fusion;
using Fusion.Addons.SimpleKCC;
using Modules.Services;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.GameContext
{
    public sealed class PlayerSpawnPositionController : SceneSimulationBehaviour, ISpawned, IDespawned
    {
        [SerializeField, Required, SceneObjectsOnly]
        private SpawnPointService _playerSpawnPointService;

        private PlayersService _playersService;

        void ISpawned.Spawned()
        {
            _playersService = ServiceLocator.Instance.Get<PlayersService>();
            _playersService.PlayerJoined += OnPlayerJoin;
        }

        void IDespawned.Despawned(NetworkRunner runner, bool hasState)
        {
            _playersService.PlayerJoined -= OnPlayerJoin;
        }

        private void OnPlayerJoin(NetworkObject playerObject)
        {
            if (Runner.IsServer == false)
                return;
            
            Transform spawnPoint = _playerSpawnPointService.NextPoint();
            SimpleKCC characterController = playerObject.GetComponent<SimpleKCC>();
            characterController.SetPosition(spawnPoint.position, teleport: true);
        }
    }
}