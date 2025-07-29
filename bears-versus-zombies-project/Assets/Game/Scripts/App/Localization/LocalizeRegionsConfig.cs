using System;
using Modules.Localization.Core.Types;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SampleGame.App
{
    [CreateAssetMenu(fileName = "new LocalizeRegionsConfig",
        menuName = "SampleGame/Localization/LocalizeRegionsConfig")]
    public sealed class LocalizeRegionsConfig : ScriptableObject
    {
        [SerializeField] private LocalizeRegionData[] _regions;
        
        public LocalizeRegionData[] Regions => _regions;
        
        [Serializable]
        public class LocalizeRegionData
        {
            [SerializeField] private Language _language;
            [SerializeField, Required] private AssetReferenceT<Sprite> _spriteRef;
            
            public Language Language => _language;
            
            public AssetReferenceT<Sprite> SpriteRef => _spriteRef;
        }
    }
}