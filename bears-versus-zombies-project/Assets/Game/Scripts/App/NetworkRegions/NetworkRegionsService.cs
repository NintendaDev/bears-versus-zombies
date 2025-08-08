using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Fusion;
using Modules.AssetsManagement.StaticData;
using ObservableCollections;

namespace SampleGame.App
{
    public sealed class NetworkRegionsService : IDisposable, INetworkRegionsService
    {
        private readonly IStaticDataService _staticDataService;
        private readonly ObservableDictionary<string, RegionInfo> _actualRegions = new();
        private CancellationTokenSource _regionsCancellationTokenSource;
        private NetworkRegionsConfig _config;
        private CancellationTokenSource _cancellationTokenSource;

        public NetworkRegionsService(IStaticDataService staticDataService)
        {
            _staticDataService = staticDataService;
        }
        
        public string CurrentRegion { get; private set; }

        public IReadOnlyObservableDictionary<string, RegionInfo> ActualRegions => _actualRegions;

        public void Dispose()
        {
            DisposeToken();
        }

        public UniTask InitializeAsync()
        {
            _config = _staticDataService.GetConfiguration<NetworkRegionsConfig>();
            SetRegion(_config.DefaultRegion);

            return UniTask.CompletedTask;
        }

        public async UniTask RefreshActualRegionsAsync(CancellationToken cancellationToken = default)
        {
            foreach (RegionInfo regionInfo in await GetRegionsAsync(cancellationToken))
                _actualRegions[regionInfo.RegionCode] = regionInfo;
        }

        public void SetRegion(RegionInfo regionInfo) => SetRegion(regionInfo.RegionCode);

        public void SetRegion(string region)
        {
            region = region.ToLower();
            CurrentRegion = region;
        }

        private async UniTask<IEnumerable<RegionInfo>> GetRegionsAsync(CancellationToken cancellationToken = default)
        {
            DisposeToken();
            _cancellationTokenSource = new CancellationTokenSource();

            List<RegionInfo> regionsInfo = await NetworkRunner
                .GetAvailableRegions(cancellationToken: _cancellationTokenSource.Token)
                .AsUniTask();

            return regionsInfo.Where(x => _config.RegionsFilter.Contains(x.RegionCode.ToLower()));
        }

        private void DisposeToken()
        {
            if (_cancellationTokenSource == null)
                return;
            
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
    }
}