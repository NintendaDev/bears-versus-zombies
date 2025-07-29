using Fusion;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.GameObjects
{
    public abstract class ZombieSensorBase : NetworkBehaviour
    {
        [SerializeField, Required] private ZombieBlackboard _blackboard;
        
        protected ZombieBlackboard Blackboard => _blackboard; 
    }
}