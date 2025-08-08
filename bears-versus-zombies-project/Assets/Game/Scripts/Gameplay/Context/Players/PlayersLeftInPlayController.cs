using Fusion;

namespace SampleGame.Gameplay.Context
{
    public sealed class PlayersLeftInPlayController : SimulationBehaviour, ISpawned, IDespawned
    {
        private PlayersService _playersService;
        private GameCycle _gameCycle;

        void ISpawned.Spawned()
        {
            _playersService = GameContext.Instance.Get<PlayersService>();
            _gameCycle = GameContext.Instance.Get<GameCycle>();
            _playersService.PlayerLeft += OnPlayerLeft;
        }

        void IDespawned.Despawned(NetworkRunner runner, bool hasState)
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