using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.Context
{
    [CreateAssetMenu(fileName = "new ZombieSpawnerSettings", 
        menuName = "SampleGame/Gameplay/Zombies/ZombieSpawnerSettings")]
    public sealed class ZombieSpawnerSettings : ScriptableObject
    {
        [SerializeField, MinValue(0)] private float _delayBetweenWaves = 10f;
        [SerializeField] private WaveSettings[] _waves;
        
        public WaveSettings[] Waves => _waves;
        
        public float DelayBetweenWaves => _delayBetweenWaves;
    }
}