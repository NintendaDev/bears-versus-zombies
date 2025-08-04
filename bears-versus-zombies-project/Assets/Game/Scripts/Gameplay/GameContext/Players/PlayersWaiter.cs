using System.Collections.Generic;
using Fusion;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.GameContext
{
    public sealed class PlayersWaiter : NetworkBehaviour, IAfterSpawned
    {
        [SerializeField, MinValue(0)] private float _timeoutSeconds = 30f;

        private readonly ReactiveProperty<float> _countdown = new();
        private readonly List<InitializeComponent> _joinedPlayers = new();
        private PlayersService _playersService;
        private GameCycle _gameCycle;
        private bool _isDisabled;

        public ReadOnlyReactiveProperty<float> Countdown => _countdown;
        
        [Networked]
        private NetworkBool IsCompleted { get; set; }

        [Networked]
        private TickTimer TimeoutTimer { get; set; }
        
        public override void Spawned()
        {
            _gameCycle = GameContextService.Instance.Get<GameCycle>();
            _playersService = GameContextService.Instance.Get<PlayersService>();
            
            _playersService.PlayerJoined += OnPlayerJoin;
            _playersService.PlayerLeft += OnPlayerLeft;
        }

        void IAfterSpawned.AfterSpawned()
        {
            if (IsCompleted)
                return;
            
            TimeoutTimer = TickTimer.CreateFromSeconds(Runner, _timeoutSeconds);
            _gameCycle.StartWaitPlayers();
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            _playersService.PlayerJoined -= OnPlayerJoin;
            _playersService.PlayerLeft -= OnPlayerLeft; 
        }

        public override void FixedUpdateNetwork()
        {
            if (Runner.IsServer == false || IsCompleted)
                return;
            
            ProcessJoinedPlayers();

            if (TimeoutTimer.ExpiredOrNotRunning(Runner) == false)
                return;
            
            if (_gameCycle.IsWait)
            {
                _gameCycle.FinishGame(FinishReason.PlayersWaitTimeout);
                IsCompleted = true;
            }
        }

        public override void Render()
        {
            if (_isDisabled || TimeoutTimer.ExpiredOrNotRunning(Runner))
                return;
            
            _countdown.Value = TimeoutTimer.RemainingTime(Runner) ?? 0;
        }

        private void ProcessJoinedPlayers()
        {
            if (Runner.SessionInfo.MaxPlayers == 1 || _joinedPlayers.Count == Runner.SessionInfo.MaxPlayers)
            {
                for (int i = _joinedPlayers.Count - 1; i >= 0; i--)
                {
                    if (_joinedPlayers[i].IsInitialized == false)
                        return;
                }
                    
                _gameCycle.StartGame();
                IsCompleted = true;
            }
        }

        private void OnPlayerJoin(NetworkObject playerObject)
        {
            if (Runner.IsServer == false || IsCompleted)
                return;
            
            _joinedPlayers.Add(playerObject.GetComponent<InitializeComponent>());
        }

        private void OnPlayerLeft(NetworkObject playerObject)
        {
            if (Runner.IsServer == false || IsCompleted)
                return;
            
            _joinedPlayers.Remove(playerObject.GetComponent<InitializeComponent>());
        }
    }
}