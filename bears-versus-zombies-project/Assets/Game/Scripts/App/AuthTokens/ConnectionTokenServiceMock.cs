using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = System.Random;

namespace SampleGame.App
{
    public sealed class ConnectionTokenServiceMock : ConnectionTokenService
    {
        [SerializeField, MinValue(0)] private float _serverDelaySeconds;
        
        private byte[] _sampleToken;

        private void Awake()
        {
            _sampleToken = new byte[sizeof(int)];
            new Random().NextBytes(_sampleToken);
        }

        public override async UniTask<byte[]> GetTokenAsync()
        {
            return await Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(_serverDelaySeconds));

                return _sampleToken;
            });
        }

        public override async UniTask<bool> ValidateTokenAsync(byte[] token, 
            CancellationToken cancellationToken = default)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_serverDelaySeconds), cancellationToken: cancellationToken);
            
            return token.Length == sizeof(int);
        }
    }
}