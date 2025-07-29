using System;
using Fusion;
using Modules.Conditions;
using Modules.Types;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.GameObjects
{
    public sealed class PathFinderMoveComponent : NetworkBehaviour
    {
        [SerializeField, Required, ChildGameObjectsOnly]
        private FollowerEntity _agent;
        
        private readonly AndCondition _condition = new();
        private readonly MemorizedVector3 _lastTargetPosition = new();
        
        [ShowInInspector, Sirenix.OdinInspector.ReadOnly, HideInEditorMode]
        public Vector3 Velocity => _agent.velocity;

        public bool IsStopped { get; private set; }

        public override void Spawned()
        {
            _agent.enabled = HasStateAuthority;

            if (HasStateAuthority)
            {
                _agent.Teleport(transform.position);
                StartAgent();
            }
        }
        
        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            _condition.Dispose();
        }

        public override void FixedUpdateNetwork()
        {
            if (IsStopped == false && _condition.IsTrue() == false)
                StopAgent();
        }
        
        public void AddCondition(Func<bool> condition)
        {
            _condition.AddCondition(condition);
        }
        
        public void Move(Vector3 position)
        {
            if (IsStopped)
                return;
            
            _lastTargetPosition.Update(position);

            if (_lastTargetPosition.IsChanged() == false)
                return;
            
            _agent.SetDestination(position);
        }

        public void StopAgent()
        {
            if (IsStopped)
                return;
            
            _agent.SetPath(null);
            _agent.isStopped = true;
            _lastTargetPosition.Reset();
            IsStopped = true;
        }
        
        public void StartAgent()
        {
            if (IsStopped == false)
                return;
            
            _agent.isStopped = false;
            IsStopped = false;
        }
    }
}