using Modules.Localization.Core.Systems;
using Modules.SaveSystem.SaveLoad;

namespace SampleGame.App
{
    public sealed class LocalizationManagerSerializer : GameSerializer<ILocalizationManager, LocalizationData>
    {
        public LocalizationManagerSerializer(ILocalizationManager service) : base(service)
        {
        }

        protected override LocalizationData Serialize(ILocalizationManager service)
        {
            return new LocalizationData(service.CurrentLanguage);
        }

        protected override void Deserialize(ILocalizationManager service, LocalizationData data)
        {
            service.SetLanguage(data.CurrentLanguage);
        }
    }
}