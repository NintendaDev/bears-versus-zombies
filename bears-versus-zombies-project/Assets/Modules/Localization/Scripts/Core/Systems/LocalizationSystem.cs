using System;
using Cysharp.Threading.Tasks;
using Modules.Localization.Core.Types;
using UnityEngine;

namespace Modules.Localization.Core.Systems
{
    public abstract class LocalizationSystem : MonoBehaviour, ILocalizationSystem
    {
        public abstract Language CurrentLanguage { get; }
        
        public abstract event Action LocalizationChanged;
        
        public abstract string MakeTranslatedTextByTerm(string term);

        public abstract UniTask InitializeAsync();

        public abstract void SetLanguage(Language language);
    }
}