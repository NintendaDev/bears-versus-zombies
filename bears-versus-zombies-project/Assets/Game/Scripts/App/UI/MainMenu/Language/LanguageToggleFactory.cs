using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Modules.AssetsManagement.AddressablesOperations;
using Modules.AssetsManagement.StaticData;
using Modules.Localization.Core.Types;
using Modules.Services;
using UnityEngine;
using UnityEngine.UI;

namespace SampleGame.App.UI
{
    public sealed class LanguageToggleFactory : MonoBehaviour
    {
        private readonly Dictionary<Language, Sprite> _languageSprites = new();
        private IAddressablesService _addressablesService;
        private MainMenuAssets _assetsConfig;
        private LocalizeRegionsConfig _languagesConfig;

        private LanguageToggle _prefab;
        private GameLocalizationSystem _localizationSystem;
        
        private void OnDestroy()
        {
            _addressablesService.Release(_assetsConfig.LanguageToggleReference);
            
            foreach (LocalizeRegionsConfig.LocalizeRegionData regionData in _languagesConfig.Regions)
                _addressablesService.Release(regionData.SpriteRef);
        }

        public async UniTask InitializeAsync()
        {
            IStaticDataService staticData = ServiceLocator.Instance.Get<IStaticDataService>();
            _localizationSystem = ServiceLocator.Instance.Get<GameLocalizationSystem>();
            _addressablesService = ServiceLocator.Instance.Get<IAddressablesService>();
            _assetsConfig = staticData.GetConfiguration<MainMenuAssets>();
            _languagesConfig = staticData.GetConfiguration<LocalizeRegionsConfig>();

            await LoadAssetsAsync();
        }

        public async UniTask<LanguageToggle> CreateAsync(Language language, ToggleGroup toggleGroup, Transform parent)
        {
            LanguageToggle toggle = Instantiate(_prefab, parent);
            toggle.Initialize(_languageSprites[language], language);
            toggle.Link(toggleGroup);
            
            LanguageTogglePresenter presenter = toggle.GetComponent<LanguageTogglePresenter>();
            await presenter.InitializeAsync();
            
            if (_localizationSystem.CurrentLanguage == language)
                toggle.Select();

            return toggle;
        }
        
        private async UniTask LoadAssetsAsync()
        {
            GameObject toggleObject = await _addressablesService
                .LoadAsync<GameObject>(_assetsConfig.LanguageToggleReference);
            
            _prefab = toggleObject.GetComponent<LanguageToggle>();

            foreach (LocalizeRegionsConfig.LocalizeRegionData regionData in _languagesConfig.Regions)
            {
                Sprite languageSprite = await _addressablesService.LoadAsync<Sprite>(regionData.SpriteRef);
                _languageSprites[regionData.Language] = languageSprite;
            }
        }
    }
}