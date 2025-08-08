using Fusion;
using Fusion.Addons.SimpleKCC;
using Modules.ObjectsDetection;
using SampleGame.Gameplay.Context;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.GameObjects
{
    public class Player : NetworkBehaviour
    {
        [SerializeField, Required, ChildGameObjectsOnly]
        private MoveComponent _moveComponent;
        
        [SerializeField, Required, ChildGameObjectsOnly]
        private RotationComponent _rotationComponent;
        
        [SerializeField, Required, ChildGameObjectsOnly]
        private HealthComponent _health;
        
        [SerializeField, Required, ChildGameObjectsOnly] 
        private SimpleKCC _characterController;
        
        [SerializeField, Required, ChildGameObjectsOnly] 
        private LagFireComponent _fireComponent;
        
        [SerializeField, Required, ChildGameObjectsOnly] 
        private AutoFireComponent _autoFireComponent;
        
        [SerializeField, Required, ChildGameObjectsOnly]
        private ObjectsRaycaster[] _raycasters;
        
        [SerializeField]
        private AutoObjectsRaycaster[] _autoObjectsRaycasters;
        
        [SerializeField, Required, ChildGameObjectsOnly] 
        private Canvas _visualUI;
        
        private GameCycle _gameCycle;
        
        public bool IsSpawned { get; private set; }

        public override void Spawned()
        {
            _gameCycle = GameContext.Instance.Get<GameCycle>();
            
            foreach (ObjectsRaycaster detector in _raycasters)
                detector.Initialize();
            
            foreach (AutoObjectsRaycaster detector in _autoObjectsRaycasters)
                detector.Initialize();
            
            _rotationComponent.Initialize();
            _moveComponent.Initialize();
            _fireComponent.Initialize();
            _autoFireComponent.Initialize();
            
            _fireComponent.AddCondition(() => _gameCycle.State == GameState.Playing);
            _autoFireComponent.AddCondition(() => _gameCycle.State == GameState.Playing);
            _moveComponent.AddCondition(() => _health.IsAlive);
            _rotationComponent.AddCondition(() => _health.IsAlive);
            
            _health.AddDieHandler(() =>
            {
                _gameCycle.FinishGame(FinishReason.PlayerDie);
            });

            _health.Die += OnDie;
            IsSpawned = true;
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            _health.Die -= OnDie;
        }

        private void OnDie()
        {
            _characterController.Collider.gameObject.SetActive(false);
            _visualUI.gameObject.SetActive(false);
        }
        
        [Button, HideInPlayMode]
        private void ScanComponents()
        {
            _raycasters = GetComponentsInChildren<ObjectsRaycaster>();
            _autoObjectsRaycasters = GetComponentsInChildren<AutoObjectsRaycaster>();
            _moveComponent = GetComponent<MoveComponent>();
            _health = GetComponent<HealthComponent>();
            _characterController = GetComponent<SimpleKCC>();
            _rotationComponent = GetComponent<RotationComponent>();
            _fireComponent = GetComponent<LagFireComponent>();
            _autoFireComponent = GetComponent<AutoFireComponent>();
            _visualUI = GetComponentInChildren<Canvas>();
        }
    }
}