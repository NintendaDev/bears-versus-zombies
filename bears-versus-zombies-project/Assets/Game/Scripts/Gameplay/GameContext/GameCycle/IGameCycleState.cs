using System;

namespace SampleGame.Gameplay.GameContext
{
    public interface IGameCycleState
    {
        public GameState State { get; }
        
        public FinishReason FinishReason { get; }
        
        public bool IsWait { get; }
        
        public bool IsPlaying { get; }
        
        public bool IsFinished { get; }
        
        public event Action<GameState, FinishReason> Changed;
    }
}