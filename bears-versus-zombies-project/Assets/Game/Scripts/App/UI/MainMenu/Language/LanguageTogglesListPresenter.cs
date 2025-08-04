using Cysharp.Threading.Tasks;
using Modules.AssetsManagement.StaticData;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

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

        [Inject]
        private void Construct(LanguageToggleFactory languageToggleFactory, IStaticDataService staticDataService)
        {
            _factory = languageToggleFactory;
            _localizeRegionsConfig = staticDataService.GetConfiguration<LocalizeRegionsConfig>();
        }

        public UniTask InitializeAsync()
        {
            foreach (Transform child in _container)
                Destroy(child.gameObject);
            
            foreach (LocalizeRegionsConfig.LocalizeRegionData regionData in _localizeRegionsConfig.Regions)
                _factory.Create(regionData.Language, _toggleGroup, _container);
            
            return UniTask.CompletedTask;
        }
    }
}