namespace SampleGame.App
{
    public struct NetworkRegionsData
    {
        public NetworkRegionsData(string currentRegion)
        {
            CurrentRegion = currentRegion;
        }
        
        public string CurrentRegion { get; }
    }
}