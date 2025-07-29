using Modules.Localization.Core.Types;

namespace SampleGame.App
{
    public struct LocalizationSystemData
    {
        public LocalizationSystemData(Language currentLanguage)
        {
            CurrentLanguage = currentLanguage;
        }

        public Language CurrentLanguage { get; }
    }
}