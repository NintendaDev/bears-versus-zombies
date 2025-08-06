using Modules.EventBus;
using Modules.SaveSystem.Signals;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace SampleGame.App.UI
{
    public sealed class LanguageTogglePresenter : MonoBehaviour
    {
        [SerializeField, Required, ChildGameObjectsOnly]
        private LanguageToggle _toggle;

        private GameLocalizationSystem _localizationSystem;
        private ISignalBus _signalBus;

        [Inject]
        private void Construct(ISignalBus signalBus, GameLocalizationSystem localizationSystem)
        {
            _signalBus = signalBus;
            _localizationSystem = localizationSystem;
        }

        private void OnEnable()
        {
            _toggle.Checked += OnToggleCheck;
        }

        private void OnDisable()
        {
            _toggle.Checked -= OnToggleCheck;
        }

        private void OnToggleCheck(LanguageToggle toggle)
        {
            _localizationSystem.SetLanguage(toggle.Language);
            _signalBus.Invoke<SaveSignal>();
        }
    }
}