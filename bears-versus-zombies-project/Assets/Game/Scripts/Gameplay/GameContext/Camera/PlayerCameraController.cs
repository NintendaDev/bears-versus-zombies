using Fusion;
using SampleGame.Gameplay.GameObjects;

namespace SampleGame.Gameplay.GameContext
{
    public sealed class PlayerCameraController : SimulationBehaviour, ISpawned, IDespawned
    {
        private PlayerCameraProvider _cameraProvider;
        private PlayersService _playersService;
        private HealthComponent _playerHealth;
        
        void ISpawned.Spawned()
        {
            _cameraProvider = GameContextService.Instance.Get<PlayerCameraProvider>();
            _playersService = GameContextService.Instance.Get<PlayersService>();

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