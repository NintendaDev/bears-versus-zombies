using Fusion;

namespace SampleGame.Gameplay.Context
{
    public interface IGameContext
    {
        public void Add(SimulationBehaviour service);
        
        public T Get<T>() where T : SimulationBehaviour;
    }
}