using System;
using Cysharp.Threading.Tasks;
using Modules.LoadingTree;
using Zenject;

namespace SampleGame.App
{
    public sealed class LoadingOperationRunner : IFixedTickable, ILoadingOperationRunner
    {
        private ILoadingOperation _loadingOperation;
        private bool _isLoaded = true;
        private Action<float> _onUpdateProgress;
        
        public bool CanStartLoad => _isLoaded;

        public void FixedTick()
        {
            if (_isLoaded)
                return;
            
            _onUpdateProgress?.Invoke(_loadingOperation.GetProgress());
        }

        public async UniTask StartLoadAsync(ILoadingOperation loadingOperation, LoadingBundle bundle,
            Action<float> onUpdateProgress)
        {
            if (CanStartLoad == false)
                throw new Exception("Loading operation in progress");
            
            _onUpdateProgress = onUpdateProgress;
            _loadingOperation = loadingOperation;
            _isLoaded = false;
            
            LoadingResult result = await _loadingOperation.Run(bundle);

            if (result.IsSuccess == false)
                throw new Exception($"ERROR LOADING: {result.Message}");

            _onUpdateProgress = null;
            _isLoaded = true;
        }
    }
}