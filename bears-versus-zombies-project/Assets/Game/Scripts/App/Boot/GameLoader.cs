using Cysharp.Threading.Tasks;
using Modules.LoadingCurtain;
using Modules.LoadingTree;
using UnityEngine;
using Zenject;

namespace SampleGame.App
{
    public sealed class GameLoader : MonoBehaviour
    {
        private ILoadingCurtain _loadingCurtain;
        private ILoadingOperationRunner _loadingOperationRunner;
        private ILoadingOperation _loadingOperation;
        private bool _isLoaded;

        [Inject]
        private void Construct(ILoadingOperationRunner loadingOperationRunner, 
            ILoadingOperation loadingOperation, ILoadingCurtain loadingCurtain)
        {
            _loadingOperationRunner = loadingOperationRunner;
            _loadingOperation = loadingOperation;
            _loadingCurtain = loadingCurtain;
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
            _loadingCurtain.Show();
            _loadingCurtain.ShowProgress();
            _loadingCurtain.HideLogo();
            
            LoadingBundle bundle = new();
            
            await _loadingOperationRunner.StartLoadAsync(_loadingOperation, bundle, progress =>
            {
                _loadingCurtain.UpdateProgress(progress);
            });

            _loadingCurtain.Hide();
            _isLoaded = true;
        }
    }
}