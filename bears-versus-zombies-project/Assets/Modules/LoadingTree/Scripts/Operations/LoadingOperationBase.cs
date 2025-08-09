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
        
        protected void SetProgress(float progress)
        {
            if (progress < 0 || progress > 1)
                throw new System.ArgumentException("progress must be between 0 and 1");
            
            _progress = progress;
        }
    }
}