using Fusion;
using SampleGame.Gameplay.GameObjects;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.PlayerContext
{
    public sealed class CharacterDieController : NetworkBehaviour
    {
        [SerializeField, MinValue(0)] private float _despawnDelay = 3f;
        [SerializeField, Required] private HealthComponent _healthComponent;
        
        [Networked]
        private NetworkBool HasDieRequest { get; set; }

        [Networked]
        private TickTimer DespawnDelayTimer { get; set; }

        public override void Spawned()
        {
            HasDieRequest = false;
            _healthComponent.AddDieHandler(OnDie);
        }

        public override void FixedUpdateNetwork()
        {
            if (Object == null || Object.HasStateAuthority == false || Runner == null)
                return;
            
            if (HasDieRequest == false)
                return;

            if (DespawnDelayTimer.ExpiredOrNotRunning(Runner) == false)
                return;
            
            HasDieRequest = false;
            Runner.Despawn(Object);
        }

        private void OnDie()
        {
            if (HasDieRequest || HasStateAuthority == false)
                return;
            
            HasDieRequest = true;
            DespawnDelayTimer = TickTimer.CreateFromSeconds(Runner, _despawnDelay);
        }
    }
}