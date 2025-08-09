using Modules.SaveSystem.SaveLoad;

namespace SampleGame.App
{
    public sealed class NetworkRegionsServiceSerializer : GameSerializer<INetworkRegionsService, NetworkRegionsData>
    {
        public NetworkRegionsServiceSerializer(INetworkRegionsService service) : base(service)
        {
        }

        protected override NetworkRegionsData Serialize(INetworkRegionsService service)
        {
            return new NetworkRegionsData(service.CurrentRegion);
        }

        protected override void Deserialize(INetworkRegionsService service, NetworkRegionsData data)
        {
            service.SetRegion(data.CurrentRegion);
        }
    }
}