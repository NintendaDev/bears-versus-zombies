using System;

namespace SampleGame.Gameplay.GameContext
{
    public interface IGameInitializeEvent
    {
        public bool IsInitialized { get; }
        
        public event Action Initialized;
    }
}