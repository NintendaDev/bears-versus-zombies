using System.Collections;
using Cysharp.Threading.Tasks;
using Fusion;
using SampleGame.Gameplay.GameContext;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SampleGame.App.SceneManagement
{
    public sealed class FusionSceneManager : NetworkSceneManagerDefault
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
            SceneInitializer[] sceneInitializers = scene.GetComponents<SceneInitializer>(includeInactive: true);
            
            foreach (SceneInitializer sceneInitializer in sceneInitializers)
                yield return sceneInitializer.InitializeAsync().ToCoroutine();
            
            SceneSimulationBehaviour[] sceneSimulationBehaviours = 
                scene.GetComponents<SceneSimulationBehaviour>(includeInactive: true);

            foreach (SceneSimulationBehaviour sceneSimulation in sceneSimulationBehaviours)
                sceneSimulation.RegisterOnRunner();
            
            _playersService.EnableEvents();

            if (_isHostMigration)
                _playersService.ReplayJoinPlayersHostMigration();
            else
                _playersService.ReplayJoinPlayers();

            yield return base.OnSceneLoaded(sceneRef, scene, sceneParams);
        }
    }
}