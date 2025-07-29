using System.Collections.Generic;
using Fusion;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.GameContext
{
    public sealed class PoolableNetworkObjectProvider : NetworkObjectProviderDefault
    {
        [SerializeField, Required] private PoolableNetworkObjectCatalog _catalog;
        
        private Transform _objectsRoot;
        private Dictionary<string, Queue<NetworkObject>> _pool;

        private void Awake()
        {
            if (_objectsRoot == null)
                _objectsRoot = transform;
            
            IReadOnlyList<string> prefabs = _catalog.PoolPrefabsNames;
            int count = prefabs.Count;

            _pool = new Dictionary<string, Queue<NetworkObject>>(count);
            
            for (int i = 0; i < count; i++)
                _pool.Add(_catalog.PoolPrefabsNames[i], new Queue<NetworkObject>());
        }

        public void SetObjectsRoot(Transform objectsRoot)
        {
            _objectsRoot = objectsRoot;
        }

        protected override NetworkObject InstantiatePrefab(NetworkRunner runner, NetworkObject prefab)
        {
            string prefabName = prefab.name;
            
            if (_catalog == null || _catalog.IsPooledPrefab(prefabName) == false)
                return base.InstantiatePrefab(runner, prefab);

            Queue<NetworkObject> queue = _pool[prefabName];

            if (queue.TryDequeue(out NetworkObject instance))
            {
                instance.gameObject.SetActive(true);
            }
            else
            {
                instance = Instantiate(prefab);
            }

            instance.name = prefab.name;
            
            return instance;
        }

        protected override void DestroyPrefabInstance(NetworkRunner runner, NetworkPrefabId prefabId, 
            NetworkObject instance)
        {
            string prefabName = instance.name;
            
            if (_catalog.IsPooledPrefab(prefabName) == false)
            {
                base.DestroyPrefabInstance(runner, prefabId, instance);
                
                return;
            }

            Queue<NetworkObject> queue = _pool[prefabName];
            queue.Enqueue(instance);

            instance.gameObject.SetActive(false);
            instance.transform.parent = _objectsRoot;
        }
    }
}