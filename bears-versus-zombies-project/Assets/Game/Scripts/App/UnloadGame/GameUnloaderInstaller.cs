using Modules.LoadingTree;
using UnityEngine.AddressableAssets;
using Zenject;

namespace SampleGame.App.SceneManagement
{
    public sealed class GameUnloaderInstaller : Installer<AssetReference, GameUnloaderInstaller>
    {
        private readonly AssetReference _mainMenuSceneReference;

        public GameUnloaderInstaller(AssetReference mainMenuSceneReference)
        {
            _mainMenuSceneReference = mainMenuSceneReference;
        }
        
        public override void InstallBindings()
        {
            Container.Bind<ILoadingOperation>()
                .FromMethod(CreateLoadingOperation)
                .AsSingle()
                .WhenInjectedInto<GameplayUnloader>();
            
            Container.BindInterfacesTo<GameplayUnloader>().AsSingle();
        }
        
        private ILoadingOperation CreateLoadingOperation()
        {
            return new SequenceOperation(weight: 1, 
                new NetworkRunnerShutdownOperation(Container.Resolve<GameFacade>()), 
                new LoadSceneOperation(_mainMenuSceneReference, Container.Resolve<CoroutineRunner>(), 
                    weight: 3),
                new SceneLoadingOperation()
            );
        }
    }
}