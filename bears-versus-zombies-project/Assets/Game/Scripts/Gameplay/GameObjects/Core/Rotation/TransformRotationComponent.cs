using UnityEngine;

namespace SampleGame.Gameplay.GameObjects
{
    public sealed class TransformRotationComponent : RotationComponent
    {
        private const int SpeedMultiplier = 100;
        
        [SerializeField] private Transform _rotateRoot;
        
        protected override void RotateInternal(Vector3 direction, float deltaTime)
        {
            RotateInternal(GetRotationRoot(), direction, deltaTime);
        }
        
        protected override Transform GetRotationRoot() => (_rotateRoot != null) ? _rotateRoot : SelfTransform;

        private void RotateInternal(Transform source, Vector3 direction, float deltaTime)
        {
            source.rotation = Quaternion.RotateTowards(source.rotation, 
                Quaternion.LookRotation(direction), 
                AngularSpeed * deltaTime * SpeedMultiplier);
        }
    }
}