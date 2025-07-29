namespace OM
{
    /// <summary>
    /// Defines the direction in which something can play.
    /// </summary>
    public enum OM_PlayDirection
    {
        Forward,
        Backward
    }

    /// <summary>
    /// Extension methods for the OM_PlayDirection enum to simplify logic.
    /// </summary>
    public static class OM_PlayDirectionExtension
    {
        /// <summary>
        /// Checks if the direction is Forward.
        /// </summary>
        /// <param name="direction">The direction to check.</param>
        /// <returns>True if Forward.</returns>
        public static bool IsForward(this OM_PlayDirection direction)
        {
            return direction == OM_PlayDirection.Forward;
        }

        /// <summary>
        /// Checks if the direction is Backward.
        /// </summary>
        /// <param name="direction">The direction to check.</param>
        /// <returns>True if Backward.</returns>
        public static bool IsBackward(this OM_PlayDirection direction)
        {
            return direction == OM_PlayDirection.Backward;
        }

        /// <summary>
        /// Returns 1 for Forward and -1 for Backward.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <returns>1 or -1 based on direction.</returns>
        public static float GetDirectionMultiplier(this OM_PlayDirection direction)
        {
            return direction.IsForward() ? 1 : -1;
        }

        /// <summary>
        /// Determines if a timeline has finished based on the direction and elapsed time.
        /// </summary>
        /// <param name="direction">The current direction.</param>
        /// <param name="elapsedTime">Elapsed time.</param>
        /// <param name="duration">Total duration.</param>
        /// <returns>True if finished.</returns>
        public static bool IsFinished(this OM_PlayDirection direction, float elapsedTime, float duration)
        {
            return direction.IsForward() ? elapsedTime >= duration : elapsedTime <= 0;
        }

        /// <summary>
        /// Reverses the current direction.
        /// </summary>
        /// <param name="direction">The direction to reverse.</param>
        /// <returns>The opposite direction.</returns>
        public static OM_PlayDirection Reverse(this OM_PlayDirection direction)
        {
            return direction == OM_PlayDirection.Forward ? OM_PlayDirection.Backward : OM_PlayDirection.Forward;
        }
    }
}
