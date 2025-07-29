using Fusion;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.GameObjects
{
    [CreateAssetMenu(fileName = "new ZombieConfig", menuName = "SampleGame/Zombies/ZombieConfig")]
    public sealed class ZombieConfig : ScriptableObject
    {
        [SerializeField, Required] private NetworkPrefabRef _prefabReference;
        [SerializeField] private int _minReward = 1;
        [SerializeField] private int _maxReward = 5;
        
        public NetworkPrefabRef PrefabReference => _prefabReference;
        
        public int MinReward => _minReward;
        
        public int MaxReward => _maxReward;
    }
}