using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Modules.AssetsManagement.AddressablesOperations;
using Modules.AssetsManagement.StaticData;
using Modules.Localization.Core.Types;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SampleGame.App.UI
{
    public sealed class LanguageToggleFactory : IDisposable
    {
        private readonly Dictionary<Language, Sprite> _languageSprites = new();
        private readonly IInstantiator _container;
        private readonly IAddressablesService _addressablesService;
        private readonly MainMenuAssets _assetsConfig;
        private readonly LocalizeRegionsConfig _languagesConfig;
        private readonly GameLocalizationSystem _localizationSystem;
        private LanguageToggle _prefab;

        public LanguageToggleFactory(IInstantiator container, IStaticDataService staticDataService, 
            IAddressablesService addressablesService, GameLocalizationSystem gameLocalization)
        {
            _container = container;
            _addressablesService = addressablesService;
            _localizationSystem = gameLocalization;
            _assetsConfig = staticDataService.GetConfiguration<MainMenuAssets>();
            _languagesConfig = staticDataService.GetConfiguration<LocalizeRegionsConfig>();
        }

        public void Dispose()
        {
            _addressablesService.Release(_assetsConfig.LanguageToggleReference);
            
            foreach (LocalizeRegionsConfig.LocalizeRegionData regionData in _languagesConfig.Regions)
                _addressablesService.Release(regionData.SpriteRef);
        }

        public async UniTask InitializeAsync()
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

        public LanguageToggle Create(Language language, ToggleGroup toggleGroup, Transform parent)
        {
            LanguageToggle toggle = _container.InstantiatePrefab(_prefab, parent).GetComponent<LanguageToggle>();
            toggle.Initialize(_languageSprites[language], language);
            toggle.Link(toggleGroup);
            
            LanguageTogglePresenter presenter = toggle.GetComponent<LanguageTogglePresenter>();
            
            if (_localizationSystem.CurrentLanguage == language)
                toggle.Select();

            return toggle;
        }
    }
}