using System;
using System.Collections.Generic;

namespace OM
{
    /// <summary>
    /// Interface for objects that want to receive spawn/despawn lifecycle callbacks from the object pool.
    /// </summary>
    /// <typeparam name="T">The type of object in the pool.</typeparam>
    public interface IOMObjectPool<T>
    {
        /// <summary>
        /// Called when the object is spawned from the pool.
        /// </summary>
        /// <param name="pool">The pool that spawned the object.</param>
        void OnSpawn(OMObjectPool<T> pool);

        /// <summary>
        /// Called when the object is returned (despawned) to the pool.
        /// </summary>
        void OnDespawn();
    }

    /// <summary>
    /// A generic object pool system that supports optional spawn/despawn events and a maximum capacity.
    /// </summary>
    /// <typeparam name="T">The type of objects managed by this pool.</typeparam>
    public class OMObjectPool<T>
    {
        private readonly Func<T> _createObjFunc;
        private readonly Action<T> _onDespawnObj;
        private readonly Action<T> _onSpawn;
        private readonly bool _useIPool;
        private readonly int _maxCapacity;
        private readonly List<T> _list;
        private readonly List<T> _spawned;

        /// <summary>
        /// Gets the number of currently spawned (active) objects.
        /// </summary>
        public int GetSpawnedCount => _spawned.Count;

        /// <summary>
        /// Creates a new object pool.
        /// </summary>
        /// <param name="maxCapacity">Maximum allowed spawned objects.</param>
        /// <param name="preload">Number of objects to pre-create and add to the pool.</param>
        /// <param name="useIPool">Whether to call IOMObjectPool callbacks.</param>
        /// <param name="createObjFunc">Delegate for creating new instances.</param>
        /// <param name="onSpawn">Optional callback when an object is spawned.</param>
        /// <param name="onDespawn">Optional callback when an object is despawned.</param>
        public OMObjectPool(int maxCapacity, int preload, bool useIPool, Func<T> createObjFunc, Action<T> onSpawn = null, Action<T> onDespawn = null)
        {
            _createObjFunc = createObjFunc ?? throw new ArgumentNullException("Create Function is null" + nameof(createObjFunc));
            _useIPool = useIPool;
            _onDespawnObj = onDespawn;
            _onSpawn = onSpawn;
            _maxCapacity = maxCapacity;
            _list = new List<T>();
            _spawned = new List<T>();

            for (var i = 0; i < preload; i++)
            {
                var obj = _createObjFunc();
                _list.Add(obj);
            }
        }

        /// <summary>
        /// Spawns an object from the pool. Creates a new one if needed.
        /// Recycles old objects if max capacity is reached.
        /// </summary>
        /// <returns>The spawned object.</returns>
        public T Spawn()
        {
            T obj;
            if (_list.Count <= 0)
            {
                if (_spawned.Count >= _maxCapacity)
                {
                    obj = _spawned[0];
                    _spawned.RemoveAt(0);
                    if (_useIPool)
                    {
                        var pool = obj as IOMObjectPool<T>;
                        pool?.OnDespawn();
                    }
                }
                else
                {
                    obj = _createObjFunc();
                }
            }
            else
            {
                obj = _list[0];
                _list.RemoveAt(0);
            }

            _onSpawn?.Invoke(obj);
            if (_useIPool)
            {
                var pool = obj as IOMObjectPool<T>;
                pool?.OnSpawn(this);
            }

            _spawned.Add(obj);
            return obj;
        }

        /// <summary>
        /// Returns an object back to the pool.
        /// </summary>
        /// <param name="obj">The object to despawn.</param>
        public void Despawn(T obj)
        {
            _onDespawnObj?.Invoke(obj);
            if (_useIPool)
            {
                var pool = obj as IOMObjectPool<T>;
                pool?.OnDespawn();
            }

            _list.Add(obj);
            _spawned.Remove(obj);
        }
    }
}
