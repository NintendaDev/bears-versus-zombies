using System;
using Fusion;
using Modules.ObjectsDetection;
using Modules.Services;
using Modules.Wallet;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.GameContext
{
    public sealed class TrapSpawner : NetworkBehaviour
    {
        [SerializeField, Required] private TrapsSettings _settings;
        [SerializeField, Required] private InputReceiver _input;
        [SerializeField, Required] private LineObjectsRaycaster _spawnPointRaycaster;
        
        private const int MaxHits = 1;
        private TrapFactory _factory;
        private Wallet _wallet;
        private CollisionData[] _hits;

        public event Action<TrapType> Success;
        
        public event Action<TrapType> Error;

        public override void Spawned()
        {
            _wallet = ServiceLocator.Instance.Get<Wallet>();
            _factory = ServiceLocator.Instance.Get<TrapFactory>();
            
            _hits = new CollisionData[MaxHits];
            _input.TrapRequested += OnTrapRequest;
        }
        
        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            _input.TrapRequested -= OnTrapRequest;
        }

        [Button, HideInEditorMode]
        private void OnTrapRequest(TrapType trapType)
        {
            if (HasStateAuthority == false)
                return;
            
            SpawnTrap(trapType);
        }
        
        private void SpawnTrap(TrapType trapType)
        {
            if (_settings.TryGetCost(trapType, out int cost) == false || _wallet.TrySpend(cost) == false)
            {
                SendSpawnStatusRpc(trapType,  isSuccess: false, target: Object.InputAuthority);
                
                return;
            }

            if (_spawnPointRaycaster.TryDetect(_hits) && _hits[0].IsValid)
            {
                if (_factory.TryCreate(trapType, _hits[0].Point, Quaternion.identity, out _))
                {
                    SendSpawnStatusRpc(trapType, isSuccess: true, target: Object.InputAuthority);

                    return;
                }
            }
            
            SendSpawnStatusRpc(trapType, isSuccess: false, target: Object.InputAuthority);
        }

        [Rpc(RpcSources.StateAuthority, 
            RpcTargets.InputAuthority,
            Channel = RpcChannel.Reliable,
            InvokeLocal = true,
            TickAligned = true)]
        private void SendSpawnStatusRpc(TrapType trapType, bool isSuccess, [RpcTarget] PlayerRef target)
        {
            if (isSuccess)
                Success?.Invoke(trapType);
            else
                Error?.Invoke(trapType);
        }
    }
}