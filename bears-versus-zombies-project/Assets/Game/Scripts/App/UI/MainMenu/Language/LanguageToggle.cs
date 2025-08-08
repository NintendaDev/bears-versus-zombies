using Modules.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace SampleGame.App.UI
{
    public sealed class LanguageToggle : ToggleBase<LanguageToggle>
    {
        [SerializeField, Required, ChildGameObjectsOnly]
        private Image _languageIcon;

        public void Initialize(Sprite sprite)
        {
            _languageIcon.sprite = sprite;
        }
    }
}