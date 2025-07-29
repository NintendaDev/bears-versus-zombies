using Cysharp.Threading.Tasks;
using Modules.LoadingTree;

namespace SampleGame.App
{
    public sealed class AsyncLoadingOperation : LoadingOperationBase
    {
        private readonly LoadingOperation _operation;

        public AsyncLoadingOperation(LoadingOperation operation)
        {
            _operation = operation;
        }

        public delegate UniTask LoadingOperation();

        protected override async UniTask<LoadingResult> RunInternal(LoadingBundle bundle)
        {
            await _operation.Invoke();
            
            return LoadingResult.Success();
        }
    }
}