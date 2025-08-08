using Fusion;
using Modules.Wallet;
using SampleGame.Gameplay.GameObjects;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SampleGame.Gameplay.Context
{
    public sealed class ZombieFactory : SimulationBehaviour, ISpawned
    {
        [SerializeField, Required, SceneObjectsOnly] private SpawnPointService _spawnPointService;
        [SerializeField, Required] private ZombieConfig _config;
        
        private Wallet _wallet;

        void ISpawned.Spawned()
        {
            _wallet = GameContext.Instance.Get<Wallet>();
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