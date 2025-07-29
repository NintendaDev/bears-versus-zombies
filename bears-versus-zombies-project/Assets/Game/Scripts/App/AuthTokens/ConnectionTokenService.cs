using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SampleGame.App
{
    public abstract class ConnectionTokenService : MonoBehaviour, IConnectionTokenService
    {
        public abstract UniTask<byte[]> GetTokenAsync();

        public abstract UniTask<bool> ValidateTokenAsync(byte[] token, CancellationToken cancellationToken = default);
    }
}