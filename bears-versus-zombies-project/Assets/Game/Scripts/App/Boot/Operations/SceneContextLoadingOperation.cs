using Cysharp.Threading.Tasks;
using Modules.LoadingTree;
using UnityEngine;
using Zenject;

namespace SampleGame.App
{
    public sealed class SceneContextLoadingOperation : ILoadingOperation
    {
        private ILoadingOperation _loadingOperation;
        
        public float GetWeight() => _loadingOperation?.GetWeight() ?? 0;
        
        public float GetProgress() => _loadingOperation?.GetProgress() ?? 0;

        public async UniTask<LoadingResult> Run(LoadingBundle bundle)
        {
            SceneContext sceneContext = Object.FindAnyObjectByType<SceneContext>();
            
            if (sceneContext == null)
                return LoadingResult.Error($"{nameof(SceneContext)} not found");
            
            _loadingOperation = sceneContext.Container.Resolve<ILoadingOperation>();
            
            if (_loadingOperation == null)
                return LoadingResult.Error($"{nameof(ILoadingOperation)} not found in DI container");
            
            LoadingBundle loadingBundle = sceneContext.Container.Resolve<LoadingBundle>();

            if (loadingBundle == null)
                loadingBundle = new LoadingBundle();
            
            return await _loadingOperation.Run(loadingBundle);
        }
    }
}