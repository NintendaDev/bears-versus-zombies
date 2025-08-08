using Cysharp.Threading.Tasks;
using Modules.Localization.Core.Systems;
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
        
        private IInstantiator _instantiator;
        private MainMenuAssetProvider _assetProvider;
        private ILocalizationManager _localizationManager;

        [Inject]
        private void Construct( MainMenuAssetProvider mainMenuAssetProvider, IInstantiator instantiator, 
            ILocalizationManager localizationManager)
        {
            _assetProvider = mainMenuAssetProvider;
            _instantiator = instantiator;
            _localizationManager = localizationManager;
        }

        public UniTask InitializeAsync()
        {
            foreach (Transform child in _container)
                Destroy(child.gameObject);

            foreach (MainMenuAssetProvider.RegionIcon regionIcon in _assetProvider.RegionIcons)
            {
                LanguageToggle toggle = _instantiator.InstantiatePrefab(_assetProvider.GetLanguageTogglePrefab(), 
                        _container).GetComponent<LanguageToggle>();
                
                toggle.Initialize(regionIcon.Sprite);
                toggle.Link(_toggleGroup);
                toggle.GetComponent<LanguageTogglePresenter>().InitLanguage(regionIcon.Language);
            
                if (_localizationManager.CurrentLanguage == regionIcon.Language)
                    toggle.Select();
            }
            
            return UniTask.CompletedTask;
        }
    }
}