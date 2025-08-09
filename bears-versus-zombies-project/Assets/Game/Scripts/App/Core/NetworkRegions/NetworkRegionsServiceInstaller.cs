using Zenject;

namespace SampleGame.App
{
    public sealed class NetworkRegionsServiceInstaller : Installer<NetworkRegionsServiceInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<NetworkRegionsService>().AsSingle();
            Container.BindInterfacesTo<NetworkRegionsServiceSerializer>().AsSingle();
        }
    }
}