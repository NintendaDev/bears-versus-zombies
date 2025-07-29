namespace OM
{
    /// <summary>
    /// Generic interface for listening to typed events.
    /// </summary>
    /// <typeparam name="T">The type of event data to receive.</typeparam>
    public interface IOM_EventListener<in T>
    {
        /// <summary>
        /// Called when the event is published.
        /// </summary>
        /// <param name="eventData">The data associated with the event.</param>
        void OnEvent(T eventData);
    }
}