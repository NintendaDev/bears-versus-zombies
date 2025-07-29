namespace OM.Animora.Runtime
{
    /// <summary>
    /// Defines the conditions under which an <see cref="AnimoraPlayer"/> automatically
    /// initiates playback of its timeline.
    /// </summary>
    public enum AnimoraPlayMode
    {
        /// <summary>
        /// Playback starts automatically when the Unity `Start()` message is called on the
        /// <see cref="AnimoraPlayer"/> component (typically once after the object is initialized in the scene).
        /// </summary>
        OnStart,

        /// <summary>
        /// Playback starts automatically whenever the Unity `OnEnable()` message is called on the
        /// <see cref="AnimoraPlayer"/> component (e.g., when the GameObject is activated, or after recompilation in the editor if the object is active).
        /// </summary>
        OnEnable,

        /// <summary>
        /// Playback does not start automatically. It must be initiated manually by calling
        /// a method like <see cref="AnimoraPlayer.PlayAnimation()"/> or related playback methods
        /// via script or editor interaction.
        /// </summary>
        Manual,
    }
}