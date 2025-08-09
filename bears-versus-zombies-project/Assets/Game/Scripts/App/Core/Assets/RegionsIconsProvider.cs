using System;
using Cysharp.Threading.Tasks;
using Modules.AssetsManagement.AddressablesOperations;
using Modules.Localization.Core.Types;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace SampleGame.App
{
    [CreateAssetMenu(fileName = "new RegionsIconsProvider", menuName = "SampleGame/Assets/RegionsIconsProvider")]
    public sealed class RegionsIconsProvider : ScriptableObject, IDisposable
    {
        [SerializeField] private RegionIconPrefabData[] _regions;
        
        private IAddressablesService _addressablesService;
        
        public RegionIcon[] RegionIcons { get; private set; }
        
        [Inject]
        private void Construct(IAddressablesService addressablesService)
        {
            _addressablesService = addressablesService;
        }
        
        public void Dispose()
        {
            foreach (RegionIconPrefabData regionData in _regions)
                _addressablesService.Release(regionData.SpriteRef);
        }
        
        public async UniTask InitializeAsync()
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