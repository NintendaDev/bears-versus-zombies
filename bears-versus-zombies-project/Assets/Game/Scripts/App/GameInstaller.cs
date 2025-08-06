using Modules.AssetsManagement.AddressablesOperations;
using Modules.AssetsManagement.StaticData;
using Modules.AudioManagement.Mixer;
using Modules.EventBus;
using Modules.LoadingCurtain;
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
        
        [SerializeField] private GameFacadeInstaller.Settings _gameFacadeSettings;
        
        [SerializeField] private LocalizationInstaller.Settings _localizationSettings;
        
        [SerializeField] private SaveSystemInstaller.Settings _saveSystemSettings;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<PerformanceSetter>()
                .AsSingle()
                .WithArguments(_targetFrameRate)
                .NonLazy();
            
            Container.BindInterfacesAndSelfTo<AddressablesService>().AsSingle();
            
            Container.BindInterfacesTo<StaticDataService>()
                .AsSingle()
                .WithArguments(_staticDataConfig);

            Container.BindInterfacesTo<LoadingCurtain>()
                .FromComponentInNewPrefab(_loadingCurtainPrefab)
                .AsSingle();
            
            Container.BindInterfacesTo<LoadingOperationRunner>().AsSingle();
            Container.BindInterfacesTo<SignalBus>().AsSingle();
            
            Container.BindInterfacesTo<GameplayTerminator>()
                .AsSingle()
                .WithArguments(_mainMenuSceneReference);
            
            LocalizationInstaller.Install(Container, _localizationSettings);
            SaveSystemInstaller.Install(Container, _saveSystemSettings);
            
            Container.BindInterfacesTo<AudioMixerSystem>()
                .AsSingle()
                .WithArguments(_audioMixerConfiguration);
            
            GameFacadeInstaller.Install(Container, _gameFacadeSettings);
        }
    }
}