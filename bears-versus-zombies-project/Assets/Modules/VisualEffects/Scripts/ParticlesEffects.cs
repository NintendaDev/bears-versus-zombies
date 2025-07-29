using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Modules.VFX
{
    public sealed class ParticlesEffects : VisualEffect
    {
        [SerializeField, Required] private ParticleSystem[] _particleSystems;
        
        public override void Play()
        {
            _particleSystems.ForEach(x => x.Play());
        }

        public override void Stop()
        {
            _particleSystems.ForEach(x => x.Stop());
        }
    }
}