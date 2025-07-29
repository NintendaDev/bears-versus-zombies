using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.Photon.Realtime;
using ObservableCollections;
using UnityEngine;
using ZLinq;

namespace SampleGame.App
{
    public partial class GameFacade
    {
        private readonly string _defaultLobbyName = "BearsVsZombiesLobby";
        private readonly ObservableDictionary<string, SessionInfo> _actualSessions = new();
        private CancellationTokenSource _lobbyCancellationTokenSource;
        
        public IReadOnlyObservableDictionary<string, SessionInfo> ActualSessions => _actualSessions;
        
        public async UniTask<bool> TryJoinLobby()
        {
            await ShutdownRunnerAsync(ShutdownReason.Ok);
            CreateNetworkComponents();
            
            _actualSessions.Clear();
            _lobbyCancellationTokenSource = _tokenSourceService.DisposeAndCreate(_lobbyCancellationTokenSource);
            
            FusionAppSettings settings = CreateAppSettings();
            
            StartGameResult result = await Runner 
                .JoinSessionLobby(
                    SessionLobby.Custom, 
                    _defaultLobbyName, 
                    customAppSettings: settings,
                    cancellationToken: _lobbyCancellationTokenSource.Token
                    ) 
                .AsUniTask();
            
            if (result.Ok == false)
                Debug.LogError($"Failed to Start: {result.ShutdownReason}");
            
            return result.Ok;
        }
        
        public void MakeVisibleCurrentSession()
        {
            if (Runner == null || Runner.IsServer == false || Runner.SessionInfo == null)
                return;

            Runner.SessionInfo.IsVisible = true;
        }

        public void MakeInvisibleCurrentSession()
        {
            if (Runner == null || Runner.IsServer == false || Runner.SessionInfo == null)
                return;

            Runner.SessionInfo.IsVisible = false;
        }
        
        void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            foreach (SessionInfo sessionInfo in sessionList)
                _actualSessions[sessionInfo.Name] = sessionInfo;
            
            foreach (string sessionName in _actualSessions.
                         AsValueEnumerable().
                         Select(x => x.Key).ToArray())
            {
                if (sessionList.AsValueEnumerable()
                        .FirstOrDefault(x => x.Name == sessionName) == null)
                {
                    _actualSessions.Remove(sessionName);
                }
            }
        }
    }
}