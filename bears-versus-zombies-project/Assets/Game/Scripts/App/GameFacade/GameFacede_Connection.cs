using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.Sockets;

namespace SampleGame.App
{
    public partial class GameFacade
    {
        private readonly Dictionary<int, OnConnectionRequestReply> _pendingConnections = new();
        private CancellationTokenSource _validateCancellationTokenSource;

        public event Action<NetConnectFailedReason> ConnectFailed;
            
        void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner,
            NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
            OnConnectRequestAsync(runner, request, token).Forget();
        }

        private async UniTask OnConnectRequestAsync(NetworkRunner runner,
            NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
            int connectionToken = BitConverter.ToInt32(token);

            _validateCancellationTokenSource = _tokenSourceService.DisposeAndCreate(_validateCancellationTokenSource);

            if (_pendingConnections.TryAdd(connectionToken, OnConnectionRequestReply.Waiting))
                await ValidateTokenAsync(token, connectionToken, _validateCancellationTokenSource.Token);

            switch (_pendingConnections[connectionToken])
            {
                case OnConnectionRequestReply.Waiting:
                    request.Waiting();
                    break;
                
                case OnConnectionRequestReply.Ok:
                    request.Accept();
                    break;
                
                case OnConnectionRequestReply.Refuse:
                    request.Refuse();
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async UniTask ValidateTokenAsync(byte[] token, int connectionToken, CancellationToken cancellationToken)
        {
            if (await _connectionTokenService.ValidateTokenAsync(token, cancellationToken))
                _pendingConnections[connectionToken] = OnConnectionRequestReply.Ok;
            else
                _pendingConnections[connectionToken] = OnConnectionRequestReply.Refuse;
        }
        
        void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, 
            NetConnectFailedReason reason)
        {
            ConnectFailed?.Invoke(reason);
        }
    }
}