using Fusion;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace SampleGame.Gameplay.GameContext
{
    [RequireComponent(typeof(GameContextService))]
    public sealed class GameContextServiceInstaller : MonoBehaviour
    {
        [SerializeField, Required] private GameObject[] _servicesRoots;

        [Inject]
        private void Construct()
        {
            GameContextService service = GetComponent<GameContextService>();
            
            foreach (GameObject root in _servicesRoots)
            {
                SimulationBehaviour[] simulationBehaviours = root.GetComponentsInChildren<SimulationBehaviour>();
                
                foreach (SimulationBehaviour behaviour in simulationBehaviours)
                    service.Add(behaviour);
            }
        }
    }
}