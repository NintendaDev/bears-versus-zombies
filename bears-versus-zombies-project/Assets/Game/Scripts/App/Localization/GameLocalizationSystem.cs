using System;
using System.Text;
using Cysharp.Threading.Tasks;
using Modules.Localization.Core.Systems;
using Modules.Localization.Core.Types;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.App
{
    public sealed class GameLocalizationSystem : MonoBehaviour, ILocalizationSystem
    {
        [SerializeField, Required] private LocalizationSystem _localizationSystem;
        
        private const string RegionsSuffix = "Regions";
        private const string EnumDelimiter = "_";
        private const string TermsDelimiter = "/";
        private readonly StringBuilder _stringBuilder = new();

        public Language CurrentLanguage => _localizationSystem.CurrentLanguage;

        public event Action LocalizationChanged
        {
            add => _localizationSystem.LocalizationChanged += value;
            remove => _localizationSystem.LocalizationChanged -= value;
        }

        public async UniTask InitializeAsync()
        {
            await _localizationSystem.InitializeAsync();
        }

        public void SetLanguage(Language language) => _localizationSystem.SetLanguage(language);

        public string MakeTranslatedTextByTerm(string term) => _localizationSystem.MakeTranslatedTextByTerm(term);

        public string MakeTranslatedText(LocalizationTerm localizationTerm)
        {
            string term = localizationTerm.ToString();
            term = term.Replace(EnumDelimiter, TermsDelimiter);
            
            return MakeTranslatedTextByTerm(term);
        }

        public string MakeTranslatedRegionCode(string regionCode)
        {
            _stringBuilder.Clear();
            _stringBuilder.Append(RegionsSuffix);
            _stringBuilder.Append(TermsDelimiter);
            _stringBuilder.Append(regionCode);
            
            return MakeTranslatedTextByTerm(_stringBuilder.ToString());
        }
    }
}