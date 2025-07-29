using Modules.Localization.Core.Systems;
using Modules.SaveSystem.SaveLoad;

namespace SampleGame.App
{
    public sealed class LocalizationSystemSerializer : GameSerializer<LocalizationSystem, LocalizationSystemData>
    {
        protected override LocalizationSystemData Serialize(LocalizationSystem service)
        {
            return new LocalizationSystemData(service.CurrentLanguage);
        }

        protected override void Deserialize(LocalizationSystem service, LocalizationSystemData data)
        {
            service.SetLanguage(data.CurrentLanguage);
        }
    }
}