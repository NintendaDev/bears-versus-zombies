using System.Text;
using Cysharp.Threading.Tasks;
using Fusion;
using Modules.EventBus;
using Modules.SaveSystem.Signals;
using Modules.Services;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.App.UI
{
    public sealed class RegionTogglePresenter : MonoBehaviour
    {
        [SerializeField, Required, ChildGameObjectsOnly]
        private RegionToggle _toggle;

        private GameFacade _gameFacade;
        private ISignalBus _signalBus;
        private GameLocalizationSystem _localizationSystem;
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
            
            _gameFacade = ServiceLocator.Instance.Get<GameFacade>();
            _signalBus = ServiceLocator.Instance.Get<ISignalBus>();
            _localizationSystem = ServiceLocator.Instance.Get<GameLocalizationSystem>();
            _isInitialized = true;
            
            Subscribe();

            return UniTask.CompletedTask;
        }

        private void Subscribe()
        {
            if (_isInitialized == false)
                return;
            
            _toggle.Checked += OnClick;
            _localizationSystem.LocalizationChanged += OnLocalizationChange;
        }

        public void UpdateRegion(RegionInfo regionInfo)
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append(regionInfo.RegionPing);
            stringBuilder.Append(" ");
            stringBuilder.Append(_localizationSystem.MakeTranslatedText(LocalizationTerm.Measures_Milliseconds));
            stringBuilder.Append(".");
            
            string pingText = stringBuilder.ToString();
            string regionText = _localizationSystem.MakeTranslatedRegionCode(regionInfo.RegionCode).ToUpper();
            
            _toggle.Initialize(regionInfo, regionText, pingText);
        }

        private void OnClick(RegionToggle toggle)
        {
            _gameFacade.SetRegion(toggle.RegionInfo);
            _signalBus.Invoke<SaveSignal>();
        }

        private void OnLocalizationChange()
        {
            UpdateRegion(_toggle.RegionInfo);
        }
        
        private void Unsubscribe()
        {
            if (_isInitialized == false)
                return;
            
            _toggle.Checked -= OnClick;
            _localizationSystem.LocalizationChanged -= OnLocalizationChange;
        }
    }
    
}