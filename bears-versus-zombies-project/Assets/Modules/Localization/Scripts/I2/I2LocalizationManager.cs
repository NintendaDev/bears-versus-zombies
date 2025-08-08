using System;
using Cysharp.Threading.Tasks;
using I2.Loc;
using Modules.Localization.Core.Detectors;
using Modules.Localization.Core.Systems;
using Modules.Localization.Core.Types;

namespace Modules.Localization.I2System
{
    public sealed class I2LocalizationManager : ILocalizationManager, IDisposable
    {
        private readonly ILanguageDetector _languageDetector;
        private readonly I2LanguagesMapping _languageMapping;
        private Language _currentLanguage;

        public I2LocalizationManager(ILanguageDetector languageDetector, I2LanguagesMapping languageMapping)
        {
            _languageDetector = languageDetector;
            _languageMapping = languageMapping;
            
            LocalizationManager.OnLocalizeEvent += InLocalizationChange;
        }

        public Language CurrentLanguage => _currentLanguage;
        
        public event Action LocalizationChanged;

        public void Dispose()
        {
            LocalizationManager.OnLocalizeEvent -= InLocalizationChange;
        }
        
        public UniTask InitializeAsync()
        {
            string currentLanguageName = _languageDetector.GetCurrentLanguageName();

            if (LocalizationManager.HasLanguage(currentLanguageName))
                LocalizationManager.CurrentLanguage = currentLanguageName;
            
            _currentLanguage = _languageMapping.GetLanguageByCode(LocalizationManager.CurrentLanguage);
            
            return UniTask.CompletedTask;
        }

        public void SetLanguage(Language language)
        {
            LocalizationManager.CurrentLanguage = _languageMapping.GetLanguageCode(language);
            _currentLanguage = language;
        }

        public string MakeTranslatedTextByTerm(string term)
        {
            if (LocalizationManager.TryGetTranslation(term, out string translation))
                return translation;

            return term;
        }

        private void InLocalizationChange() =>
            LocalizationChanged?.Invoke();
    }
}