using Modules.Localization.Core.Types;
using UnityEngine;

namespace Modules.Localization.Core.Detectors
{
    public sealed class ConstantLanguageDetector : LanguageDetector
    {
        [SerializeField] private Language _language;

        public override Language GetCurrentLanguage() => _language;
    }
}
