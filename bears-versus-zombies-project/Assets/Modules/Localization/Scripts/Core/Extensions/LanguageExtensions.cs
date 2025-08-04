using Modules.Localization.Core.Types;

namespace Modules.Localization.Core.Extensions
{
    public static class LanguageExtensions
    {
        private const string EnglishLanguageCode = "en";
        private const string RussianLanguageCode = "ru";

        public static string GetLanguageCode(this Language language)
        {
            switch(language)
            {
                case Language.English:
                    return EnglishLanguageCode;

                case Language.Russian:
                    return RussianLanguageCode;

                case Language.None:
                    return EnglishLanguageCode;

                default:
                    throw new System.Exception($"Unknown language type: {language}");
            }
        }
    }
}
