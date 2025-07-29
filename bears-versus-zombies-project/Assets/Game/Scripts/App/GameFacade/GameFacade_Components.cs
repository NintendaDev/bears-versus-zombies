using System;
using Fusion;
using SampleGame.App.SceneManagement;
using SampleGame.Gameplay.GameContext;

namespace SampleGame.App
{
    public partial class GameFacade
    {
        public NetworkRunner Runner { get; private set; }
        
        private PoolableNetworkObjectProvider ObjectProvider { get; set; }
        
        private FusionSceneManager NetworkSceneManager { get; set; }
        
        public event Action<NetworkRunner> RunnerChanged;
        
        private void CreateNetworkComponents()
        {
            if (Runner != null)
                Destroy(Runner.gameObject);
            
            Runner = Instantiate(_networkRunnerPrefab);
            DontDestroyOnLoad(Runner.gameObject);
            
            NetworkSceneManager = Runner.GetComponentInChildren<FusionSceneManager>();
            ObjectProvider = Runner.GetComponentInChildren<PoolableNetworkObjectProvider>();
            
            Runner.AddCallbacks(this);
            Runner.ProvideInput = true;
            
            RunnerChanged?.Invoke(Runner);
        }
    }
}