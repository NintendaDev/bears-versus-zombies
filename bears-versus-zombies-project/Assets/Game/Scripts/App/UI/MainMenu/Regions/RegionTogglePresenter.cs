using System.Text;
using Fusion;
using Modules.EventBus;
using Modules.SaveSystem.Signals;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace SampleGame.App.UI
{
    public sealed class RegionTogglePresenter : MonoBehaviour
    {
        [SerializeField, Required, ChildGameObjectsOnly]
        private RegionToggle _toggle;

        private GameFacade _gameFacade;
        private ISignalBus _signalBus;
        private GameLocalizationSystem _localizationSystem;

        [Inject]
        private void Construct(GameFacade gameFacade, ISignalBus signalBus, 
            GameLocalizationSystem gameLocalizationSystem)
        {
            _gameFacade = gameFacade;
            _signalBus = signalBus;
            _localizationSystem = gameLocalizationSystem;
        }

        private void OnEnable()
        {
            _toggle.Checked += OnClick;
            _localizationSystem.LocalizationChanged += OnLocalizationChange;
        }

        private void OnDisable()
        {
            _toggle.Checked -= OnClick;
            _localizationSystem.LocalizationChanged -= OnLocalizationChange;
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
    }
}