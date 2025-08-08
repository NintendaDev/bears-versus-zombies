using Fusion;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SampleGame.App
{
    [CreateAssetMenu(fileName = "new GameFacadeConfig", menuName = "SampleGame/GameFacadeConfig")]
    public sealed class GameFacadeConfig : ScriptableObject
    {
        public const string SingleSessionName = "SingleSession";
        public const int SinglePlayersCount = 1;
        public const int MultiPlayersCount = 2;
        
        [SerializeField, Required, AssetsOnly] private NetworkRunner _networkRunnerPrefab;
        [SerializeField, Required] private string _levelSceneAddressablesPath = "Assets/Game/Scenes/GameLevel.unity";
        [SerializeField, Required] private AssetReference _levelSceneReference;
        
        public NetworkRunner NetworkRunnerPrefab => _networkRunnerPrefab;
        
        public string LevelSceneAddressablesPath => _levelSceneAddressablesPath;
        
        public AssetReference LevelSceneReference => _levelSceneReference;
    }
}