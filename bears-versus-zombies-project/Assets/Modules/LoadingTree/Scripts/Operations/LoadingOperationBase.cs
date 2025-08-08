using Cysharp.Threading.Tasks;

namespace Modules.LoadingTree
{
    public abstract class LoadingOperationBase : ILoadingOperation
    {
        private float _progress;
        
        public float GetProgress() => _progress;
        
        public async UniTask<LoadingResult> RunAsync(LoadingBundle bundle)
        {
            LoadingResult result = await RunInternal(bundle);
            _progress = 1;
            
            return result;
        }

        protected abstract UniTask<LoadingResult> RunInternal(LoadingBundle bundle);
    }
}