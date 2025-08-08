using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Modules.LoadingTree
{
    public sealed class LoadSceneOperation : ILoadingOperation
    {
        private readonly AssetReference _sceneReference;
        private readonly CoroutineRunner _coroutineRunner;
        private AsyncOperationHandle<SceneInstance> _sceneHandler;
        private readonly float _weight;

        public LoadSceneOperation(AssetReference sceneReference, CoroutineRunner coroutineRunner, float weight)
        {
            _weight = weight;
            _sceneReference = sceneReference;
            _coroutineRunner = coroutineRunner;
        }

        public float GetProgress()
        {
            return (_sceneHandler.IsValid()) ? _sceneHandler.PercentComplete : 0;
        }

        public float GetWeight() => _weight;

        public async UniTask<LoadingResult> RunAsync(LoadingBundle bundle)
        {
            _sceneHandler = Addressables.LoadSceneAsync(_sceneReference);
            
            if (_sceneHandler.IsValid() == false)
                return LoadingResult.Error("Failed to load scene");
            
            await _sceneHandler.ToUniTask(_coroutineRunner);
            
            return LoadingResult.Success();
        }
    }
}