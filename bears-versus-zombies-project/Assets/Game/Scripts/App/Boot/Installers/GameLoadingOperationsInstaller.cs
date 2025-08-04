using System;
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
    public sealed class GameLoadingOperationsInstaller : Installer<GameLoadingOperationsInstaller.Settings, 
        GameLoadingOperationsInstaller>
    {
        private readonly Settings _settings;

        public GameLoadingOperationsInstaller(Settings settings)
        {
            _settings = settings;
        }
        
        public override void InstallBindings()
        {
            Container.Bind<ILoadingOperation>()
                .FromMethod(CreateLoadingOperation)
                .AsSingle();
        }
        
        private ILoadingOperation CreateLoadingOperation()
        {
            return new SequenceOperation(weight: 1, 
                new AsyncLoadingOperation(Container.Resolve<IStaticDataService>().InitializeAsync),
                CreateInitServicesOperation(),
                Container.Instantiate<SaveLoadingOperation>(),
                new LoadSceneOperation(_settings.MainMenuSceneReference, weight: 3),
                new SceneContextLoadingOperation()
            );
        }

        private ILoadingOperation CreateInitServicesOperation()
        {
            return new ParallelOperation(weight: 1,
                
                Container.Instantiate<InitializeGameFacadeOperation>(),
                new AsyncLoadingOperation(Container.Resolve<ILocalizationSystem>().InitializeAsync),
                new AsyncLoadingOperation(Container.Resolve<AddressablesService>().InitializeAsync),
                new AsyncLoadingOperation(Container.Resolve<IAudioMixerSystem>().InitializeAsync)
            );
        }

        [Serializable]
        public class Settings
        {
            [SerializeField, Required] private AssetReference _mainMenuSceneReference;
            
            public AssetReference MainMenuSceneReference => _mainMenuSceneReference;
        }
    }
}