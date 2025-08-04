using Cysharp.Threading.Tasks;
using Modules.LoadingTree;

namespace SampleGame.App
{
    public sealed class InitializeGameFacadeOperation : LoadingOperationBase
    {
        private readonly GameFacade _gameFacade;

        public InitializeGameFacadeOperation(GameFacade gameFacade)
        {
            _gameFacade = gameFacade;
        }
        
        public float GetWeight() => 10;

        protected override async UniTask<LoadingResult> RunInternal(LoadingBundle bundle)
        {
            await _gameFacade.InitializeAsync();
            
            return LoadingResult.Success();
        }
    }
}