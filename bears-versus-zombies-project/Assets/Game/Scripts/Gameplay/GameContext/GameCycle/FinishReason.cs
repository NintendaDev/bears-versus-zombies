namespace SampleGame.Gameplay.GameContext
{
    public enum FinishReason
    {
        None,
        Win,
        BusDestroyed,
        PlayerDie,
        PlayersWaitTimeout,
        PlayerDisconnect,
        NetworkError,
    }
}