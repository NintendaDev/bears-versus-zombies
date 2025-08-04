using System;
using Cysharp.Threading.Tasks;
using Fusion;
using Modules.AssetsManagement.AddressablesOperations;
using Modules.AssetsManagement.StaticData;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Object = UnityEngine.Object;

namespace SampleGame.App.UI
{
    public sealed class RegionToggleFactory : IDisposable
    {
        private readonly IAddressablesService _addressablesService;
        private readonly MainMenuAssets _assetsConfig;
        private readonly IInstantiator _container;
        private readonly GameFacade _gameFacade;
        private RegionToggle _prefab;

        public RegionToggleFactory(IInstantiator container, GameFacade gameFacade, IStaticDataService staticDataService,
            IAddressablesService addressablesService)
        {
            _container = container;
            _gameFacade = gameFacade;
            _addressablesService = addressablesService;
            _assetsConfig = staticDataService.GetConfiguration<MainMenuAssets>();
        }

        public void Dispose()
        {
            _addressablesService.Release(_assetsConfig.RegionToggleReference);
        }

        public async UniTask InitializeAsync()
        {
            GameObject regionToggleObject = await _addressablesService
                .LoadAsync<GameObject>(_assetsConfig.RegionToggleReference);
            
            _prefab = regionToggleObject.GetComponent<RegionToggle>();
        }

        public RegionToggle Create(RegionInfo regionInfo, ToggleGroup toggleGroup, Transform parent)
        {
            RegionToggle toggle = _container.InstantiatePrefab(_prefab, parent).GetComponent<RegionToggle>();
            RegionTogglePresenter presenter = toggle.GetComponent<RegionTogglePresenter>();
            
            presenter.UpdateRegion(regionInfo);
            toggle.Link(toggleGroup);
            
            if (regionInfo.RegionCode == _gameFacade.CurrentRegion)
                toggle.Select();

            return toggle;
        }
    }
}