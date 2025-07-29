using Cysharp.Threading.Tasks;
using Modules.LoadingCurtain;
using Modules.LoadingTree;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SampleGame.App.SceneManagement
{
    public sealed class GameplayToMenuLoader : MonoBehaviour
    {
        [SerializeField, Required] private GameFacade _gameFacade;
        [SerializeField, Required] private LoadingOperationRunner _loadingOperationRunner;
        [SerializeField, Required] private AssetReference _mainMenuSceneReference;
        [SerializeField, Required] private LoadingCurtain _loadingCurtain;

        public async UniTask StartLoadingAsync()
        {
            _loadingCurtain.Show();
            _loadingCurtain.ShowProgress();
            
            LoadingBundle bundle = new();
            ILoadingOperation loadingOperation = CreateLoadingOperation();

            await _loadingOperationRunner.StartLoadAsync(loadingOperation, bundle, progress =>
            {
                _loadingCurtain.UpdateProgress(progress);
            });

            _loadingCurtain.Hide();
        }
        
        private ILoadingOperation CreateLoadingOperation()
        {
            return new SequenceOperation(weight: 1, 
                new NetworkRunnerShutdownOperation(_gameFacade), 
                new LoadSceneOperation(_mainMenuSceneReference, weight: 3),
                new SceneInitializerLoadingOperation()
            );
        }
    }
}