using Fusion.Photon.Realtime;

namespace SampleGame.App
{
    public partial class GameFacade
    {
        private FusionAppSettings CreateAppSettings()
        {
            FusionAppSettings appSettings = PhotonAppSettings.Global.AppSettings.GetCopy();
            appSettings.FixedRegion = _networkRegionsService.CurrentRegion.ToLower();
            
            return appSettings;
        }
    }
}