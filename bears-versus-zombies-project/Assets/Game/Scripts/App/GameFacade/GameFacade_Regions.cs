using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Fusion;
using ObservableCollections;
using UnityEngine;

namespace SampleGame.App
{
    public partial class GameFacade
    {
        [SerializeField] private string[] _regionsFilter = new[] { "eu", "us", "asia" };

        private readonly ObservableDictionary<string, RegionInfo> _actualRegions = new();
        private CancellationTokenSource _regionsCancellationTokenSource;

        [field: SerializeField] public string DefaultRegion { get; private set; }  = "eu";
        
        public string CurrentRegion { get; private set; }

        public IReadOnlyObservableDictionary<string, RegionInfo> ActualRegions => _actualRegions;
        
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
            _lobbyCancellationTokenSource = _cancelTokenSourceService.DisposeAndCreate(_lobbyCancellationTokenSource);
            
            CancellationToken linkedToken = CancellationTokenSource.CreateLinkedTokenSource(
                _lobbyCancellationTokenSource.Token, cancellationToken)
                .Token;

            List<RegionInfo> regionsInfo = await NetworkRunner
                .GetAvailableRegions(cancellationToken: linkedToken)
                .AsUniTask();

            return regionsInfo.Where(x => _regionsFilter.Contains(x.RegionCode.ToLower()));
        }
    }
}