using Cysharp.Threading.Tasks;
using Modules.AssetsManagement.AddressablesOperations;
using Modules.AssetsManagement.StaticData;
using Modules.AudioManagement.Mixer;
using Modules.LoadingCurtain;
using Modules.LoadingTree;
using Modules.Localization.Core.Systems;
using Modules.SaveSystem.SaveLoad;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SampleGame.App
{
    public sealed class GameLoader : MonoBehaviour
    {
        [SerializeField, Required] private AssetReference _mainMenuSceneReference;
        [SerializeField, Required] private LoadingCurtain _loadingCurtain;
        [SerializeField, Required] private LoadingOperationRunner _loadingOperationRunner;
        
        [Title("Initializable Services")]
        [SerializeField, Required] private StaticDataService _staticDataService;
        [SerializeField, Required] private LocalizationSystem _localizationSystem;
        [SerializeField, Required] private AddressablesService _addressablesService;
        [SerializeField, Required] private GameSaveLoader _gameSaveLoader;
        [SerializeField, Required] private AudioMixerSystem _audioMixerSystem;
        
        private bool _isLoaded;

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
            ILoadingOperation loadingOperation = CreateLoadingOperation();

            await _loadingOperationRunner.StartLoadAsync(loadingOperation, bundle, progress =>
            {
                _loadingCurtain.UpdateProgress(progress);
            });

            _loadingCurtain.Hide();
            _isLoaded = true;
        }
        
        private ILoadingOperation CreateLoadingOperation()
        {
            return new SequenceOperation(weight: 1, 
                new AsyncLoadingOperation(_staticDataService.InitializeAsync),
                CreateInitServicesOperation(),
                new SaveLoadingOperation(_gameSaveLoader),
                new LoadSceneOperation(_mainMenuSceneReference, weight: 3),
                new SceneInitializerLoadingOperation()
            );
        }

        private ILoadingOperation CreateInitServicesOperation()
        {
            return new ParallelOperation(weight: 1,
                
                new InitializeGameFacadeOperation(),
                new AsyncLoadingOperation(_localizationSystem.InitializeAsync),
                new AsyncLoadingOperation(_addressablesService.InitializeAsync),
                new AsyncLoadingOperation(_gameSaveLoader.InitializeAsync),
                new AsyncLoadingOperation(_audioMixerSystem.InitializeAsync)
            );
        }
    }
}