using System;
using System.Linq;
using Fusion;
using UnityEngine;

namespace SampleGame.Gameplay.Context
{
    [CreateAssetMenu(fileName = "new TrapsSettings", menuName = "SampleGame/TrapsSettings")]
    public sealed class TrapsSettings : ScriptableObject
    {
        [SerializeField] private TrapData[] _prefabs;
        
        public bool TryGetButtonKey(TrapType trapType, out string buttonKey)
        {
            buttonKey = string.Empty;
            
            if (IsExistsTrapData(trapType, out TrapData data) == false)
                return false;

            buttonKey = data.ButtonCode;
            
            return string.IsNullOrEmpty(buttonKey) == false;
        }
        
        public bool TryGetPrefab(TrapType trapType, out NetworkPrefabRef prefab)
        {
            prefab = default;
            
            if (IsExistsTrapData(trapType, out TrapData data) == false)
                return false;

            prefab = data.Prefab;
            
            return prefab != null;
        }

        public bool TryGetCost(TrapType trapType, out int cost)
        {
            cost = 0;
            
            if (IsExistsTrapData(trapType, out TrapData data) == false)
                return false;

            cost = data.Cost;
            
            return true;
        }

        private bool IsExistsTrapData(TrapType trapType, out TrapData data)
        {
            data = _prefabs.Where(x => x.Type == trapType).FirstOrDefault();

            return data != null;
        }
        
        [Serializable]
        private class TrapData
        {
            [field: SerializeField] public NetworkPrefabRef Prefab { get; private set; }

            [field: SerializeField] public TrapType Type { get; private set; }

            [field: SerializeField] public int Cost { get; private set; }
            
            [field: SerializeField] public string ButtonCode { get; private set; }
        }
    }
}