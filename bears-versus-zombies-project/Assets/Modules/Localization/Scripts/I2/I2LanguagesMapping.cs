using System;
using Modules.Localization.Core.Types;
using UnityEngine;
using ZLinq;

namespace Modules.Localization.I2System
{
    [CreateAssetMenu(fileName = "new I2LanguagesMapping", menuName = "Modules/Localization/I2LanguagesMapping")]
    public sealed class I2LanguagesMapping : ScriptableObject
    {
        [SerializeField] private LanguageMap[] _mappings;

        public string GetLanguageCode(Language language)
        {
            LanguageMap mapping = _mappings.AsValueEnumerable()
                .Where(x => x.Language == language)
                .FirstOrDefault();
            
            if (mapping == null)
                throw new NullReferenceException($"Language {language} is not mapped");
            
            return mapping.Code;
        }
        
        public Language GetLanguageByCode(string languageCode)
        {
            LanguageMap mapping = _mappings.AsValueEnumerable()
                .Where(x => x.Code == languageCode)
                .FirstOrDefault();
            
            if (mapping == null)
                throw new NullReferenceException($"Language code {languageCode} is not mapped");
            
            return mapping.Language;
        }
        
        [Serializable]
        private class LanguageMap
        {
            [field: SerializeField] public string Code { get; private set; }
            
            [field: SerializeField] public Language Language { get; private set; }
        }
    }
}