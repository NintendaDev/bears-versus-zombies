using System;
using System.Collections.Generic;
using Fusion;
using R3;
using SampleGame.App;
using UnityEngine;
using Zenject;

namespace SampleGame.Gameplay.Context
{
    public sealed class GameContext : MonoBehaviour, IGameContext
    {
        private readonly Dictionary<Type, object> _services = new();
        private GameFacade _gameFacade;
        
        public static IGameContext Instance { get; private set; }
        
        private NetworkRunner Runner { get; set; }

        [Inject]
        private void Construct(GameFacade gameFacade)
        {
            Instance = this;
            _gameFacade = gameFacade;
            InitRunner();

            _gameFacade.StateChanged
                .Subscribe((_) => InitRunner())
                .AddTo(this);
        }

        public void Add(SimulationBehaviour service)
        {
            if (service == null)
                throw new ArgumentNullException($"Serice is null");
            
            Type serviceType = service.GetType();
            _services[serviceType] = service;
        }
        
        public T Get<T>() where T : SimulationBehaviour
        {
            Type serviceType = typeof(T);
            
            if (_services.ContainsKey(serviceType))
                return (T)_services[serviceType];
            
            if (Runner.HasSingleton<T>() == false)
                throw new Exception($"Service {typeof(T)} not found");
            
            return Runner.GetSingleton<T>();
        }
        
        private void InitRunner() => Runner = FindAnyObjectByType<NetworkRunner>();
    }
}