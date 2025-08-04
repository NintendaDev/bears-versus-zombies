using Fusion;
using Modules.ObjectsDetection;
using SampleGame.Gameplay.GameContext;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace SampleGame.Gameplay.GameObjects
{
    public sealed class Turret : NetworkBehaviour
    {
        [SerializeField, Required, ChildGameObjectsOnly] 
        private RaycastFireComponent _fireComponent;
        
        [SerializeField, Required, ChildGameObjectsOnly] 
        private AutoFireComponent _autoFireComponent;
        
        [SerializeField, Required, ChildGameObjectsOnly] 
        private HealthComponent _health;

        [SerializeField, Required, ChildGameObjectsOnly]
        private ObjectsRaycaster[] _raycasters;

        [SerializeField]
        private AutoObjectsRaycaster[] _autoObjectsRaycasters;

        [SerializeField]
        private GameObject[] _onDieDisableObjects;

        private IGameCycleState _gameCycleState;

        public override void Spawned()
        {
            _gameCycleState = GameContextService.Instance.Get<GameCycle>();
            
            _raycasters.ForEach(x => x.Initialize());
            _autoObjectsRaycasters.ForEach(x => x.Initialize());
            _fireComponent.Initialize();
            _autoFireComponent.Initialize();
            
            _autoFireComponent.AddCondition(() => _gameCycleState.IsPlaying);
            _onDieDisableObjects.ForEach(x => x.SetActive(true));
            _health.Die += OnDie;
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            _health.Die -= OnDie;
        }

        private void OnDie()
        {
            _onDieDisableObjects.ForEach(x => x.SetActive(false));
        }
        
        [Button, HideInPlayMode]
        private void ScanComponents()
        {
            _raycasters = GetComponentsInChildren<ObjectsRaycaster>();
            _autoObjectsRaycasters = GetComponentsInChildren<AutoObjectsRaycaster>();
            _fireComponent = GetComponent<RaycastFireComponent>();
            _autoFireComponent = GetComponent<AutoFireComponent>();
            _health = GetComponent<HealthComponent>();
        }
    }
}