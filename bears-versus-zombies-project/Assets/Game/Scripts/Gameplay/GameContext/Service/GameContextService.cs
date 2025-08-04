using System;
using System.Collections.Generic;
using Fusion;
using SampleGame.App;
using UnityEngine;
using Zenject;

namespace SampleGame.Gameplay.GameContext
{
    public sealed class GameContextService : MonoBehaviour, IGameContext
    {
        private readonly Dictionary<Type, object> _services = new();
        private GameFacade _gameFacade;

        public static IGameContext Instance { get; private set; }
        
        private NetworkRunner Runner => _gameFacade.Runner;

        [Inject]
        private void Construct(GameFacade gameFacade)
        {
            _gameFacade = gameFacade;
            Instance = this;
        }

        public void Add(SimulationBehaviour service)
        {
            if (service == null)
                throw new ArgumentNullException($"Serice is null");
            
            Type serviceType = service.GetType();

            if (_services.ContainsKey(serviceType))
                _services[serviceType] = service;
            else
                _services.Add(serviceType, service);
        }
        
        public T Get<T>() where T : SimulationBehaviour
        {
            Type serviceType = typeof(T);
            
            if (_services.ContainsKey(serviceType))
                return (T)_services[serviceType];
            
            if (Runner.HasSingleton<T>() == false)
                throw new Exception($"Service {serviceType.Name} not found");
            
            return Runner.GetSingleton<T>();
        }
    }
}