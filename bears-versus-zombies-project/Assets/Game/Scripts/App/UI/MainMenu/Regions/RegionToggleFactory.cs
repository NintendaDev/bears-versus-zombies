using Cysharp.Threading.Tasks;
using Fusion;
using Modules.AssetsManagement.AddressablesOperations;
using Modules.AssetsManagement.StaticData;
using Modules.Services;
using UnityEngine;
using UnityEngine.UI;

namespace SampleGame.App.UI
{
    public sealed class RegionToggleFactory : MonoBehaviour
    {
        private IAddressablesService _addressablesService;
        private MainMenuAssets _assetsConfig;

        private RegionToggle _prefab;
        private GameFacade _gameFacade;

        private void OnDestroy()
        {
            _addressablesService.Release(_assetsConfig.RegionToggleReference);
        }

        public async UniTask InitializeAsync()
        {
            _gameFacade = ServiceLocator.Instance.Get<GameFacade>();
            IStaticDataService staticData = ServiceLocator.Instance.Get<IStaticDataService>();
            _addressablesService = ServiceLocator.Instance.Get<IAddressablesService>();
            _assetsConfig = staticData.GetConfiguration<MainMenuAssets>();

            await LoadAssetsAsync();
        }

        public async UniTask<RegionToggle> CreateAsync(RegionInfo regionInfo, ToggleGroup toggleGroup, Transform parent)
        {
            RegionToggle toggle = Instantiate(_prefab, parent);
            RegionTogglePresenter presenter = toggle.GetComponent<RegionTogglePresenter>();
            await presenter.InitializeAsync();
            
            presenter.UpdateRegion(regionInfo);
            toggle.Link(toggleGroup);
            
            if (regionInfo.RegionCode == _gameFacade.CurrentRegion)
                toggle.Select();

            return toggle;
        }
        
        private async UniTask LoadAssetsAsync()
        {
            GameObject regionToggleObject = await _addressablesService
                .LoadAsync<GameObject>(_assetsConfig.RegionToggleReference);
            
            _prefab = regionToggleObject.GetComponent<RegionToggle>();
        }
    }
}