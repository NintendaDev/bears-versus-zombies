using Modules.EventBus;

namespace SampleGame.App.UI
{
    public struct SessionRowClickSignal : IPayloadSignal
    {
        public SessionRowClickSignal(string sessionName)
        {
            SessionName = sessionName;
        }
        
        public string SessionName { get; }
    }
}