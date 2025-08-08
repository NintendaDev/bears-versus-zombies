using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using SampleGame.Gameplay.Context;
using UnityEngine;

namespace SampleGame.App
{
    public partial class GameFacade
    {
        private readonly bool _isEnabledHostMigration = false;
        
        public event Action HostMigrationStarted;
        
        public event Action HostMigrationFinished;
        
        async void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            if (_isEnabledHostMigration == false)
                return;
            
            HostMigrationStarted?.Invoke();
            await runner.Shutdown();

            CreateNetworkComponents();
            await _sceneLoader.LoadAsync(_config.LevelSceneReference);

            byte[] token = await _connectionTokenService.GetTokenAsync();
            NetworkSceneManager.EnableHostMigrationMode();
            
            StartGameResult startResult = await Runner.StartGame(new StartGameArgs
            {
                GameMode = hostMigrationToken.GameMode,
                SessionName = _lastSessionName,
                Scene = GetGameplaySceneRef(),
                SceneManager = NetworkSceneManager,
                ObjectProvider = ObjectProvider,
                PlayerCount = GameFacadeConfig.MultiPlayersCount,
                ConnectionToken = token,
                HostMigrationToken = hostMigrationToken,
                HostMigrationResume = OnHostMigrationResume,
            });
            
            if (startResult.Ok)
                await PushHostMigrationSnapshotAsync();
            else
                Debug.LogError($"Failed to Start Host Migration: {startResult.ShutdownReason}");
        }
        
        public async UniTask PushHostMigrationSnapshotAsync()
        {
            if (_isEnabledHostMigration == false)
                return;
            
            Debug.Log("Pushing Host Migration Snapshot...");
            await Runner.PushHostMigrationSnapshot().AsUniTask();
        }

        private void OnHostMigrationResume(NetworkRunner runner)
        {
            MigrateSceneObjects(runner);
            MigratePrefabObjects(runner);
            InvokeAfterMigration(runner);
        }
        
        private void MigrateSceneObjects(NetworkRunner runner)
        {
            HashSet<NetworkObject> activeSceneObjects = new();
            IEnumerable<(NetworkObject, NetworkObjectHeaderPtr)> sceneObjects = 
                runner.GetResumeSnapshotNetworkSceneObjects();

            foreach ((NetworkObject CurrentObject, 
                     NetworkObjectHeaderPtr PreviousSnapshot) sceneObjectTuple in sceneObjects)
            {
                sceneObjectTuple.CurrentObject.CopyStateFrom(sceneObjectTuple.PreviousSnapshot);
                activeSceneObjects.Add(sceneObjectTuple.CurrentObject);
            }

            foreach (NetworkObject networkObject in runner.GetAllNetworkObjects())
            {
                if (networkObject.NetworkTypeId.IsSceneObject && activeSceneObjects.Contains(networkObject) == false)
                    runner.Despawn(networkObject);
            }
        }

        private void MigratePrefabObjects(NetworkRunner runner)
        {
            IEnumerable<NetworkObject> networkObjects = runner.GetResumeSnapshotNetworkObjects();

            foreach (NetworkObject prefabObject in networkObjects)
            {
                NetworkObjectTypeId typeId = prefabObject.NetworkTypeId;
                
                if (typeId.IsValid == false || typeId.IsPrefab == false)
                    continue;
                
                Vector3 position = Vector3.zero;
                Quaternion rotation = Quaternion.identity;

                if (prefabObject.TryGetComponent(out NetworkTRSP networkTransform))
                {
                    position = networkTransform.Data.Position;
                    rotation = networkTransform.Data.Rotation;
                }

                NetworkObject networkObject = runner.Spawn(prefabObject, position, rotation, PlayerRef.None,
                    onBeforeSpawned: (_, spawnedObject) =>
                    {
                        spawnedObject.CopyStateFrom(prefabObject);
                    });
                
                if (networkObject.TryGetComponent(out InitializeComponent initializeStateComponent))
                    initializeStateComponent.ResetState();
            }
        }

        private void InvokeAfterMigration(NetworkRunner runner)
        {
            foreach (IHostMigrationHandler handler in runner.GetComponentsInChildren<IHostMigrationHandler>())
                handler.AfterMigration();
            
            HostMigrationFinished?.Invoke();
        }
    }
}