using Sirenix.OdinInspector;
using UnityEngine;

namespace Modules.VFX
{
    public sealed class SoundEffect : VisualEffect
    {
        [SerializeField, Required] private AudioSource _audioSource;
        [SerializeField, Required] private AudioClip[] _clips;
        [SerializeField] private bool _canStop = true;
        
        public override void Play()
        {
            if (_clips.Length == 0)
                return;
            
            _audioSource.PlayOneShot(_clips[Random.Range(0, _clips.Length)]);
        }

        public override void Stop()
        {
            if (_canStop)
                _audioSource.Stop();
        }
    }
}