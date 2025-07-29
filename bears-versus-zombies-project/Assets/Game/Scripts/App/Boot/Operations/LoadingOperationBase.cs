using Cysharp.Threading.Tasks;
using Modules.LoadingTree;

namespace SampleGame.App
{
    public abstract class LoadingOperationBase : ILoadingOperation
    {
        private float _progress;
        
        public float GetProgress() => _progress;
        
        public async UniTask<LoadingResult> Run(LoadingBundle bundle)
        {
            LoadingResult result = await RunInternal(bundle);
            _progress = 1;
            
            return result;
        }

        protected abstract UniTask<LoadingResult> RunInternal(LoadingBundle bundle);
    }
}