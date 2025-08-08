using System;

namespace SampleGame.Gameplay.Context
{
    public interface IGameInitializeEvent
    {
        public bool IsInitialized { get; }
        
        public event Action Initialized;
    }
}