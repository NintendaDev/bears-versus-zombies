using UnityEditor;

namespace OM.Editor
{
    /// <summary>
    /// Automatically clears all events in OM_EventsManager when exiting play mode.
    /// Ensures a clean event state for the next play session.
    /// </summary>
    [InitializeOnLoad]
    public static class OM_EventsManagerClearAllEventsOnStop
    {
        /// <summary>
        /// Static constructor that subscribes to Unity's play mode state change event.
        /// </summary>
        static OM_EventsManagerClearAllEventsOnStop()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        /// <summary>
        /// Called when the play mode state changes.
        /// Clears all registered events if exiting play mode.
        /// </summary>
        /// <param name="state">The current play mode state.</param>
        private static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                OM_EventsManager.ClearAllEvents();
            }
        }
    }
}