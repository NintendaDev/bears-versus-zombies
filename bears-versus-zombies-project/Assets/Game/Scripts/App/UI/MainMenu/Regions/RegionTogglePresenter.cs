using System.Text;
using Fusion;
using Modules.EventBus;
using Modules.SaveSystem.Signals;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace SampleGame.App.UI
{
    public sealed class RegionTogglePresenter : MonoBehaviour
    {
        [SerializeField, Required, ChildGameObjectsOnly]
        private RegionToggle _toggle;

        private INetworkRegionsService _networkRegionsService;
        private ISignalBus _signalBus;
        private LocalizationManager _localizationManager;

        public RegionInfo LastRegionInfo { get; private set; }
        

        [Inject]
        private void Construct(INetworkRegionsService networkRegionsService, ISignalBus signalBus, 
            LocalizationManager localizationManager)
        {
            _networkRegionsService = networkRegionsService;
            _signalBus = signalBus;
            _localizationManager = localizationManager;

            _toggle.Checked
                .Subscribe(OnClick)
                .AddTo(this);
        }

        private void OnEnable()
        {
            _localizationManager.LocalizationChanged += OnLocalizationChange;
        }

        private void OnDisable()
        {
            _localizationManager.LocalizationChanged -= OnLocalizationChange;
        }

        public void UpdateRegion(RegionInfo regionInfo)
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append(regionInfo.RegionPing);
            stringBuilder.Append(" ");
            stringBuilder.Append(_localizationManager.MakeTranslatedText(LocalizationTerm.Measures_Milliseconds));
            stringBuilder.Append(".");
            
            string pingText = stringBuilder.ToString();
            string regionText = _localizationManager.MakeTranslatedRegionCode(regionInfo.RegionCode).ToUpper();
            
            _toggle.Initialize(regionText, pingText);
            LastRegionInfo = regionInfo;
        }

        private void OnClick(RegionToggle toggle)
        {
            _networkRegionsService.SetRegion(LastRegionInfo);
            _signalBus.Invoke<SaveSignal>();
        }

        private void OnLocalizationChange()
        {
            UpdateRegion(LastRegionInfo);
        }
    }
}