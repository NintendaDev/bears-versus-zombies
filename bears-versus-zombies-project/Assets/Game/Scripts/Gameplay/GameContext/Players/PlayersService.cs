using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using SampleGame.App;
using SampleGame.Gameplay.GameObjects;
using Sirenix.OdinInspector;
using UnityEngine;

public sealed class PlayersService : SimulationBehaviour, IPlayerJoined, IPlayerLeft, IHostMigrationHandler
{
    [SerializeField, Required, AssetsOnly]
    private NetworkPrefabRef _playerPrefab;
        
    [SerializeField, Required, MinValue(0)]
    private float _disconnectTimeout = 10f;

    [ShowInInspector, Sirenix.OdinInspector.ReadOnly]
    private readonly Dictionary<int, NetworkObject> _allPlayers = new();
        
    [ShowInInspector, Sirenix.OdinInspector.ReadOnly]
    private readonly Dictionary<NetworkObject, Tick> _timeoutPlayers = new();

    private readonly List<PlayerRef> _joinedPlayersQueue = new();
    private bool _canSendEvents;

    public event Action<NetworkObject> PlayerJoined;
    
    public event Action<NetworkObject> LocalPlayerJoined;
    
    public event Action<NetworkObject> PlayerLeft;

    public void EnableEvents()
    {
        _canSendEvents = true;
    }

    public void ReplayJoinPlayers()
    {
        foreach (NetworkObject playerObject in _allPlayers.Values)
            ((IPlayerJoined)this).PlayerJoined(playerObject.InputAuthority);
    }
    
    public void ReplayJoinPlayersHostMigration()
    {
        if (Runner.IsServer == false)
            throw new InvalidOperationException("Can't replay join players on client");
        
        int localPlayerToken = ReceiveConnectionToken(Runner.LocalPlayer);

        foreach (NetworkObject playerObject in _allPlayers.Values)
        {
            ConnectionTokenComponent tokenComponent = playerObject.GetComponent<ConnectionTokenComponent>();

            if (tokenComponent.Token == localPlayerToken)
                ((IPlayerJoined)this).PlayerJoined(Runner.LocalPlayer);
        }
    }

    void IPlayerJoined.PlayerJoined(PlayerRef playerRef)
    {
        if (Runner.IsServer)
        {
            if (Runner.GameMode == GameMode.Single)
                Instantiate(playerRef);
            else
                InstantiateWithToken(playerRef);
        }
        
        _joinedPlayersQueue.Add(playerRef);
    }

    void IPlayerLeft.PlayerLeft(PlayerRef playerRef)
    {
        if (Runner.IsServer)
        {
            NetworkObject playerObject = Runner.GetPlayerObject(playerRef);
            Runner.SetPlayerObject(playerRef, null);

            if (playerObject == null)
                return;
            
            Tick timeoutTick = CalculateTimeoutTick();
            _timeoutPlayers.Add(playerObject, timeoutTick);
        }
        
        if (_canSendEvents)
            PlayerLeft?.Invoke(GetPlayer(playerRef));
    }

    void IHostMigrationHandler.AfterMigration()
    {
        _allPlayers.Clear();
        _timeoutPlayers.Clear();
        
        EnableEvents();
            
        Tick timeoutTick = CalculateTimeoutTick();
            
        foreach (NetworkObject networkObject in Runner.GetAllNetworkObjects())
        {
            if (networkObject.TryGetComponent(out Player _) == false)
                continue;
                
            ConnectionTokenComponent tokenComponent = networkObject.GetComponent<ConnectionTokenComponent>();
            _allPlayers.Add(tokenComponent.Token, networkObject);
            _timeoutPlayers.Add(networkObject, timeoutTick);
        }
    }

    public override void FixedUpdateNetwork()
    {
        ProcessJoinedPlayers();
        ProcessTimeoutPlayers();
    }

    public bool TryGetLocalPlayer(out NetworkObject player) => 
        Runner.TryGetPlayerObject(Runner.LocalPlayer, out player);
    
    public bool TryGetPlayer(PlayerRef playerRef, out NetworkObject player) => 
        Runner.TryGetPlayerObject(playerRef, out player);

    public NetworkObject GetLocalPlayer() => Runner.GetPlayerObject(Runner.LocalPlayer);

    public NetworkObject GetPlayer(PlayerRef player) => Runner.GetPlayerObject(player);

    private void Instantiate(PlayerRef player)
    {
        NetworkObject playerObject;
        
        if (_allPlayers.Count > 0)
        {
            playerObject = _allPlayers.Values.First();
        }
        else
        {
            playerObject = Runner.Spawn(_playerPrefab, Vector3.zero, Quaternion.identity, player);
            _allPlayers.Add(0, playerObject);
        }
        
        Runner.SetPlayerObject(player, playerObject);
    }

    private void InstantiateWithToken(PlayerRef playerRef)
    {
        int token = ReceiveConnectionToken(playerRef);
                
        if (IsExistPlayer(token, out NetworkObject playerObject))
        {
            playerObject.AssignInputAuthority(playerRef);
            
            if (_timeoutPlayers.ContainsKey(playerObject))
                _timeoutPlayers.Remove(playerObject);
        }
        else
        {
            playerObject = Runner.Spawn(_playerPrefab, Vector3.zero, Quaternion.identity, playerRef);
            InitializeToken(playerObject, playerRef, token);
            _allPlayers.Add(token, playerObject);
        }
        
        Runner.SetPlayerObject(playerRef, playerObject);
    }

    private bool IsExistPlayer(int token, out NetworkObject context)
    {
        return _allPlayers.TryGetValue(token, out context);
    }

    private void InitializeToken(NetworkObject context, PlayerRef player, int token)
    {
        ConnectionTokenComponent tokenComponent = context.GetComponent<ConnectionTokenComponent>();
                    
        if (tokenComponent.TrySetToken(token) == false)
            throw new InvalidOperationException("Can't set token to player context.");
                    
        tokenComponent.ReplicateToAll(false);
        tokenComponent.ReplicateTo(player,true);
    }

    private void ProcessJoinedPlayers()
    {
        for (int i = _joinedPlayersQueue.Count - 1; i >= 0; i--)
        {
            PlayerRef playerRef = _joinedPlayersQueue[i];

            if (TryGetPlayer(playerRef, out NetworkObject playerObject))
            {
                if (_canSendEvents)
                {
                    PlayerJoined?.Invoke(playerObject);
                
                    if (Runner.LocalPlayer == playerRef)
                        LocalPlayerJoined?.Invoke(playerObject);
                }
                
                _joinedPlayersQueue.RemoveAt(i);
            }
        }
    }

    private void ProcessTimeoutPlayers()
    {
        if (Runner.IsServer == false)
            return;
        
        if (Runner.GameMode == GameMode.Single)
            return;

        if (_timeoutPlayers.Count == 0)
            return;

        Tick currentTick = Runner.Tick;

        foreach ((NetworkObject playerObject, Tick timeoutTick) in _timeoutPlayers.ToArray())
        {
            if (timeoutTick > currentTick)
                continue;

            _timeoutPlayers.Remove(playerObject);
            ConnectionTokenComponent tokenComponent = playerObject.GetComponent<ConnectionTokenComponent>();
            _allPlayers.Remove(tokenComponent.Token);
                
            Debug.Log($"Drop player by timeout: {playerObject.name}, currentTick = {currentTick}, " +
                      $"timeoutTick = {timeoutTick}");
            
            Runner.Despawn(playerObject);
        }
    }

    private Tick CalculateTimeoutTick()
    {
        return Runner.Tick + Mathf.RoundToInt(_disconnectTimeout / Runner.DeltaTime);
    }

    private int ReceiveConnectionToken(PlayerRef playerRef)
    {
        return BitConverter.ToInt32(Runner.GetPlayerConnectionToken(playerRef));
    }
}