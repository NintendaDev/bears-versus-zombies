using Cysharp.Threading.Tasks;
using Modules.Specifications;
using Modules.Types;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Modules.AudioManagement.Mixer
{
    public sealed class AudioMixerSystem : MonoBehaviour, IAudioMixerSystem
    {
        [SerializeField, Required] private AudioMixerConfiguration _mixerConfiguration;
        
        private const float MinPercent = 0;
        private const float MaxPercent = 1;
        private const float AttenuationLevelMultiplier = 20f;
        
        private readonly FloatMemorizedValue _lastMusicVolumePercent = new();
        private readonly FloatMemorizedValue _lastEffectVolumePercent = new();
        private readonly FloatValidator _floatValidator = new();
        private UnityEngine.Audio.AudioMixer _audioMixer;

        public float EffectsPercentVolume => _lastEffectVolumePercent.Current;

        public float MusicPercentVolume => _lastMusicVolumePercent.Current;

        public bool IsChanged => _lastEffectVolumePercent.IsChanged() || _lastMusicVolumePercent.IsChanged();

        public UniTask InitializeAsync()
        {
            _audioMixer = _mixerConfiguration.AudioMixer;

            SetMusicPercentVolume(_mixerConfiguration.DefaultMusicVolumePercent);
            SetEffectsPercentVolume(_mixerConfiguration.DefaultEffectsVolumePercent);

            return UniTask.CompletedTask;
        }

        public void SetMusicPercentVolume(float percent)
        {
            _lastMusicVolumePercent.Update(percent);
            SetVolume(_mixerConfiguration.MusicMixerParameter, percent);
        }
            
        public void SetEffectsPercentVolume(float percent)
        {
            _lastEffectVolumePercent.Update(percent);
            SetVolume(_mixerConfiguration.EffectsMixerParameter, percent);
        }

        public void Mute() =>
            SetVolume(_mixerConfiguration.MasterMixerParameter, MinPercent);

        public void Unmute() =>
            SetVolume(_mixerConfiguration.MasterMixerParameter, MaxPercent);

        public void Reset()
        {
            _lastMusicVolumePercent.Reset();
            _lastMusicVolumePercent.Reset();
        }

        private void SetVolume(string mixerParameter, float percent)
        {
            _floatValidator.BetweenZeroAndOne(percent);

            _audioMixer.SetFloat(mixerParameter, Mathf.Log10(percent) * AttenuationLevelMultiplier);
        }
    }
}