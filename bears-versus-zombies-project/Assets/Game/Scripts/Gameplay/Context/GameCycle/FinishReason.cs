namespace SampleGame.Gameplay.Context
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