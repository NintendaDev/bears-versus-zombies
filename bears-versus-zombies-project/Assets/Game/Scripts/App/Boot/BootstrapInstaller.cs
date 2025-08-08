using Modules.AssetsManagement.AddressablesOperations;
using Modules.AssetsManagement.StaticData;
using Modules.AudioManagement.Mixer;
using Modules.LoadingTree;
using Modules.Localization.Core.Systems;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace SampleGame.App
{
    [CreateAssetMenu(fileName = "BootstrapInstaller", 
        menuName = "SampleGame/Installers/BootstrapInstaller")]
    public sealed class BootstrapInstaller : ScriptableObjectInstaller<BootstrapInstaller>
    {
        [SerializeField, Required] private AssetReference _mainMenuSceneReference;
        
        public override void InstallBindings()
        {
            Container.Bind<ILoadingOperation>()
                .FromMethod(CreateLoadingOperation)
                .AsSingle()
                .WhenInjectedInto<BootstrapLoader>();
            
            Container.Bind<BootstrapLoader>()
                .FromNewComponentOnNewGameObject()
                .AsSingle()
                .NonLazy();
        }
        
        private ILoadingOperation CreateLoadingOperation()
        {
            return new SequenceOperation(weight: 1, 
                new InlineLoadingOperation(Container.Resolve<IStaticDataService>().InitializeAsync),
                CreateInitServicesOperation(),
                Container.Instantiate<SaveLoadingOperation>(),
                new LoadSceneOperation(_mainMenuSceneReference, Container.Resolve<CoroutineRunner>(),
                    weight: 3),
                new SceneLoadingOperation()
            );
        }

        private ILoadingOperation CreateInitServicesOperation()
        {
            return new ParallelOperation(weight: 1,
                
                Container.Instantiate<InitializeGameFacadeOperation>(),
                new InlineLoadingOperation(Container.Resolve<ILocalizationManager>().InitializeAsync),
                new InlineLoadingOperation(Container.Resolve<AddressablesService>().InitializeAsync),
                new InlineLoadingOperation(Container.Resolve<IAudioMixerSystem>().InitializeAsync),
                new InlineLoadingOperation(Container.Resolve<NetworkRegionsService>().InitializeAsync)
            );
        }
    }
}