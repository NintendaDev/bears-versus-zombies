using Cysharp.Threading.Tasks;
using Fusion;
using Modules.LoadingTree;

namespace SampleGame.App
{
    public sealed class NetworkRunnerShutdownOperation : ILoadingOperation
    {
        private readonly GameFacade _gameFacade;

        public NetworkRunnerShutdownOperation(GameFacade gameFacade)
        {
            _gameFacade = gameFacade;
        }

        public async UniTask<LoadingResult> RunAsync(LoadingBundle bundle)
        {
            if (_gameFacade.IsRunning == false)
                return LoadingResult.Success();
            
            if (_gameFacade.GameMode == GameMode.Single || _gameFacade.IsClient)
                await _gameFacade.ShutdownGameAsync();
            else
                await _gameFacade.QuitGameAsync();
            
            return LoadingResult.Success();
        }
    }
}