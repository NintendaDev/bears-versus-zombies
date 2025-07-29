using System;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.Services
{
    public sealed class ServiceLocator : MonoBehaviour, IServiceLocator
    {
        private readonly Dictionary<Type, object> _services = new();

        public static IServiceLocator Instance { get; private set; }

        public void Initialize()
        {
            Instance = this;
        }

        public void Add(object service)
        {
            _services.Add(service.GetType(), service);
        }

        public void Add<T>(T service)
        {
            if (service == null)
                throw new ArgumentNullException($"Serice is null");
            
            Type serviceType = typeof(T);

            if (_services.ContainsKey(serviceType))
                _services[serviceType] = service;
            else
                _services.Add(typeof(T), service);
        }
        
        public T Get<T>()
        {
            Type serviceType = typeof(T);
            
            if (_services.ContainsKey(serviceType) == false)
                throw new Exception($"Service {serviceType.Name} not found");
            
            return (T)_services[serviceType];
        }
    }
}