using System;
using System.Text;
using Cysharp.Threading.Tasks;
using Modules.Localization.Core.Systems;
using Modules.Localization.Core.Types;

namespace SampleGame.App
{
    public sealed class LocalizationManager : ILocalizationManager
    {
        private const string RegionsSuffix = "Regions";
        private const string EnumDelimiter = "_";
        private const string TermsDelimiter = "/";
        private readonly ILocalizationManager _localizationManager;
        private readonly StringBuilder _stringBuilder = new();

        public LocalizationManager(ILocalizationManager localizationManager)
        {
            _localizationManager = localizationManager;
        }

        public Language CurrentLanguage => _localizationManager.CurrentLanguage;

        public event Action LocalizationChanged
        {
            add => _localizationManager.LocalizationChanged += value;
            remove => _localizationManager.LocalizationChanged -= value;
        }

        public async UniTask InitializeAsync()
        {
            await _localizationManager.InitializeAsync();
        }

        public void SetLanguage(Language language) => _localizationManager.SetLanguage(language);

        public string MakeTranslatedTextByTerm(string term) => _localizationManager.MakeTranslatedTextByTerm(term);

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