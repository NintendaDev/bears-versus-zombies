using Fusion;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

namespace SampleGame.Gameplay.GameContext
{
    public class PlayerCameraProvider : SimulationBehaviour
    {
        [SerializeField, Required] private CinemachineCamera _camera;
        
        public CinemachineCamera Camera => _camera;
    }
}