using Cysharp.Threading.Tasks;

namespace Modules.LoadingTree
{
    public interface ILoadingOperationExecutor
    {
        public bool IsBusy { get; }

        public void DisableCurtainLogo();

        public UniTask ExecuteAsync(ILoadingOperation loadingOperation, LoadingBundle bundle);
    }
}