using Cysharp.Threading.Tasks;

namespace Modules.AudioManagement.Mixer
{
    public interface IAudioMixerSystem
    {
        public float EffectsPercentVolume { get; }

        public float MusicPercentVolume { get; }

        public bool IsChanged { get; }
        
        public UniTask InitializeAsync();

        public void SetEffectsPercentVolume(float percent);

        public void SetMusicPercentVolume(float percent);

        public void Mute();

        public void Unmute();

        public void Reset();
    }
}