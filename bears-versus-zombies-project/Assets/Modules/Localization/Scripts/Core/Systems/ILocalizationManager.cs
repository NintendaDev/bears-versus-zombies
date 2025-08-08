using System;
using Cysharp.Threading.Tasks;
using Modules.Localization.Core.Types;

namespace Modules.Localization.Core.Systems
{
    public interface ILocalizationManager : ITranslation
    {
        public Language CurrentLanguage { get; }
        
        public event Action LocalizationChanged;

        public UniTask InitializeAsync();

        public void SetLanguage(Language language);
    }
}