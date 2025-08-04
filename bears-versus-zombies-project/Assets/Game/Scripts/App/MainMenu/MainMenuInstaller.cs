using SampleGame.App.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace SampleGame.App
{
    public sealed class MainMenuInstaller : MonoInstaller
    {
        [SerializeField, Required, SceneObjectsOnly] private MainMenuView _mainMenuView;
        [SerializeField, Required, SceneObjectsOnly] private LobbyMenuView _lobbyMenuView;
        [SerializeField] private MainMenuLoadingOperationInstaller.Settings _loadingOperationSettings;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_mainMenuView).AsSingle();
            Container.BindInstance(_lobbyMenuView).AsSingle();
            Container.BindInterfacesAndSelfTo<RegionToggleFactory>().AsSingle();
            Container.BindInterfacesAndSelfTo<LanguageToggleFactory>().AsSingle();
            Container.BindInterfacesAndSelfTo<SessionButtonFactory>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<AppMenuController>().AsSingle().NonLazy();
            
            MainMenuLoadingOperationInstaller.Install(Container, _loadingOperationSettings);
        }
    }
}