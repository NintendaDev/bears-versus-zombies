using Modules.AssetsManagement.AddressablesOperations;
using Modules.AssetsManagement.StaticData;
using Modules.AudioManagement.Mixer;
using Modules.EventBus;
using Modules.LoadingCurtain;
using Modules.LoadingTree;
using SampleGame.App.SceneManagement;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace SampleGame.App
{
    public sealed class GameInstaller : MonoInstaller
    {
        [SerializeField, Required]
        private StaticDataServiceConfiguration _staticDataConfig;
        
        [SerializeField, Required]
        private AudioMixerConfiguration _audioMixerConfiguration;
        
        [SerializeField, MinValue(30)] private int _targetFrameRate = 60;
        
        [SerializeField, Required, AssetsOnly]
        private LoadingCurtain _loadingCurtainPrefab;
        
        [SerializeField, Required]
        private AssetReference _mainMenuSceneReference;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<TargetFrameRateService>()
                .AsSingle()
                .WithArguments(_targetFrameRate)
                .NonLazy();
            
            Container.Bind<CoroutineRunner>()
                .FromNewComponentOnNewGameObject()
                .AsSingle();
            
            Container.BindInterfacesAndSelfTo<AddressablesService>().AsSingle();
            
            Container.BindInterfacesTo<StaticDataService>()
                .AsSingle()
                .WithArguments(_staticDataConfig);

            Container.BindInterfacesTo<LoadingCurtain>()
                .FromComponentInNewPrefab(_loadingCurtainPrefab)
                .AsSingle();
            
            Container.BindInterfacesTo<LoadingOperationExecutor>().AsSingle();
            Container.BindInterfacesTo<SignalBus>().AsSingle();
            
            NetworkRegionsServiceInstaller.Install(Container);
            GameUnloaderInstaller.Install(Container, _mainMenuSceneReference);
            AudioMixerInstaller.Install(Container);
        }
    }
}