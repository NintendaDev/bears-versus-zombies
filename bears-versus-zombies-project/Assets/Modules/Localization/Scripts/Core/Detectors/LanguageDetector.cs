using Modules.Localization.Core.Types;
using UnityEngine;

namespace Modules.Localization.Core.Detectors
{
    public abstract class LanguageDetector : MonoBehaviour, ILanguageDetector
    {
        public abstract Language GetCurrentLanguage();

        public string GetCurrentLanguageName()
        {
            Language currentLanguage = GetCurrentLanguage();

            return currentLanguage.ToString();
        }
    }
}
