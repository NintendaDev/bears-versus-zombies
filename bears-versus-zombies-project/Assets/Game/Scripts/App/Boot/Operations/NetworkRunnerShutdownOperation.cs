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

        public async UniTask<LoadingResult> Run(LoadingBundle bundle)
        {
            if (_gameFacade.Runner.IsRunning == false || _gameFacade.Runner == null)
                return LoadingResult.Success();
            
            if (_gameFacade.Runner.GameMode == GameMode.Single || _gameFacade.Runner.IsClient)
                await _gameFacade.ShutdownGameAsync();
            else
                await _gameFacade.QuitGameAsync();
            
            return LoadingResult.Success();
        }
    }
}