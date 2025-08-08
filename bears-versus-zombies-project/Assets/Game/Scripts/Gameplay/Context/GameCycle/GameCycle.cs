using System;
using Fusion;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.Context
{
    public sealed class GameCycle : NetworkBehaviour, IGameCycleState
    {
        private bool _isSpawned;

        [ShowInInspector, Sirenix.OdinInspector.ReadOnly]
        public GameState State => (_isSpawned) ? NetworkedGameState.GameState : GameState.None;

        [ShowInInspector, Sirenix.OdinInspector.ReadOnly]
        public FinishReason FinishReason => (_isSpawned) ? NetworkedGameState.FinishReason : FinishReason.None;

        public bool IsWait => State == GameState.Waiting;

        public bool IsPlaying => State == GameState.Playing;

        public bool IsFinished => State == GameState.Finished;

        [Networked, OnChangedRender(nameof(OnGameStateRender))]
        private GameStateData NetworkedGameState { get; set; }

        public event Action<GameState, FinishReason> Changed;

        public override void Spawned()
        {
            _isSpawned = true;
            OnGameStateRender();
        }

        public void StartWaitPlayers()
        {
            if (HasStateAuthority == false || State != GameState.None)
                return;
            
            NetworkedGameState = new GameStateData { GameState = GameState.Waiting };
        }

        public void StartGame()
        {
            if (HasStateAuthority == false || IsWait == false)
                return;
            
            NetworkedGameState = new GameStateData { GameState = GameState.Playing };
        }

        public void FinishGame(FinishReason reason)
        {
            if (HasStateAuthority == false || (IsWait == false && IsPlaying == false))
                return;
            
            NetworkedGameState = new GameStateData { GameState = GameState.Finished, FinishReason = reason };
        }
        
        private void OnGameStateRender()
        {
            Changed?.Invoke(NetworkedGameState.GameState, NetworkedGameState.FinishReason);
        }
    }
}