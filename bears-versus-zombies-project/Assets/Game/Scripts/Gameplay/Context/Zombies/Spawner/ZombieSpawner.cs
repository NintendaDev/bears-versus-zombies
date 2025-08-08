using System;
using Fusion;
using SampleGame.Gameplay.GameObjects;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.Context
{
    public sealed class ZombieSpawner : NetworkBehaviour
    {
        [SerializeField, Required, InlineEditor] 
        private ZombieSpawnerSettings _settings;
        
        private ZombieFactory _factory;
        private bool _isEnabled;

        [Networked]
        [OnChangedRender(nameof(OnAliveZombiesCountRender))]
        public int AliveZombiesCount { get; private set; }

        [Networked]
        private TickTimer SpawnDelayTimer { get; set; }

        [Networked]
        private TickTimer WaveDelayTimer { get; set; }

        [Networked]
        private int WaveIndex { get; set; }

        [Networked]
        private int WaveSpawnCount { get; set; }

        private bool CanSpawn => _isEnabled && IsFinished == false;
        
        public bool IsFinished => WaveIndex >= _settings.Waves.Length;
        
        public event Action<int> AliveCountChanged;
        
        public void Enable() => _isEnabled = true;
        
        public void Disable() => _isEnabled = false;

        public override void Spawned()
        {
            _factory = GameContext.Instance.Get<ZombieFactory>();
        }

        public override void FixedUpdateNetwork()
        {
            if (CanSpawn == false || SpawnDelayTimer.ExpiredOrNotRunning(Runner) == false
                                  || WaveDelayTimer.ExpiredOrNotRunning(Runner) == false)
            {
                return;
            }

            if (WaveSpawnCount >= _settings.Waves[WaveIndex].Count)
            {
                NextWave();

                return;
            }

            if (_factory.TryCreate(out ZombieAI zombie) == false)
                return;

            AliveZombiesCount++;
            HealthComponent zombieHealth = zombie.GetComponent<HealthComponent>();
            zombieHealth.AddDieHandler(() => AliveZombiesCount--);
            
            SpawnDelayTimer = TickTimer.CreateFromSeconds(Runner, _settings.Waves[WaveIndex].Delay);
            WaveSpawnCount++;
        }

        private void NextWave()
        {
            WaveDelayTimer = TickTimer.CreateFromSeconds(Runner, _settings.DelayBetweenWaves);
            WaveSpawnCount = 0;
            WaveIndex++;
        }

        private void OnAliveZombiesCountRender()
        {
            AliveCountChanged?.Invoke(AliveZombiesCount);
        }
    }
}