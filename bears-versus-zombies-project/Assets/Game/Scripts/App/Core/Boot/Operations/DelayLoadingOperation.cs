using System;
using Cysharp.Threading.Tasks;
using Modules.LoadingTree;

namespace SampleGame.App
{
    public sealed class DelayLoadingOperation : LoadingOperationBase
    {
        private const float MaxProgress = 1f;
        private readonly float _delay;

        public DelayLoadingOperation(float delay)
        {
            _delay = delay;
        }

        protected override async UniTask<LoadingResult> RunInternal(LoadingBundle bundle)
        {
            SetProgress(MaxProgress);
            await UniTask.Delay(TimeSpan.FromSeconds(_delay));
            
            return LoadingResult.Success();
        }
    }
}