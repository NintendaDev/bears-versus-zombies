using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.Context
{
    [CreateAssetMenu(
        fileName = "new PoolableNetworkObjectCatalog",
        menuName = "SampleGame/Network/PoolableNetworkObjectCatalog"
    )]
    public sealed class PoolableNetworkObjectCatalog : ScriptableObject
    {
#if UNITY_EDITOR
        [SerializeField, OnValueChanged(nameof(OnPrefabsChange))]
        private List<GameObject> _prefabs;
#endif

        [SerializeField, Sirenix.OdinInspector.ReadOnly, ShowInInspector]
        private List<string> _poolPrefabNames = new();

        public IReadOnlyList<string> PoolPrefabsNames => _poolPrefabNames;

        public bool IsPooledPrefab(string prefabName)
        {
            return PoolPrefabsNames.Contains(prefabName);
        }

        private void OnPrefabsChange()
        {
#if UNITY_EDITOR
            _poolPrefabNames.Clear();
            
            foreach (var prefab in _prefabs)
                _poolPrefabNames.Add(prefab.name);
#endif
        }
    }
}