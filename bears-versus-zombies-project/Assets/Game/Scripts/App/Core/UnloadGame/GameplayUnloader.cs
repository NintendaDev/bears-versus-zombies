using Cysharp.Threading.Tasks;
using Modules.LoadingTree;

namespace SampleGame.App.SceneManagement
{
    public sealed class GameplayUnloader : IGameplayUnloader
    {
        private readonly ILoadingOperation _loadingOperation;
        private readonly ILoadingOperationExecutor _loadingOperationExecutor;

        public GameplayUnloader(ILoadingOperation loadingOperation, ILoadingOperationExecutor loadingOperationExecutor)
        {
            _loadingOperation = loadingOperation;
            _loadingOperationExecutor = loadingOperationExecutor;
        }

        public async UniTask UnloadAsync()
        {
            LoadingBundle bundle = new();
            await _loadingOperationExecutor.ExecuteAsync(_loadingOperation, bundle);
        }
    }
}