using System.Collections.Generic;
using System.Threading;

namespace Modules.AsyncTaskTokens
{
    public sealed class TokenSourceService : ITokenSourceService
    {
        private readonly List<CancellationTokenSource> _sources = new();

        public void Dispose()
        {
            foreach (CancellationTokenSource tokenSource in _sources)
                DisposeTokenInternal(tokenSource);
            
            _sources.Clear();
        }

        public CancellationTokenSource CreateTokenSource()
        {
            CancellationTokenSource tokenSource = new();
            _sources.Add(tokenSource);
            
            return tokenSource;
        }

        public CancellationTokenSource DisposeAndCreate(CancellationTokenSource tokenSource)
        {
            TryDisposeToken(tokenSource);
            
            return CreateTokenSource();
        }
        
        public bool TryDisposeToken(CancellationTokenSource tokenSource)
        {
            if (tokenSource == null || _sources.Contains(tokenSource) == false)
                return false;
            
            tokenSource.Cancel();
            tokenSource.Dispose();
            _sources.Remove(tokenSource);
            
            return true;
        }
        
        private void DisposeTokenInternal(CancellationTokenSource tokenSource)
        {
            tokenSource.Cancel();
            tokenSource.Dispose();
        }
    }
}