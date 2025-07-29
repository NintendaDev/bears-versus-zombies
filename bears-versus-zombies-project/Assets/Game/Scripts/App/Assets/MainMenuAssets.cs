using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SampleGame.App
{
    [CreateAssetMenu(fileName = "new MainMenuAssets", menuName = "SampleGame/Assets/MainMenuAssets")]
    public sealed class MainMenuAssets : ScriptableObject
    {
        [SerializeField, Required] private AssetReferenceGameObject _languageToggleReference;
        [SerializeField, Required] private AssetReferenceGameObject _regionToggleReference;
        [SerializeField, Required] private AssetReferenceGameObject _sessionButtonReference;
        
        public AssetReferenceGameObject LanguageToggleReference => _languageToggleReference;
        
        public AssetReferenceGameObject RegionToggleReference => _regionToggleReference;
        
        public AssetReferenceGameObject SessionButtonReference => _sessionButtonReference;
    }
}