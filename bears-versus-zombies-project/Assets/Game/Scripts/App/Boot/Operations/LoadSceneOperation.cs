using Cysharp.Threading.Tasks;
using Modules.LoadingTree;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace SampleGame.App
{
    public sealed class LoadSceneOperation : ILoadingOperation
    {
        private readonly AssetReference _sceneReference;
        private AsyncOperationHandle<SceneInstance> _sceneHandler;
        private readonly float _weight;

        public LoadSceneOperation(AssetReference sceneReference, float weight)
        {
            _weight = weight;
            _sceneReference = sceneReference;
        }

        public float GetProgress()
        {
            return (_sceneHandler.IsValid()) ? _sceneHandler.PercentComplete : 0;
        }

        public float GetWeight() => _weight;

        public async UniTask<LoadingResult> Run(LoadingBundle bundle)
        {
            _sceneHandler = Addressables.LoadSceneAsync(_sceneReference);
            
            if (_sceneHandler.IsValid() == false)
                return LoadingResult.Error("Failed to load scene");
            
            await _sceneHandler.ToUniTask();
            
            return LoadingResult.Success();
        }
    }
}