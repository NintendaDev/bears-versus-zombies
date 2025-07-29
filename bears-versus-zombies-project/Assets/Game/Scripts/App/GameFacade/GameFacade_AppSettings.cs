using Fusion.Photon.Realtime;

namespace SampleGame.App
{
    public partial class GameFacade
    {
        private FusionAppSettings CreateAppSettings()
        {
            FusionAppSettings appSettings = PhotonAppSettings.Global.AppSettings.GetCopy();
            appSettings.FixedRegion = CurrentRegion.ToLower();
            
            return appSettings;
        }
    }
}