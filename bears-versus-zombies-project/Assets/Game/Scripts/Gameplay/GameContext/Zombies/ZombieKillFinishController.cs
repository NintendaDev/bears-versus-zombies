using Fusion;

namespace SampleGame.Gameplay.GameContext
{
    public sealed class ZombieKillFinishController : NetworkBehaviour
    {
        private ZombieSpawner _spawner;
        private GameCycle _gameCycle;

        [Networked]
        private NetworkBool IsFinished { get; set; }

        public override void Spawned()
        {
            _gameCycle = GameContextService.Instance.Get<GameCycle>();
            _spawner = GameContextService.Instance.Get<ZombieSpawner>();
        }

        public override void FixedUpdateNetwork()
        {
            if (IsFinished || Runner.IsServer == false)
                return;

            if (_spawner.AliveZombiesCount == 0 && _spawner.IsFinished)
            {
                _gameCycle.FinishGame(FinishReason.Win);
                IsFinished = true;
            }
        }
    }
}