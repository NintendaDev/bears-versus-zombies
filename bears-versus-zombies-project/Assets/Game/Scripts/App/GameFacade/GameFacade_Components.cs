using Fusion;
using R3;
using SampleGame.Gameplay.Context;
using Object = UnityEngine.Object;

namespace SampleGame.App
{
    public partial class GameFacade
    {
        private readonly ReactiveCommand _stateChangedCommand = new();
        
        private NetworkRunner Runner { get; set; }
        
        private PoolableNetworkObjectProvider ObjectProvider { get; set; }
        
        private NetworkSceneManager NetworkSceneManager { get; set; }
        
        public Observable<Unit> StateChanged => _stateChangedCommand.AsObservable();
        
        private void CreateNetworkComponents()
        {
            if (Runner != null)
                Object.Destroy(Runner.gameObject);
            
            Runner = Object.Instantiate(_config.NetworkRunnerPrefab);
            Object.DontDestroyOnLoad(Runner.gameObject);
            
            NetworkSceneManager = Runner.GetComponentInChildren<NetworkSceneManager>();
            ObjectProvider = Runner.GetComponentInChildren<PoolableNetworkObjectProvider>();
            
            Runner.AddCallbacks(this);
            Runner.ProvideInput = true;
            _stateChangedCommand.Execute(Unit.Default);
        }
    }
}