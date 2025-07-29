using System;
using Cysharp.Threading.Tasks;
using I2.Loc;
using Modules.Localization.Core.Detectors;
using Modules.Localization.Core.Systems;
using Modules.Localization.Core.Types;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Modules.Localization.I2System
{
    public sealed class I2LocalizationSystem : LocalizationSystem
    {
        [SerializeField, Required] private LanguageDetector _languageDetector;
        [SerializeField, Required] private I2LanguagesMapping _languageMapping;
        
        private Language _currentLanguage;

        public override Language CurrentLanguage => _currentLanguage;
        
        public override event Action LocalizationChanged;

        private void OnEnable()
        {
            LocalizationManager.OnLocalizeEvent += InLocalizationChange;
        }

        private void OnDisable()
        {
            LocalizationManager.OnLocalizeEvent -= InLocalizationChange;
        }
        
        public override UniTask InitializeAsync()
        {
            string currentLanguageName = _languageDetector.GetCurrentLanguageName();

            if (LocalizationManager.HasLanguage(currentLanguageName))
                LocalizationManager.CurrentLanguage = currentLanguageName;
            
            _currentLanguage = _languageMapping.GetLanguageByCode(LocalizationManager.CurrentLanguage);
            
            return UniTask.CompletedTask;
        }

        public override void SetLanguage(Language language)
        {
            LocalizationManager.CurrentLanguage = _languageMapping.GetLanguageCode(language);
            _currentLanguage = language;
        }

        public override string MakeTranslatedTextByTerm(string term)
        {
            if (LocalizationManager.TryGetTranslation(term, out string translation))
                return translation;

            return term;
        }

        private void InLocalizationChange() =>
            LocalizationChanged?.Invoke();
    }
}