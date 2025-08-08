using System.Threading;
using Cysharp.Threading.Tasks;
using Fusion;
using ObservableCollections;

namespace SampleGame.App
{
    public interface INetworkRegionsService
    {
        public string CurrentRegion { get; }
        
        public IReadOnlyObservableDictionary<string, RegionInfo> ActualRegions { get; }
        
        public UniTask RefreshActualRegionsAsync(CancellationToken cancellationToken = default);
        
        public void SetRegion(RegionInfo regionInfo);
        
        public void SetRegion(string region);
    }
}