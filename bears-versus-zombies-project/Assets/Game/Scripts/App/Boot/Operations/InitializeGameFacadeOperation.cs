using Cysharp.Threading.Tasks;
using Modules.LoadingTree;
using UnityEngine;

namespace SampleGame.App
{
    public sealed class InitializeGameFacadeOperation : LoadingOperationBase
    {
        public float GetWeight() => 10;

        protected override async UniTask<LoadingResult> RunInternal(LoadingBundle bundle)
        {
            GameFacade gameFacade = Object.FindAnyObjectByType<GameFacade>();
            
            if (gameFacade == null)
                return LoadingResult.Error($"{nameof(GameFacade)} not found");
            
            await gameFacade.InitializeAsync();
            
            return LoadingResult.Success();
        }
    }
}