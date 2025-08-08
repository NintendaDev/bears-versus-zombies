using Fusion;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.Context
{
    [CreateAssetMenu(fileName = "new GameplayAssets", menuName = "SampleGame/Assets/GameplayAssets")]
    public sealed class GameplayAssets : ScriptableObject
    {
        [SerializeField, Required] private NetworkPrefabRef _zombiePrefabReference;
        [SerializeField, Required] private NetworkPrefabRef _turretPrefabReference;
        [SerializeField, Required] private NetworkPrefabRef _minePrefabReference;
        
        public NetworkPrefabRef ZombiePrefabReference => _zombiePrefabReference;
        
        public NetworkPrefabRef TurretPrefabReference => _turretPrefabReference;
        
        public NetworkPrefabRef MinePrefabReference => _minePrefabReference;
    }
}