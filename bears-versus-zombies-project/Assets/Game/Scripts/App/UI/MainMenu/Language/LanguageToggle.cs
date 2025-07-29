using Modules.Localization.Core.Types;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace SampleGame.App.UI
{
    public sealed class LanguageToggle : ToggleBase<LanguageToggle>
    {
        [SerializeField, Required, ChildGameObjectsOnly]
        private Image _languageIcon;
        
        public Language Language { get; private set; }

        public void Initialize(Sprite sprite, Language language)
        {
            _languageIcon.sprite = sprite;
            Language = language;
        }
    }
}