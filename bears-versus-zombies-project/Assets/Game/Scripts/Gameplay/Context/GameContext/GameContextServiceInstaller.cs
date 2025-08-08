using Fusion;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace SampleGame.Gameplay.Context
{
    public sealed class GameContextServiceInstaller : MonoBehaviour
    {
        [SerializeField, Required] private GameContext _gameContext;
        [SerializeField, Required] private GameObject[] _servicesRoots;
        
        [Inject]
        private void Construct()
        {
            foreach (GameObject root in _servicesRoots)
            {
                SimulationBehaviour[] simulationBehaviours = root.GetComponentsInChildren<SimulationBehaviour>();
                
                foreach (SimulationBehaviour behaviour in simulationBehaviours)
                    _gameContext.Add(behaviour);
            }
        }
    }
}