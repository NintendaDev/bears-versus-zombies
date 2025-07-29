using UnityEngine;

namespace OM
{
    /// <summary>
    /// Provides static helper methods for logging messages, warnings, and errors.
    /// These methods wrap Unity's standard Debug logging functions but add features like:
    /// - Conditional compilation using `#if DEBUG`: Logs will only be compiled and executed
    ///   if the 'DEBUG' symbol is defined (typically in Unity Editor and Development builds).
    /// - Optional `canLog` flag: Allows disabling specific log calls even within a DEBUG build.
    /// - Contextual information: Appends the type name of the provided context object to the log message.
    /// - Unity Object context: If the context is a `UnityEngine.Object`, it's passed to the Unity
    ///   Debug methods, allowing users to click the log message in the console to ping the object.
    /// </summary>
    public static class OM_Debug
    {
        /// <summary>
        /// Logs a standard message to the Unity console, conditionally compiled for DEBUG builds.
        /// </summary>
        /// <param name="message">The message string to log.</param>
        /// <param name="canLog">If false, the log message will be skipped (even in DEBUG builds).</param>
        /// <param name="context">Optional object to provide context. Its type name is appended, and if it's a Unity Object, it allows pinging.</param>
        public static void Log(string message, bool canLog, object context = null)
        {
            // This entire block is stripped out in non-DEBUG builds.
#if DEBUG
            // Skip logging if the specific call is disabled.
            if (!canLog) return;

            // Handle logging based on the type of context provided.
            switch (context)
            {
                case null:
                    // No context, just log the message.
                    Debug.Log(message);
                    break;
                case Object o: // Check if it's specifically a UnityEngine.Object
                    // Log the message and pass the Unity Object as context for pinging.
                    // Append the type name for clarity.
                    Debug.Log(message + $" <color=grey>({o.GetType().Name})</color>", o); // Added color tag for better visibility
                    break;
                default:
                    // Context is not null and not a UnityEngine.Object.
                    // Log the message and append the type name.
                    Debug.Log(message + $" <color=grey>({context.GetType().Name})</color>"); // Added color tag
                    break;
            }
#endif
        }

        /// <summary>
        /// Logs a warning message to the Unity console, conditionally compiled for DEBUG builds.
        /// </summary>
        /// <param name="message">The warning message string to log.</param>
        /// <param name="canLog">If false, the log message will be skipped (even in DEBUG builds).</param>
        /// <param name="context">Optional object to provide context. Its type name is appended, and if it's a Unity Object, it allows pinging.</param>
        public static void LogWarning(string message, bool canLog, object context = null)
        {
#if DEBUG
            if (!canLog) return;

            switch (context)
            {
                case null:
                    Debug.LogWarning(message);
                    break;
                case Object o:
                    Debug.LogWarning(message + $" <color=grey>({o.GetType().Name})</color>", o); // Added color tag
                    break;
                default:
                    Debug.LogWarning(message + $" <color=grey>({context.GetType().Name})</color>"); // Added color tag
                    break;
            }
#endif
        }

        /// <summary>
        /// Logs an error message to the Unity console, conditionally compiled for DEBUG builds.
        /// </summary>
        /// <param name="message">The error message string to log.</param>
        /// <param name="canLog">If false, the log message will be skipped (even in DEBUG builds).</param>
        /// <param name="context">Optional object to provide context. Its type name is appended, and if it's a Unity Object, it allows pinging.</param>
        public static void LogError(string message, bool canLog, object context = null)
        {
#if DEBUG
            if (!canLog) return;

            switch (context)
            {
                case null:
                    Debug.LogError(message);
                    break;
                case Object o:
                    Debug.LogError(message + $" <color=grey>({o.GetType().Name})</color>", o); // Added color tag
                    break;
                default:
                    Debug.LogError(message + $" <color=grey>({context.GetType().Name})</color>"); // Added color tag
                    break;
            }
#endif
        }
    }
}