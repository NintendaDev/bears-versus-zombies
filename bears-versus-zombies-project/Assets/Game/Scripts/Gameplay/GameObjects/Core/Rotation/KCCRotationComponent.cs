using Fusion.Addons.SimpleKCC;
using UnityEngine;

namespace SampleGame.Gameplay.GameObjects
{
    [RequireComponent(typeof(SimpleKCC))]
    public class KCCRotationComponent : RotationComponent
    {
        private const int SpeedMultiplier = 100;
        private SimpleKCC _kcc;

        public override void Initialize()
        {
            base.Initialize();
            _kcc = GetComponent<SimpleKCC>();
        }

        protected override void RotateInternal(Vector3 direction, float deltaTime)
        {
            _kcc.SetLookRotation(Quaternion.RotateTowards(
                SelfTransform.rotation,
                Quaternion.LookRotation(direction),
                AngularSpeed * deltaTime * SpeedMultiplier));
        }
    }
}