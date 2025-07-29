using Fusion;

namespace SampleGame.Gameplay.GameContext
{
    public struct GameStateData : INetworkStruct
    {
        public GameState GameState;
        
        public FinishReason FinishReason;

        public static GameStateData CopyFrom(GameStateData data)
        {
            return new GameStateData { GameState = data.GameState, FinishReason = data.FinishReason };
        }
    }
}