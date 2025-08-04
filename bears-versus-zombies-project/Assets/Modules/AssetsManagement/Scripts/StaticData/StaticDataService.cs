using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Modules.AssetsManagement.AddressablesOperations;
using UnityEngine;

namespace Modules.AssetsManagement.StaticData
{
    public class StaticDataService : IStaticDataService
    {
        private readonly AddressablesService _addressablesService;
        private readonly StaticDataServiceConfiguration _configuration;
        
        private readonly Dictionary<Type, UnityEngine.Object> _configurations = new();

        public StaticDataService(StaticDataServiceConfiguration configuration, AddressablesService addressablesService)
        {
            _addressablesService = addressablesService;
            _configuration = configuration;
        }

        public async UniTask InitializeAsync() => await LoadAllConfigurations();
        
        public TConfigType GetConfiguration<TConfigType>() where TConfigType : ScriptableObject
        {
            Type configurationType = typeof(TConfigType);

            if (_configurations.ContainsKey(configurationType) == false)
                throw new Exception($"Configuration type {configurationType} is not exists");

            return _configurations[configurationType] as TConfigType;
        }

        private async UniTask LoadAllConfigurations()
        {
            ScriptableObject[] rawConfigurations = await _addressablesService
                .LoadByLabelAsync<ScriptableObject>(_configuration.ConfigurationsAssetLabel);

            foreach (ScriptableObject rawConfiguration in rawConfigurations)
            {
                Type configType = rawConfiguration.GetType();
                _configurations.Add(configType, rawConfiguration);
                Debug.Log($"Loaded configuration: {configType}");
            }
        }
    }
}