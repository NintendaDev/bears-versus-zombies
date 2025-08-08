using Modules.EventBus;

namespace SampleGame.App.UI
{
    public struct SessionRowDropSignal : IPayloadSignal
    {
        public SessionRowDropSignal(string sessionName)
        {
            SessionName = sessionName;
        }
        
        public string SessionName { get; }
    }
}