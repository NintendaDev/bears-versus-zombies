using Fusion;
using Modules.Services;
using Modules.Wallet;
using SampleGame.Gameplay.GameObjects;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SampleGame.Gameplay.GameContext
{
    public sealed class ZombieFactory : SceneSimulationBehaviour, ISpawned
    {
        [SerializeField] private SpawnPointService _spawnPointService;
        
        private ZombieConfig _config;
        private Wallet _wallet;

        void ISpawned.Spawned()
        {
            _wallet = ServiceLocator.Instance.Get<Wallet>();
            _config = ServiceLocator.Instance.Get<ZombieConfig>();
        }

        [Button, HideInEditorMode]
        public bool TryCreate(out ZombieAI zombieAI)
        {
            zombieAI = null;
            
            if (Runner.IsServer == false)
                return false;
            
            Transform spawnPoint = _spawnPointService.NextPoint();
            NetworkObject zombieObject = Runner.Spawn(_config.PrefabReference, spawnPoint.position, spawnPoint.rotation);
            HealthComponent healthComponent = zombieObject.GetComponent<HealthComponent>();
            zombieAI = zombieObject.GetComponent<ZombieAI>();
            
            healthComponent.AddDieHandler(() =>
            {
                if (healthComponent.LastDamageData.IsSelf == false)
                    _wallet.Add(Random.Range(_config.MinReward, _config.MaxReward));
            });

            return true;
        }
    }
}