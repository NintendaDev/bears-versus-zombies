using System;
using System.Threading;

namespace Modules.AsyncTaskTokens
{
    public interface ITokenSourceService : IDisposable
    {
        public CancellationTokenSource CreateTokenSource();

        public CancellationTokenSource DisposeAndCreate(CancellationTokenSource tokenSource);
        
        public bool TryDisposeToken(CancellationTokenSource tokenSource);
    }
}