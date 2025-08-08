using Cysharp.Threading.Tasks;
using Modules.AssetsManagement.StaticData;
using Modules.LoadingTree;

namespace SampleGame.App
{
    public sealed class MainMenuLoader : ISceneLoader
    {
        private readonly ILoadingOperation _loadingOperation;

        public MainMenuLoader(ILoadingOperation loadingOperation, IStaticDataService staticDataService)
        {
            _loadingOperation = loadingOperation;
        }

        public async UniTask<LoadingResult> LoadAsync()
        {
            LoadingBundle bundle = new();
            return await _loadingOperation.RunAsync(bundle);
        }
    }
}