using System;
using Fusion;

namespace SampleGame.Gameplay.GameContext
{
    public sealed class InitializeComponent : NetworkBehaviour
    {
        [Networked, OnChangedRender(nameof(OnChangedRender))]
        public NetworkBool IsInitialized { get; private set; }
        
        public event Action Initialized;

        public void Initialize()
        {
            if (Object.HasInputAuthority == false && Runner.IsServer == false)
                throw new Exception("Can't initialize on client without input authority");
            
            if (Runner.IsServer)
                IsInitialized = true;
            else
                InitializeStateRpc();
        }

        public void ResetState()
        {
            if (Runner.IsServer == false && Object.HasInputAuthority == false)
                throw new InvalidOperationException("Can't reset state on client without input authority");

            if (Runner.IsServer)
                IsInitialized = false;
            else
                ResetStateRpc();
        }

        [Rpc(RpcSources.InputAuthority,
            RpcTargets.StateAuthority | RpcTargets.InputAuthority,
            Channel = RpcChannel.Reliable,
            InvokeLocal = true,
            TickAligned = false)]
        public void InitializeStateRpc()
        {
            IsInitialized = true;
        }
        
        [Rpc(RpcSources.InputAuthority,
            RpcTargets.StateAuthority | RpcTargets.InputAuthority,
            Channel = RpcChannel.Reliable,
            InvokeLocal = true,
            TickAligned = false)]
        public void ResetStateRpc()
        {
            IsInitialized = false;
        }

        private void OnChangedRender()
        {
            if (IsInitialized == false)
                return;
            
            Initialized?.Invoke();
        }
    }
}