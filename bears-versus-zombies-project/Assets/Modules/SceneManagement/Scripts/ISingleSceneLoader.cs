using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace Modules.SceneManagement
{
    public interface ISingleSceneLoader
    {
        public float GetProgress();
        
        public UniTask LoadAsync(string nextScene);

        public UniTask LoadAsync(AssetReference nextScene);
    }
}