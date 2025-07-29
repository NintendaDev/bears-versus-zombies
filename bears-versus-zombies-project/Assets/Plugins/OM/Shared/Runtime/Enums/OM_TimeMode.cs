using UnityEngine;

namespace OM
{
    /// <summary>
    /// Specifies whether to use scaled or unscaled time.
    /// </summary>
    public enum OM_TimeMode
    {
        /// <summary>
        /// Uses unscaled time (not affected by Time.timeScale).
        /// </summary>
        UnscaledTime = 0,

        /// <summary>
        /// Uses scaled time (affected by Time.timeScale).
        /// </summary>
        ScaledTime = 1
    }

    /// <summary>
    /// Extension methods for OM_TimeMode.
    /// </summary>
    public static class OM_TimeModeExtension
    {
        /// <summary>
        /// Returns deltaTime based on the selected OM_TimeMode.
        /// </summary>
        /// <param name="timeMode">The time mode to use.</param>
        /// <returns>Time.deltaTime or Time.unscaledDeltaTime depending on the mode.</returns>
        public static float GetDeltaTime(this OM_TimeMode timeMode)
        {
            return timeMode == OM_TimeMode.ScaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
        }
    }
}