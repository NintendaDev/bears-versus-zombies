using Cysharp.Threading.Tasks;
using Modules.AssetsManagement.StaticData;
using Modules.Specifications;
using Modules.Types;
using UnityEngine;

namespace Modules.AudioManagement.Mixer
{
    public sealed class AudioMixerSystem : IAudioMixerSystem
    {
        private readonly IStaticDataService _staticDataService;
        private const float MinPercent = 0;
        private const float MaxPercent = 1;
        private const float AttenuationLevelMultiplier = 20f;

        private readonly FloatMemorizedValue _lastMusicVolumePercent = new();
        private readonly FloatMemorizedValue _lastEffectVolumePercent = new();
        private readonly FloatValidator _floatValidator = new();
        private AudioMixerConfiguration _configuration;
        private UnityEngine.Audio.AudioMixer _audioMixer;

        public AudioMixerSystem(IStaticDataService staticDataService)
        {
            _staticDataService = staticDataService;
        }

        public float EffectsPercentVolume => _lastEffectVolumePercent.Current;

        public float MusicPercentVolume => _lastMusicVolumePercent.Current;

        public bool IsChanged => _lastEffectVolumePercent.IsChanged() || _lastMusicVolumePercent.IsChanged();

        public UniTask InitializeAsync()
        {
            _configuration = _staticDataService.GetConfiguration<AudioMixerConfiguration>();
            _audioMixer = _configuration.AudioMixer;

            SetMusicPercentVolume(_configuration.DefaultMusicVolumePercent);
            SetEffectsPercentVolume(_configuration.DefaultEffectsVolumePercent);

            return UniTask.CompletedTask;
        }

        public void SetMusicPercentVolume(float percent)
        {
            _lastMusicVolumePercent.Update(percent);
            SetVolume(_configuration.MusicMixerParameter, percent);
        }
            
        public void SetEffectsPercentVolume(float percent)
        {
            _lastEffectVolumePercent.Update(percent);
            SetVolume(_configuration.EffectsMixerParameter, percent);
        }

        public void Mute() =>
            SetVolume(_configuration.MasterMixerParameter, MinPercent);

        public void Unmute() =>
            SetVolume(_configuration.MasterMixerParameter, MaxPercent);

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