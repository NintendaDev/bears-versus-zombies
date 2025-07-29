using Modules.Timers;
using Sirenix.Utilities;
using UnityEngine;

namespace Modules.VFX
{
    public sealed class VisualEffectLooper : MonoBehaviour, IVisualEffect
    {
        [SerializeField] private float _delay = 3f;
        [SerializeField] private VisualEffect[] _effects;

        private readonly CountdownTimer _timer = new();

        private bool CanPlay { get; set; }
        
        private void Update()
        {
            if (CanPlay == false || _timer.IsRunning)
                return;
            
            _timer.Start(_delay);
            _effects.ForEach(x => x.Play());
        }

        private void OnDestroy()
        {
            _timer.Dispose();
        }

        public void Play()
        {
            CanPlay = true;
        }

        public void Stop()
        {
            CanPlay = false;
        }
    }
}