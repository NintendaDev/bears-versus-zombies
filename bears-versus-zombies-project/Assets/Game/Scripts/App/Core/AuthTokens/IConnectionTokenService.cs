using System.Threading;
using Cysharp.Threading.Tasks;

namespace SampleGame.App
{
    public interface IConnectionTokenService
    {
        public UniTask<byte[]> GetTokenAsync();
        
        public UniTask<bool> ValidateTokenAsync(byte[] token, CancellationToken cancellationToken = default);
    }
}