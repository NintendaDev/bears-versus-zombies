using System;
using Cysharp.Threading.Tasks;
using Modules.EventBus;
using Modules.LoadingTree;
using SampleGame.App.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace SampleGame.App
{
    public sealed class MainMenuLoadingOperationInstaller 
        : Installer<MainMenuLoadingOperationInstaller.Settings, MainMenuLoadingOperationInstaller>
    {
        private readonly Settings _settings;

        public MainMenuLoadingOperationInstaller(Settings settings)
        {
            _settings = settings;
        }
        
        public override void InstallBindings()
        {
            Container.Bind<LoadingBundle>().AsSingle();
            
            Container.Bind<ILoadingOperation>()
                .FromMethod(CreateLoadingOperation)
                .AsSingle();
        }

        private ILoadingOperation CreateLoadingOperation()
        {
            return new SequenceOperation(weight: 1,
                
                CreateFactoriesInitOperation(),
                CreatePresentersInitOperation(),
                
                new AsyncLoadingOperation(() =>
                {
                    Container.Resolve<ISignalBus>().Invoke<ShowMainMenuInstanceSignal>();
                    
                    return UniTask.CompletedTask;
                })
            );
        }

        private ILoadingOperation CreateFactoriesInitOperation()
        {
            return new ParallelOperation(weight: 1,
                new AsyncLoadingOperation(Container.Resolve<RegionToggleFactory>().InitializeAsync),
                new AsyncLoadingOperation(Container.Resolve<LanguageToggleFactory>().InitializeAsync),
                new AsyncLoadingOperation(Container.Resolve<SessionButtonFactory>().InitializeAsync));
        }
        
        private ILoadingOperation CreatePresentersInitOperation()
        {
            return new ParallelOperation(weight: 1,
                new AsyncLoadingOperation(_settings.LanguageTogglesListPresenter.InitializeAsync),
                new AsyncLoadingOperation(_settings.RegionTogglesListPresenter.InitializeAsync),
                new AsyncLoadingOperation(_settings.SessionButtonListPresenter.InitializeAsync));
        }

        [Serializable]
        public class Settings
        {
            [Title("Presenters")]
            [SerializeField, Required, SceneObjectsOnly] 
            private LanguageTogglesListPresenter _languageTogglesListPresenter;
        
            [SerializeField, Required, SceneObjectsOnly] 
            private RegionTogglesListPresenter _regionTogglesListPresenter;
        
            [SerializeField, Required, SceneObjectsOnly] 
            private SessionButtonListPresenter _sessionButtonListPresenter;
            
            public LanguageTogglesListPresenter LanguageTogglesListPresenter => _languageTogglesListPresenter;
            
            public RegionTogglesListPresenter RegionTogglesListPresenter => _regionTogglesListPresenter;
            
            public SessionButtonListPresenter SessionButtonListPresenter => _sessionButtonListPresenter;
        }
    }
}