using Modules.AudioManagement.Mixer;
using Zenject;

namespace SampleGame.App
{
    public sealed class AudioMixerInstaller : Installer<AudioMixerInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<AudioMixerSystem>().AsSingle();
            Container.BindInterfacesTo<AudioMixerSystemsSerializer>().AsSingle();
        }
    }
}