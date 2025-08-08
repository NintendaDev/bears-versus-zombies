using Modules.EventBus;
using Modules.Localization.Core.Types;
using Modules.SaveSystem.Signals;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace SampleGame.App.UI
{
    public sealed class LanguageTogglePresenter : MonoBehaviour
    {
        [SerializeField, Required, ChildGameObjectsOnly]
        private LanguageToggle _toggle;

        private LocalizationManager _localizationManager;
        private ISignalBus _signalBus;
        
        public Language Language { get; private set; }

        [Inject]
        private void Construct(ISignalBus signalBus, LocalizationManager localizationManager)
        {
            _signalBus = signalBus;
            _localizationManager = localizationManager;

            _toggle.Checked
                .Subscribe(OnToggleCheck)
                .AddTo(this);
        }

        public void InitLanguage(Language language)
        {
            Language = language;
        }

        private void OnToggleCheck(LanguageToggle toggle)
        {
            _localizationManager.SetLanguage(Language);
            _signalBus.Invoke<SaveSignal>();
        }
    }
}