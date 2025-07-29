using Fusion;
using Modules.Services;

namespace SampleGame.Gameplay.GameContext
{
    public sealed class PlayersLeftInPlayController : SceneSimulationBehaviour, ISpawned, IDespawned
    {
        private PlayersService _playersService;
        private GameCycle _gameCycle;

        public void Spawned()
        {
            _playersService = ServiceLocator.Instance.Get<PlayersService>();
            _gameCycle = ServiceLocator.Instance.Get<GameCycle>();
            _playersService.PlayerLeft += OnPlayerLeft;
        }

        public void Despawned(NetworkRunner runner, bool hasState)
        {
            _playersService.PlayerLeft -= OnPlayerLeft;
        }

        private void OnPlayerLeft(NetworkObject _)
        {
            if (_gameCycle.IsPlaying == false)
                return;
            
            _gameCycle.FinishGame(FinishReason.PlayerDisconnect);
        }
    }
}