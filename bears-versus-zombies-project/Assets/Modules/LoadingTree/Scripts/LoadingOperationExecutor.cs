using System;
using Cysharp.Threading.Tasks;
using Modules.LoadingCurtain;
using Zenject;

namespace Modules.LoadingTree
{
    public sealed class LoadingOperationExecutor : IFixedTickable, ILoadingOperationExecutor
    {
        private readonly ILoadingCurtain _loadingCurtain;
        private ILoadingOperation _loadingOperation;
        private bool _isBusy = true;
        private bool _isDisableCurtainLogo;

        public LoadingOperationExecutor(ILoadingCurtain loadingCurtain)
        {
            _loadingCurtain = loadingCurtain;
        }
        
        public bool IsBusy => _isBusy;

        public void FixedTick()
        {
            if (_isBusy)
                return;
            
            _loadingCurtain.UpdateProgress(_loadingOperation.GetProgress());
        }

        public void DisableCurtainLogo()
        {
            _isDisableCurtainLogo = true;
        }

        public async UniTask ExecuteAsync(ILoadingOperation loadingOperation, LoadingBundle bundle)
        {
            if (IsBusy == false)
                throw new Exception("Loading operation in progress");
            
            _loadingCurtain.Show();
            _loadingCurtain.ShowProgress();
            
            if (_isDisableCurtainLogo)
                _loadingCurtain.HideLogo();
            
            _loadingOperation = loadingOperation;
            _isBusy = false;
            
            LoadingResult result = await _loadingOperation.RunAsync(bundle);
            
            if (result.IsError)
                throw new Exception(result.Message);
            
            _isBusy = true;
            
            _loadingCurtain.Hide();
        }
    }
}