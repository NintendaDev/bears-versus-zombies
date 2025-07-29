using UnityEngine;

namespace SampleGame.Gameplay.GameObjects
{
    public sealed class LookToCameraComponent : MonoBehaviour
    {
        [SerializeField] private bool _isBlockX;
        [SerializeField] private bool _isBlockY;
        [SerializeField] private bool _isBlockZ;
        
        private Transform _transform;
        private Transform _cameraTransform;

        private void Awake()
        {
            _cameraTransform = Camera.main?.transform;
            _transform = transform;
        }

        private void LateUpdate()
        {
            if (_cameraTransform == null)
                return;

            Rotate();
        }

        private void Rotate()
        {
            Vector3 direction = _cameraTransform.position - _transform.position;

            Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
            
            if (_isBlockX || _isBlockY || _isBlockZ)
            {
                Vector3 eulerRotation = lookRotation.eulerAngles;
                
                if (_isBlockX)
                    eulerRotation.x = 0;
            
                if (_isBlockY)
                    eulerRotation.y = 0;
            
                if (_isBlockZ)
                    eulerRotation.z = 0;
                
                lookRotation = Quaternion.Euler(eulerRotation);
            }
            
            _transform.rotation = lookRotation;
        }
    }
}

