using Fusion.Addons.FSM;

namespace SampleGame.Gameplay.GameObjects
{
    public abstract class ZombieStateBase : State<ZombieStateBase>
    {
        private readonly ZombieAI _zombieAI;

        protected ZombieStateBase(ZombieAI zombieAI)
        {
            _zombieAI = zombieAI;
        }

        protected ZombieBlackboard Blackboard => _zombieAI.Blackboard;
    }
}