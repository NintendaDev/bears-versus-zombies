using Fusion;

namespace SampleGame.Gameplay.GameContext
{
    public abstract class SceneSimulationBehaviour : SimulationBehaviour
    {
        public void RegisterOnRunner() 
        {
            NetworkRunner runner = NetworkRunner.GetRunnerForGameObject(gameObject);
            
            if (runner.IsRunning)
                runner.AddGlobal(this);
        }

        public void RemoveFromRunner() 
        {
            NetworkRunner runner = NetworkRunner.GetRunnerForGameObject(gameObject);
            
            if (runner.IsRunning)
                runner.RemoveGlobal(this);
        }
    }
}