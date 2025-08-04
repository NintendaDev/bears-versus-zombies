using Cysharp.Threading.Tasks;
using Modules.LoadingCurtain;
using Modules.LoadingTree;
using UnityEngine.AddressableAssets;

namespace SampleGame.App.SceneManagement
{
    public sealed class GameplayTerminator : IGameplayTerminator
    {
        private readonly GameFacade _gameFacade;
        private readonly ILoadingOperationRunner _loadingOperationRunner;
        private readonly AssetReference _mainMenuSceneReference;
        private readonly ILoadingCurtain _loadingCurtain;

        public GameplayTerminator(GameFacade gameFacade, ILoadingOperationRunner loadingOperationRunner,
            ILoadingCurtain loadingCurtain, AssetReference mainMenuSceneReference)
        {
            _gameFacade = gameFacade;
            _loadingOperationRunner = loadingOperationRunner;
            _loadingCurtain = loadingCurtain;
            _mainMenuSceneReference = mainMenuSceneReference;
        }

        public async UniTask TerminateAsync()
        {
            _loadingCurtain.Show();
            _loadingCurtain.ShowProgress();
            
            LoadingBundle bundle = new();
            ILoadingOperation loadingOperation = CreateLoadingOperation();

            await _loadingOperationRunner.StartLoadAsync(loadingOperation, bundle, progress =>
            {
                _loadingCurtain.UpdateProgress(progress);
            });

            _loadingCurtain.Hide();
        }
        
        private ILoadingOperation CreateLoadingOperation()
        {
            return new SequenceOperation(weight: 1, 
                new NetworkRunnerShutdownOperation(_gameFacade), 
                new LoadSceneOperation(_mainMenuSceneReference, weight: 3),
                new SceneContextLoadingOperation()
            );
        }
    }
}