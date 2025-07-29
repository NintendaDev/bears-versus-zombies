using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Modules.AssetsManagement.StaticData;
using Modules.Services;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace SampleGame.App.UI
{
    public sealed class LanguageTogglesListPresenter : MonoBehaviour
    {
        [SerializeField, Required, ChildGameObjectsOnly]
        private Transform _container;

        [SerializeField, Required, ChildGameObjectsOnly]
        private ToggleGroup _toggleGroup;
        
        private LanguageToggleFactory _factory;
        private LocalizeRegionsConfig _localizeRegionsConfig;

        public async UniTask InitializeAsync()
        {
            _factory = ServiceLocator.Instance.Get<LanguageToggleFactory>();
            IStaticDataService staticDataService = ServiceLocator.Instance.Get<IStaticDataService>();
            _localizeRegionsConfig = staticDataService.GetConfiguration<LocalizeRegionsConfig>();
            
            foreach (Transform child in _container)
                Destroy(child.gameObject);

            List<UniTask> createToggleTasks = new();
            
            foreach (LocalizeRegionsConfig.LocalizeRegionData regionData in _localizeRegionsConfig.Regions)
                createToggleTasks.Add(_factory.CreateAsync(regionData.Language, _toggleGroup, _container));
            
            await UniTask.WhenAll(createToggleTasks);
        }
    }
}