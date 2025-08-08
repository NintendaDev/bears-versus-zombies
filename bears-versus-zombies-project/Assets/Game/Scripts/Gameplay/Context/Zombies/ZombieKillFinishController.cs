using Fusion;

namespace SampleGame.Gameplay.Context
{
    public sealed class ZombieKillFinishController : NetworkBehaviour
    {
        private ZombieSpawner _spawner;
        private GameCycle _gameCycle;

        [Networked]
        private NetworkBool IsFinished { get; set; }

        public override void Spawned()
        {
            _gameCycle = GameContext.Instance.Get<GameCycle>();
            _spawner = GameContext.Instance.Get<ZombieSpawner>();
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