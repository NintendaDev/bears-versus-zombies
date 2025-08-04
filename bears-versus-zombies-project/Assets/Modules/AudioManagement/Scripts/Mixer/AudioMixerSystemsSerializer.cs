using Modules.SaveSystem.SaveLoad;

namespace Modules.AudioManagement.Mixer
{
    public sealed class AudioMixerSystemsSerializer : GameSerializer<IAudioMixerSystem, AudioMixerData>
    {
        public AudioMixerSystemsSerializer(IAudioMixerSystem service) : base(service)
        {
        }

        protected override AudioMixerData Serialize(IAudioMixerSystem service) =>
            new AudioMixerData(service.MusicPercentVolume, service.EffectsPercentVolume);

        protected override void Deserialize(IAudioMixerSystem service, AudioMixerData data)
        {
            service.Reset();
            service.SetMusicPercentVolume(data.MusicPercentVolume);
            service.SetEffectsPercentVolume(data.EffectsPercentVolume);
        }
    }
}