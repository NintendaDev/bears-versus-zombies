using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

namespace SampleGame.Gameplay.GameContext
{
    public class PlayerCameraProvider : MonoBehaviour
    {
        [SerializeField, Required] private CinemachineCamera _camera;
        
        public CinemachineCamera Camera => _camera;
    }
}