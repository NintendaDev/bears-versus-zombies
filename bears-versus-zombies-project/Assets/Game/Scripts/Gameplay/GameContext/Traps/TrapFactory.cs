using Fusion;
using Modules.Services;
using UnityEngine;

namespace SampleGame.Gameplay.GameContext
{
    public sealed class TrapFactory : SceneSimulationBehaviour, ISpawned
    {
        private TrapsSettings _settings;
        
        void ISpawned.Spawned()
        {
            _settings = ServiceLocator.Instance.Get<TrapsSettings>();
        }

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