using System;
using Fusion;
using Fusion.Sockets;

namespace SampleGame.App
{
    public partial class GameFacade
    {
        public event Action<NetDisconnectReason> Disconnected;
        
        public event Action GameClosed;
        
        void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
            Disconnected?.Invoke(reason);
        }
        
        void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            if (shutdownReason == ShutdownReason.DisconnectedByPluginLogic)
                GameClosed?.Invoke();
        }
    }
}