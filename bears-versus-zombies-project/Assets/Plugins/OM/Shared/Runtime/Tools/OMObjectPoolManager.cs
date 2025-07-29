using System.Collections.Generic;
using UnityEngine;

namespace OM
{
    /// <summary>
    /// Manages object pools for efficient instantiation and reuse of GameObjects.
    /// </summary>
    public class OMObjectPoolManager : MonoBehaviour
    {
        /// <summary>
        /// Represents a single object pool item with its configuration.
        /// </summary>
        [System.Serializable]
        public class OMObjectPoolItem
        {
            public string key;
            public bool rename = true;
            public GameObject prefab;
            public bool useIPool = false;
            public int preload = 0;
            public int maxCapacity = 10;

            public OMObjectPool<GameObject> Pool { get; private set; }

            /// <summary>
            /// Sets up the object pool item with the specified parent transform.
            /// </summary>
            /// <param name="parent"></param>
            public void Setup(Transform parent)
            {
                Pool = new OMObjectPool<GameObject>(maxCapacity, preload, useIPool, () =>
                {
                    var obj = Instantiate(prefab, parent);
                    obj.gameObject.SetActive(false);
                    if (rename)
                    {
                        obj.name += $" _{Pool.GetSpawnedCount}";
                    }
                    return obj;
                },
                obj => { obj.SetActive(true); },
                obj =>
                {
                    obj.SetActive(false);
                    obj.transform.SetParent(parent, true);
                });
            }
        }

        /// <summary>
        /// Singleton instance of the OMObjectPoolManager.
        /// </summary>
        public static OMObjectPoolManager Instance { get; private set; }

        /// <summary>
        /// array of object pool items to be managed.
        /// </summary>
        [SerializeField] private OMObjectPoolItem[] items;
        
        /// <summary>
        /// Dictionary to hold the object pools by their keys.
        /// </summary>
        private readonly Dictionary<string, OMObjectPool<GameObject>> _dictionary = new();

        /// <summary>
        /// Initializes the object pool manager and sets up the pools.
        /// </summary>
        private void Awake()
        {
            Instance = this;
            foreach (var item in items)
            {
                var parent = new GameObject($"Pool ({item.key})");
                parent.transform.SetParent(transform, true);
                item.Setup(parent.transform);
                _dictionary.TryAdd(item.key, item.Pool);
            }
        }

        /// <summary>
        /// Gets the object pool associated with the specified key.
        /// </summary>
        public OMObjectPool<GameObject> GetPoolByKey(string key)
        {
            if (_dictionary.TryGetValue(key, out var pool))
            {
                return pool;
            }

            Debug.LogError($"[OMObjectPoolManager] No pool found with key: {key}");
            return null;
        }

        /// <summary>
        /// Spawns an object from the pool associated with the specified key.
        /// </summary>
        public GameObject Spawn(string key)
        {
            return GetPoolByKey(key)?.Spawn();
        }

        /// <summary>
        /// Spawns an object from the pool and sets its position.
        /// </summary>
        public GameObject Spawn(string key, Vector3 pos)
        {
            var o = Spawn(key);
            if (o != null) o.transform.position = pos;
            return o;
        }

        /// <summary>
        /// Spawns an object from the pool and sets its position and rotation.
        /// </summary>
        public GameObject Spawn(string key, Vector3 pos, Quaternion rot)
        {
            var o = Spawn(key);
            if (o != null) o.transform.SetPositionAndRotation(pos, rot);
            return o;
        }

        /// <summary>
        /// Spawns an object from the pool and sets its position, rotation, and parent.
        /// </summary>
        public GameObject Spawn(string key, Vector3 pos, Quaternion rot, Transform parent)
        {
            var o = Spawn(key);
            if (o != null)
            {
                o.transform.SetPositionAndRotation(pos, rot);
                o.transform.SetParent(parent, true);
            }
            return o;
        }

        /// <summary>
        /// Spawns an object from the pool and returns its component of type T.
        /// </summary>
        public T Spawn<T>(string key)
        {
            var o = Spawn(key);
            return o ? o.GetComponent<T>() : default;
        }

        /// <summary>
        /// Spawns an object from the pool, sets its position, and returns its component of type T.
        /// </summary>
        public T Spawn<T>(string key, Vector3 pos)
        {
            var o = Spawn(key, pos);
            return o ? o.GetComponent<T>() : default;
        }

        /// <summary>
        /// Spawns an object from the pool, sets its position and rotation, and returns its component of type T.
        /// </summary>
        public T Spawn<T>(string key, Vector3 pos, Quaternion rot)
        {
            var o = Spawn(key, pos, rot);
            return o ? o.GetComponent<T>() : default;
        }

        /// <summary>
        /// Spawns an object from the pool, sets its position, rotation, and parent, and returns its component of type T.
        /// </summary>
        public T Spawn<T>(string key, Vector3 pos, Quaternion rot, Transform parent)
        {
            var o = Spawn(key, pos, rot, parent);
            return o ? o.GetComponent<T>() : default;
        }

        /// <summary>
        /// Despawns an object back to the pool associated with the specified key.
        /// </summary>
        public void Despawn(string key, GameObject obj)
        {
            var pool = GetPoolByKey(key);
            pool?.Despawn(obj);
        }

        /// <summary>
        /// Despawns an object back to the pool associated with the specified key.
        /// </summary>
        public void Despawn<T>(string key, T obj) where T : MonoBehaviour
        {
            Despawn(key, obj.gameObject);
        }
    }
}
