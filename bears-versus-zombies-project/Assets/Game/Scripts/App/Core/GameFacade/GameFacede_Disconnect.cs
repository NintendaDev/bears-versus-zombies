using Fusion;
using Fusion.Sockets;
using R3;

namespace SampleGame.App
{
    public partial class GameFacade
    {
        private readonly ReactiveCommand<NetDisconnectReason> _disconnectedCommand = new();
        private readonly ReactiveCommand _gameCloseCommand = new();

        public Observable<NetDisconnectReason> Disconnected => _disconnectedCommand.AsObservable();
        
        public Observable<Unit> GameClosed => _gameCloseCommand.AsObservable();
        
        void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
            _disconnectedCommand.Execute(reason);
        }
        
        void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            if (shutdownReason == ShutdownReason.DisconnectedByPluginLogic)
                _gameCloseCommand.Execute(Unit.Default);
        }
    }
}