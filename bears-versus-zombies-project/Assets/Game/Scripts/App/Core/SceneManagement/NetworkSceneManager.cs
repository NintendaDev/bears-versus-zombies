using System.Collections;
using Fusion;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SampleGame.App
{
    public sealed class NetworkSceneManager : NetworkSceneManagerDefault
    {
        [SerializeField, Required] private PlayersService _playersService;
        
        private bool _isHostMigration;

        public void EnableHostMigrationMode()
        {
            _isHostMigration = true;
        }
        
        public void DisableHostMigrationMode()
        {
            _isHostMigration = false;
        }

        protected override IEnumerator OnSceneLoaded(SceneRef sceneRef, Scene scene,
            NetworkLoadSceneParameters sceneParams)
        {
            RegisterSceneSimulationBehaviours(scene);
            yield return base.OnSceneLoaded(sceneRef, scene, sceneParams);
            InitPlayersService();
        }

        private void InitPlayersService()
        {
            _playersService.EnableEvents();

            if (_isHostMigration)
                _playersService.ReplayJoinPlayersHostMigration();
            else
                _playersService.ReplayJoinPlayers();
        }

        private void RegisterSceneSimulationBehaviours(Scene scene)
        {
            SimulationBehaviour[] simulationBehaviours = 
                scene.GetComponents<SimulationBehaviour>(includeInactive: true);

            foreach (SimulationBehaviour sceneSimulation in simulationBehaviours)
            {
                if (sceneSimulation is NetworkBehaviour)
                    continue;
                
                if (sceneSimulation.GetComponentInParent<NetworkRunner>() != null)
                    continue;
                
                Runner.AddGlobal(sceneSimulation);
            }
        }
    }
}