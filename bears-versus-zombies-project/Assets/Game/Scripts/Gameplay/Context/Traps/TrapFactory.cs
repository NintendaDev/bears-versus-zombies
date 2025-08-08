using Fusion;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.Context
{
    public sealed class TrapFactory : SimulationBehaviour
    {
        [SerializeField, Required] private TrapsSettings _settings;

        public bool TryCreate(TrapType trapType, Vector3 position, Quaternion rotation, out NetworkObject trap)
        {
            trap = null;

            if (Runner.IsServer == false)
                return false;
            
            if (_settings.TryGetPrefab(trapType, out NetworkPrefabRef prefab) == false)
                return false;
            
            trap = Runner.Spawn(prefab, position, rotation);

            return true;
        }
    }
}