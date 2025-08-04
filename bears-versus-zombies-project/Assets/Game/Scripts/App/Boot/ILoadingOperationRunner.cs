using System;
using Cysharp.Threading.Tasks;
using Modules.LoadingTree;

namespace SampleGame.App
{
    public interface ILoadingOperationRunner
    {
        public bool CanStartLoad { get; }

        public UniTask StartLoadAsync(ILoadingOperation loadingOperation, LoadingBundle bundle,
            Action<float> onUpdateProgress);
    }
}