using Cysharp.Threading.Tasks;
using Modules.EventBus;
using Modules.SaveSystem.Signals;
using Modules.Services;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.App.UI
{
    public sealed class LanguageTogglePresenter : MonoBehaviour
    {
        [SerializeField, Required, ChildGameObjectsOnly]
        private LanguageToggle _toggle;

        private GameLocalizationSystem _localizationSystem;
        private ISignalBus _signalBus;
        private bool _isInitialized;

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        public UniTask InitializeAsync()
        {
            if (_isInitialized)
                return UniTask.CompletedTask;
            
            _localizationSystem = ServiceLocator.Instance.Get<GameLocalizationSystem>();
            _signalBus = ServiceLocator.Instance.Get<ISignalBus>();
            _isInitialized = true;
            
            Subscribe();
            
            return UniTask.CompletedTask;
        }

        private void Subscribe()
        {
            if (_isInitialized == false)
                return;
            
            _toggle.Checked += OnToggleCheck;
        }

        private void OnToggleCheck(LanguageToggle toggle)
        {
            _localizationSystem.SetLanguage(toggle.Language);
            _signalBus.Invoke<SaveSignal>();
        }
        
        private void Unsubscribe()
        {
            if (_isInitialized == false)
                return;
            
            _toggle.Checked -= OnToggleCheck;
        }
    }
}