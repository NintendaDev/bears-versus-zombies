using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace SampleGame.App
{
    public sealed class GameFacadeInstaller : Installer<GameFacadeInstaller.Settings, GameFacadeInstaller>
    {
        private readonly Settings _settings;

        public GameFacadeInstaller(Settings settings)
        {
            _settings = settings;
        }
        
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<ConnectionTokenServiceMock>()
                .AsSingle()
                .WithArguments(_settings.ServerDelaySeconds);
            
            Container.BindInterfacesAndSelfTo<GameFacade>()
                .FromComponentInNewPrefab(_settings.GameFacadePrefab)
                .AsSingle();
        }

        [Serializable]
        public class Settings
        {
            [SerializeField, MinValue(0)]
            private float _serverDelaySeconds;
            
            [SerializeField, Required, AssetsOnly]
            private GameFacade _gameFacadePrefab;

            public float ServerDelaySeconds => _serverDelaySeconds;
            
            public GameFacade GameFacadePrefab => _gameFacadePrefab;
        }
    }
}