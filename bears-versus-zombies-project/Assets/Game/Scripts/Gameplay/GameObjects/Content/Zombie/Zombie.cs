using Fusion;
using Modules.Hitboxes;
using Modules.ObjectsDetection;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace SampleGame.Gameplay.GameObjects
{
    public sealed class Zombie : NetworkBehaviour
    {
        [Title("Components")]
        [SerializeField, Required, ChildGameObjectsOnly] 
        private HealthComponent _health;
        
        [SerializeField, Required, ChildGameObjectsOnly] 
        private PathFinderMoveComponent _moveComponent;
        
        [SerializeField, Required, ChildGameObjectsOnly] 
        private HitboxesStateSwitcher _hitboxesStateSwitcher;
        
        [SerializeField, Required, ChildGameObjectsOnly]
        private ObjectsRaycaster[] _detectors;
        
        [SerializeField]
        private AutoObjectsRaycaster[] _autoObjectsDetectors;
        
        [Title("Settings")]
        [SerializeField, MinValue(0)] private float _despawnDelay = 2f;
        [SerializeField, MinValue(0)] private float _hitStunSeconds = 0.6f;
        [SerializeField, MinValue(0)] private int _attackDamage = 10;
        
        [Title("Handlers")]
        [SerializeField] private GameObject[] _onDieDisableObjects;
        
        public float DespawnDelay => _despawnDelay;
        
        public float HitStunSeconds => _hitStunSeconds;

        public int AttackDamage => _attackDamage;

        public override void Spawned()
        {
            foreach (ObjectsRaycaster detector in _detectors)
                detector.Initialize();
            
            foreach (AutoObjectsRaycaster detector in _autoObjectsDetectors)
                detector.Initialize();
            
            _hitboxesStateSwitcher.Enable();
            _onDieDisableObjects.ForEach(x => x.SetActive(true));
            _moveComponent.AddCondition(() => _health.IsAlive);
            _health.Die += OnDie;
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            _health.Die -= OnDie;
        }

        private void OnDie()
        {
            _onDieDisableObjects.ForEach(x => x.SetActive(false));
            _hitboxesStateSwitcher.Disable();
        }
        
        [Button, HideInPlayMode]
        private void ScanComponents()
        {
            _detectors = GetComponentsInChildren<ObjectsRaycaster>();
            _autoObjectsDetectors = GetComponentsInChildren<AutoObjectsRaycaster>();
            _moveComponent = GetComponent<PathFinderMoveComponent>();
            _health = GetComponent<HealthComponent>();
            _hitboxesStateSwitcher = GetComponentInChildren<HitboxesStateSwitcher>();
        }
    }
}