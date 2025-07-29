using Modules.EventBus;

namespace SampleGame.App.UI
{
    public struct SessionButtonClickSignal : IPayloadSignal
    {
        public SessionButtonClickSignal(string sessionName)
        {
            SessionName = sessionName;
        }
        
        public string SessionName { get; }
    }
}