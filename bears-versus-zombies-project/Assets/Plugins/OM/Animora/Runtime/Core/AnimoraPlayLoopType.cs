namespace OM.Animora.Runtime
{
    /// <summary>
    /// Defines the different looping behaviors available for an <see cref="AnimoraPlayer"/> timeline.
    /// Determines what happens when the playback reaches the end of the timeline duration.
    /// </summary>
    public enum AnimoraPlayLoopType
    {
        /// <summary>
        /// When the timeline reaches the end, it immediately restarts playback from the beginning
        /// in the <see cref="AnimoraPlayer.defaultPlayDirection"/>. This continues until the specified
        /// <see cref="AnimoraPlayer.loopCount"/> is reached (or indefinitely if loopCount is -1).
        /// </summary>
        Loop,

        /// <summary>
        /// When the timeline reaches the end (either forward or backward), the playback direction reverses.
        /// Playback continues back and forth until the specified <see cref="AnimoraPlayer.loopCount"/> is reached
        /// (each full back-and-forth counts as one loop, or check specific implementation details), or indefinitely if loopCount is -1.
        /// </summary>
        PingPong,

        /// <summary>
        /// The timeline plays through exactly once from start to end based on the initial direction.
        /// Playback stops automatically after reaching the end. The <see cref="AnimoraPlayer.loopCount"/> setting is ignored.
        /// </summary>
        Once,
    }
}