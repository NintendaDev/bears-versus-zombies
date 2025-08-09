using System;
using Cysharp.Threading.Tasks;
using Modules.AssetsManagement.AddressablesOperations;
using SampleGame.App.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace SampleGame.App
{
    [CreateAssetMenu(fileName = "new MainMenuAssetProvider", menuName = "SampleGame/Assets/MainMenuAssetProvider")]
    public sealed class MainMenuAssetProvider : ScriptableObject, IDisposable
    {
        [SerializeField, Required] private AssetReferenceGameObject _languageToggleReference;
        [SerializeField, Required] private AssetReferenceGameObject _regionToggleReference; 
        [SerializeField, Required] private AssetReferenceGameObject _sessionRowReference;
        
        private IAddressablesService _addressablesService;
        private LanguageToggle _languageTogglePrefab;
        private RegionToggle _regionTogglePrefab;
        private SessionRowView _sessionRowPrefab;
        
        [Inject]
        private void Construct(IAddressablesService addressablesService)
        {
            _addressablesService = addressablesService;
        }

        public void Dispose()
        {
            _addressablesService.Release(_languageToggleReference);
            _addressablesService.Release(_regionToggleReference);
            _addressablesService.Release(_sessionRowReference);
        }

        public async UniTask InitializeAsync()
        {
            await UniTask.WhenAll(InitLanguageTogglePrefabAsync(),
                InitRegionTogglePrefabAsync(),
                InitSessionRowPrefabAsync());
        }

        public LanguageToggle GetLanguageTogglePrefab() => _languageTogglePrefab;
        
        public RegionToggle GetRegionTogglePrefab() => _regionTogglePrefab;
        
        public SessionRowView GetSessionRowPrefab() => _sessionRowPrefab;

        private async UniTask InitLanguageTogglePrefabAsync()
        {
            _languageTogglePrefab = await GetPrefabAsync<LanguageToggle>(_languageToggleReference);
        }
        
        private async UniTask<T> GetPrefabAsync<T>(AssetReferenceGameObject reference)
        {
            return (await _addressablesService
                    .LoadByAddressAsync<GameObject>(reference.AssetGUID))
                .GetComponent<T>();
        }
        
        private async UniTask InitRegionTogglePrefabAsync()
        {
            _regionTogglePrefab = await GetPrefabAsync<RegionToggle>(_regionToggleReference);
        }
        
        private async UniTask InitSessionRowPrefabAsync()
        {
            _sessionRowPrefab = await GetPrefabAsync<SessionRowView>(_sessionRowReference);
        }
    }
}