using Cysharp.Threading.Tasks;
using Modules.LoadingTree;
using UnityEngine;
using Zenject;

namespace SampleGame.App
{
    public sealed class BootstrapLoader : MonoBehaviour
    {
        private ILoadingOperation _loadingOperation;
        private bool _isLoaded;
        private ILoadingOperationExecutor _loadingOperationExecutor;

        [Inject]
        private void Construct(ILoadingOperation loadingOperation, ILoadingOperationExecutor loadingOperationExecutor)
        {
            _loadingOperation = loadingOperation;
            _loadingOperationExecutor = loadingOperationExecutor;
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (_isLoaded)
                return;
            
            StartLoading().Forget();
        }

        private async UniTaskVoid StartLoading()
        {
            LoadingBundle bundle = new();
            _loadingOperationExecutor.DisableCurtainLogo();
            await _loadingOperationExecutor.ExecuteAsync(_loadingOperation, bundle);
            _isLoaded = true;
        }
    }
}