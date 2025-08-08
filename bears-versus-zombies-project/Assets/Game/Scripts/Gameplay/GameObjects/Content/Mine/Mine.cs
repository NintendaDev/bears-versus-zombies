using Fusion;
using Modules.ObjectsDetection;
using SampleGame.Gameplay.Context;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace SampleGame.Gameplay.GameObjects
{
    public sealed class Mine : NetworkBehaviour
    {
        [SerializeField, Required, ChildGameObjectsOnly]
        private AutoObjectsRaycaster _activateAutoRaycaster;
        
        [SerializeField, Required, ChildGameObjectsOnly]
        private ObjectsRaycaster _killRaycaster;
        
        [SerializeField, Required, ChildGameObjectsOnly]
        private HealthComponent _health;
        
        [SerializeField, Required, ChildGameObjectsOnly]
        private ObjectsRaycaster[] _raycasters;
        
        [SerializeField]
        private AutoObjectsRaycaster[] _autoObjectsRaycasters;
        
        [SerializeField, Required, ChildGameObjectsOnly]
        private GameObject[] _onExplosionDisableObjects;

        private CollisionData[] _killHits;
        private IGameCycleState _gameCycleState;

        public override void Spawned()
        {
            _gameCycleState = GameContext.Instance.Get<GameCycle>();
            
            _killHits = new CollisionData[_killRaycaster.MaxHits];

            foreach (ObjectsRaycaster detector in _raycasters)
                detector.Initialize();
            
            foreach (AutoObjectsRaycaster detector in _autoObjectsRaycasters)
                detector.Initialize();
            
            _activateAutoRaycaster.AddCondition(() => _gameCycleState.IsPlaying);
            _killRaycaster.AddCondition(() => _gameCycleState.IsPlaying);
            
            _onExplosionDisableObjects.ForEach(x => x.SetActive(true));
            _activateAutoRaycaster.Hit += OnActivateAuto;
            _health.Die += OnDie;
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            _activateAutoRaycaster.Hit -= OnActivateAuto;
            _health.Die -= OnDie;
        }

        private void OnActivateAuto(CollisionData[] data)
        {
            if (_killRaycaster.TryDetect(_killHits))
            {
                foreach (CollisionData killHit in _killHits)
                {
                    if (killHit.IsValid == false)
                        continue;
                
                    HealthComponent health = killHit.Object.GetComponentInParent<HealthComponent>();
                
                    if (health != null && health.IsAlive)
                        health.Kill(DamageData.Default(Runner.Tick.Raw));
                }
            }
            
            _health.Kill(DamageData.Self(Runner.Tick.Raw));
        }

        private void OnDie()
        {
            _onExplosionDisableObjects.ForEach(x => x.SetActive(false));
        }
        
        [Button, HideInPlayMode]
        private void ScanComponents()
        {
            _raycasters = GetComponentsInChildren<ObjectsRaycaster>();
            _autoObjectsRaycasters = GetComponentsInChildren<AutoObjectsRaycaster>();
        }
    }
}