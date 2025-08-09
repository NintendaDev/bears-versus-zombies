using Cysharp.Threading.Tasks;
using Modules.LoadingTree;
using UnityEngine;
using Zenject;

namespace SampleGame.App
{
    public sealed class SceneLoadingOperation : ILoadingOperation
    {
        private ILoadingOperation _loadingOperation;
        
        public float GetWeight() => _loadingOperation?.GetWeight() ?? 0;
        
        public float GetProgress() => _loadingOperation?.GetProgress() ?? 0;

        public async UniTask<LoadingResult> RunAsync(LoadingBundle bundle)
        {
            SceneContext sceneContext = Object.FindAnyObjectByType<SceneContext>();
            
            if (sceneContext == null)
                return LoadingResult.Error($"{nameof(SceneContext)} not found");
            
            ISceneLoader sceneLoader = sceneContext.Container.Resolve<ISceneLoader>();
            
            if (sceneLoader == null)
                return LoadingResult.Error($"{nameof(ISceneLoader)} not found in DI container");
            
            return await sceneLoader.LoadAsync();
        }
    }
}