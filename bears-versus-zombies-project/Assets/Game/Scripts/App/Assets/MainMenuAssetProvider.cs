using System;
using Cysharp.Threading.Tasks;
using Modules.AssetsManagement.AddressablesOperations;
using Modules.Localization.Core.Types;
using SampleGame.App.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace SampleGame.App
{
    [CreateAssetMenu(fileName = "MainMenuAssetProvider", menuName = "SampleGame/Assets/MainMenuAssetProvider")]
    public sealed class MainMenuAssetProvider : ScriptableObject, IDisposable
    {
        [SerializeField, Required] private AssetReferenceGameObject _languageToggleReference;
        [SerializeField, Required] private AssetReferenceGameObject _regionToggleReference; 
        [SerializeField, Required] private AssetReferenceGameObject _sessionRowReference;
        [SerializeField] private RegionIconPrefabData[] _regions;
        
        private IAddressablesService _addressablesService;
        private LanguageToggle _languageTogglePrefab;
        private RegionToggle _regionTogglePrefab;
        private SessionRowView _sessionRowPrefab;
        
        public RegionIcon[] RegionIcons { get; private set; }

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
            
            foreach (RegionIconPrefabData regionData in _regions)
                _addressablesService.Release(regionData.SpriteRef);
        }

        public async UniTask InitializeAsync()
        {
            await UniTask.WhenAll(InitLanguageTogglePrefabAsync(),
                InitRegionTogglePrefabAsync(),
                InitSessionRowPrefabAsync(),
                InitRegionsIconsAsync());
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
        
        private async UniTask InitRegionsIconsAsync()
        {
            UniTask<Sprite>[] tasks = new UniTask<Sprite>[_regions.Length];
            int index = 0;
            
            foreach (RegionIconPrefabData regionData in _regions)
            {
                tasks[index] = _addressablesService.LoadAsync<Sprite>(regionData.SpriteRef);
                index++;
            }
            
            Sprite[] sprites = await UniTask.WhenAll(tasks);
            
            RegionIcons = new RegionIcon[_regions.Length];
            index = 0;
            
            foreach (RegionIconPrefabData regionData in _regions)
            {
                RegionIcons[index] = new RegionIcon { Language = regionData.Language, Sprite = sprites[index] };
                index++;
            }
        }
        
        [Serializable]
        public class RegionIconPrefabData
        {
            [SerializeField] private Language _language;
            [SerializeField, Required] private AssetReferenceT<Sprite> _spriteRef;
            
            public Language Language => _language;
            
            public AssetReferenceT<Sprite> SpriteRef => _spriteRef;
        }
        
        public struct RegionIcon
        {
            public Language Language;
            
            public Sprite Sprite;
        }
    }
}