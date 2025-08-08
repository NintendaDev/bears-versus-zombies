using System;
using Cysharp.Threading.Tasks;

namespace Modules.LoadingTree
{
    public sealed class InlineLoadingOperation : LoadingOperationBase
    {
        private readonly Func<UniTask> _operation;

        public InlineLoadingOperation(Func<UniTask> operation)
        {
            _operation = operation;
        }

        protected override async UniTask<LoadingResult> RunInternal(LoadingBundle bundle)
        {
            await _operation.Invoke();
            
            return LoadingResult.Success();
        }
    }
}