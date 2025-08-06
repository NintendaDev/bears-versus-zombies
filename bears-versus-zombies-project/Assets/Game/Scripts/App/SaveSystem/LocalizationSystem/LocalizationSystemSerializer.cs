using Modules.Localization.Core.Systems;
using Modules.SaveSystem.SaveLoad;

namespace SampleGame.App
{
    public sealed class LocalizationSystemSerializer : GameSerializer<ILocalizationSystem, LocalizationSystemData>
    {
        public LocalizationSystemSerializer(ILocalizationSystem service) : base(service)
        {
        }

        protected override LocalizationSystemData Serialize(ILocalizationSystem service)
        {
            return new LocalizationSystemData(service.CurrentLanguage);
        }

        protected override void Deserialize(ILocalizationSystem service, LocalizationSystemData data)
        {
            service.SetLanguage(data.CurrentLanguage);
        }
    }
}