using System;
using Modules.AudioManagement.Mixer;
using Modules.SaveSystem.Repositories;
using Modules.SaveSystem.SaveLoad;
using Modules.SaveSystem.SaveStrategies;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using JsonSerialization = Modules.SaveSystem.Repositories.SerializeStrategies.JsonSerialization;

namespace SampleGame.App
{
    public sealed class SaveSystemInstaller : Installer<SaveSystemInstaller.Settings, SaveSystemInstaller>
    {
        private readonly Settings _settings;

        public SaveSystemInstaller(Settings settings)
        {
            _settings = settings;
        }
        
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<PrefsStorage>()
                .AsSingle()
                .WithArguments(_settings.PrefsKey)
                .WhenInjectedInto<GameRepository>();
            
            Container.BindInterfacesTo<JsonSerialization>()
                .AsSingle()
                .WhenInjectedInto<GameRepository>();
            
            Container.BindInterfacesTo<GameRepository>()
                .AsSingle()
                .WithArguments(_settings.AesPassword)
                .WhenInjectedInto<GameSaveLoader>();
            
            InstallSerializers();
            
            Container.BindInterfacesTo<GameSaveLoader>().AsSingle();

            Container.BindInterfacesTo<SaveLoadController>()
                .AsSingle()
                .WithArguments(_settings.SavePeriodSeconds)
                .NonLazy();
        }

        private void InstallSerializers()
        {
            Container.BindInterfacesTo<LocalizationSystemSerializer>().AsSingle();
            Container.BindInterfacesTo<AudioMixerSystemsSerializer>().AsSingle();
        }
        
        [Serializable]
        public class Settings
        {
            [SerializeField] private string _aesPassword = "5b54ff5f-f526-407d-9d13-4c5f82727d64";
        
            [SerializeField] private string _prefsKey = "GameSave";

            [SerializeField, MinValue(0.1)] private float _savePeriodSeconds = 1f;
            
            public string AesPassword => _aesPassword;
        
            public string PrefsKey => _prefsKey;
        
            public float SavePeriodSeconds => _savePeriodSeconds;
        }
    }
}