using Modules.EventBus;

namespace SampleGame.App.UI
{
    public struct SessionButtonDropSignal : IPayloadSignal
    {
        public SessionButtonDropSignal(string sessionName)
        {
            SessionName = sessionName;
        }
        
        public string SessionName { get; }
    }
}