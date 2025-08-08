using Modules.Localization.Core.Types;

namespace SampleGame.App
{
    public struct LocalizationData
    {
        public LocalizationData(Language currentLanguage)
        {
            CurrentLanguage = currentLanguage;
        }

        public Language CurrentLanguage { get; }
    }
}