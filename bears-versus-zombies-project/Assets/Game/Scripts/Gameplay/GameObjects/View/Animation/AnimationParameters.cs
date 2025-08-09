using UnityEngine;

namespace SampleGame.GameObjects.View
{
    [CreateAssetMenu(fileName = "new AnimationParameters", 
        menuName = "SampleGame/Animation/AnimationParameters")]
    public sealed class AnimationParameters : ScriptableObject
    {
        [field: SerializeField] public string IsIdleParameterName { get; private set; } = "IsIdle";
        
        [field: SerializeField] public string IsMovingParameterName { get; private set; } = "IsMoving";
        
        [field: SerializeField] public string TakeDamageTriggerName { get; private set; } = "TakeDamage";
        
        [field: SerializeField] public string IsDieParameterName { get; private set; } = "IsDie";
        
        [field: SerializeField] public string FireTriggerName { get; private set; } = "Fire";
    }
}