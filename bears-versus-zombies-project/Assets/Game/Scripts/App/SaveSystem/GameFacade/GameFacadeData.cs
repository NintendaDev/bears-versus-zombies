namespace SampleGame.App
{
    public struct GameFacadeData
    {
        public GameFacadeData(string currentRegion)
        {
            CurrentRegion = currentRegion;
        }
        
        public string CurrentRegion { get; }
    }
}