using Cysharp.Threading.Tasks;
using Modules.EventBus;
using Modules.LoadingTree;
using SampleGame.App.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace SampleGame.App
{
    public sealed class MainMenuInstaller : MonoInstaller
    {
        [SerializeField, Required, SceneObjectsOnly] private MainMenuPresenter _mainMenuPresenter;
        [SerializeField, Required, SceneObjectsOnly] private LobbyPresenter _lobbyPresenter;
        
        [SerializeField, Required] private MainMenuAssetProvider _assetProvider;
        
        [SerializeField, Required, SceneObjectsOnly]
        LanguageTogglesListPresenter _languageTogglesListPresenter;
        
        [SerializeField, Required, SceneObjectsOnly]
        RegionTogglesListPresenter _regionTogglesListPresenter;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<MainMenuAssetProvider>()
                .FromInstance(_assetProvider)
                .AsSingle();
            
            Container.QueueForInject(_assetProvider);
            
            Container.BindInstance(_mainMenuPresenter).AsSingle();
            Container.BindInstance(_lobbyPresenter).AsSingle();
            
            Container.BindInterfacesAndSelfTo<AppMenuController>().AsSingle().NonLazy();
            
            Container.Bind<ILoadingOperation>()
                .FromMethod(CreateLoadingOperation)
                .AsSingle()
                .WhenInjectedInto<MainMenuLoader>();
            
            Container.BindInterfacesAndSelfTo<MainMenuLoader>().AsSingle();
        }
        
        private ILoadingOperation CreateLoadingOperation()
        {
            return new SequenceOperation(weight: 1,
                new InlineLoadingOperation(_assetProvider.InitializeAsync),
                
                new ParallelOperation(weight: 1,
                    new InlineLoadingOperation(_languageTogglesListPresenter.InitializeAsync),
                    new InlineLoadingOperation(_regionTogglesListPresenter.InitializeAsync)
                ),
                
                new InlineLoadingOperation(() =>
                {
                    Container.Resolve<ISignalBus>().Invoke<ShowMainMenuInstanceSignal>();
                    
                    return UniTask.CompletedTask;
                })
            );
        }
    }
}