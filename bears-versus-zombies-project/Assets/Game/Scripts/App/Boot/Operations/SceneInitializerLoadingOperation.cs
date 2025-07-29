using Cysharp.Threading.Tasks;
using Modules.LoadingTree;
using UnityEngine;

namespace SampleGame.App
{
    public sealed class SceneInitializerLoadingOperation : LoadingOperationBase
    {
        public float GetWeight() => 3;

        protected override async UniTask<LoadingResult> RunInternal(LoadingBundle bundle)
        {
            SceneInitializer sceneInitializer = Object.FindAnyObjectByType<SceneInitializer>();
            
            if (sceneInitializer == null)
                return LoadingResult.Error($"{nameof(SceneInitializer)} not found");
            
            await sceneInitializer.InitializeAsync();

            return LoadingResult.Success();
        }
    }
}