using UnityEngine;
using Zenject;

namespace SampleGame.App
{
    public sealed class BootstrapInstaller : MonoInstaller
    {
        [SerializeField] private GameLoadingOperationsInstaller.Settings _loadingSettings;
        
        public override void InstallBindings()
        {
            GameLoadingOperationsInstaller.Install(Container, _loadingSettings);
            
            Container.Bind<GameLoader>()
                .FromNewComponentOnNewGameObject()
                .AsSingle()
                .NonLazy();
        }
    }
}