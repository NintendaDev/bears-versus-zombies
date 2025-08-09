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
    [CreateAssetMenu(fileName = "new SaveSystemInstaller", 
        menuName = "SampleGame/Installers/Project/SaveSystemInstaller")]
    public sealed class SaveSystemInstaller : ScriptableObjectInstaller<SaveSystemInstaller>
    {
        [SerializeField] private string _aesPassword = "5b54ff5f-f526-407d-9d13-4c5f82727d64";
        [SerializeField] private string _prefsKey = "GameSave";
        [SerializeField, MinValue(0.1)] private float _savePeriodSeconds = 1f;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<PrefsStorage>()
                .AsSingle()
                .WithArguments(_prefsKey)
                .WhenInjectedInto<GameRepository>();
            
            Container.BindInterfacesTo<JsonSerialization>()
                .AsSingle()
                .WhenInjectedInto<GameRepository>();
            
            Container.BindInterfacesTo<GameRepository>()
                .AsSingle()
                .WithArguments(_aesPassword)
                .WhenInjectedInto<GameSaveLoader>();
            
            Container.BindInterfacesTo<GameSaveLoader>().AsSingle();

            Container.BindInterfacesTo<SaveLoadController>()
                .AsSingle()
                .WithArguments(_savePeriodSeconds)
                .NonLazy();
        }
    }
}