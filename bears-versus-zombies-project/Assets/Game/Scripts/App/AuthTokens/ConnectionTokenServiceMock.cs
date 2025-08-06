using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Random = System.Random;

namespace SampleGame.App
{
    public sealed class ConnectionTokenServiceMock : IConnectionTokenService
    {
        private readonly float _serverDelaySeconds;
        private byte[] _sampleToken;

        public ConnectionTokenServiceMock(float serverDelaySeconds)
        {
            _serverDelaySeconds = serverDelaySeconds;
            _sampleToken = new byte[sizeof(int)];
            new Random().NextBytes(_sampleToken);
        }

        public async UniTask<byte[]> GetTokenAsync()
        {
            return await Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(_serverDelaySeconds));

                return _sampleToken;
            });
        }

        public async UniTask<bool> ValidateTokenAsync(byte[] token, 
            CancellationToken cancellationToken = default)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_serverDelaySeconds), cancellationToken: cancellationToken);
            
            return token.Length == sizeof(int);
        }
    }
}