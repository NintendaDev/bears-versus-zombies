using System;
using System.Collections.Generic;
using UnityEngine;

namespace OM
{
    /// <summary>
    /// Provides static helper methods for working with the EasingFunction-related enums.
    /// Primarily facilitates iterating through the different categories of easing functions (All, Idle, PingPong)
    /// and includes convenience wrappers for Unity's unclamped Lerp functions.
    /// </summary>
    public static class EasingHelper
    {
        // --- Cached Enum Value Lists ---
        // These lists store the results of Enum.GetValues to avoid repeated reflection calls,
        // improving performance when iterating through the enums multiple times.

        /// <summary>Cached list of all values from the main EasingFunction enum.</summary>
        private static List<EasingFunction> _easingFunctions;
        /// <summary>Cached list of all values from the EasingFunctionPingPong enum.</summary>
        private static List<EasingFunctionPingPong> _easingFunctionsPingPong;
        /// <summary>Cached list of all values from the EasingFunctionIdle enum.</summary>
        private static List<EasingFunctionIdle> _easingFunctionsIdle;

        // --- Enum Iteration Methods ---

        /// <summary>
        /// Returns an enumerable collection of all values defined in the main EasingFunction enum.
        /// Caches the result on the first call for subsequent efficiency.
        /// </summary>
        /// <returns>An IEnumerable containing all EasingFunction enum values.</returns>
        public static IEnumerable<EasingFunction> LoopThroughEasingFunctions()
        {
            // If the list is already populated, return the cached version.
            if (_easingFunctions != null && _easingFunctions.Count != 0) return _easingFunctions;

            // If not cached, create the list and populate it using reflection.
            _easingFunctions = new List<EasingFunction>();
            foreach (EasingFunction easingFunction in Enum.GetValues(typeof(EasingFunction)))
            {
                _easingFunctions.Add(easingFunction);
            }
            return _easingFunctions;
        }

        /// <summary>
        /// Returns an enumerable collection of all values defined in the EasingFunctionPingPong enum.
        /// Caches the result on the first call for subsequent efficiency.
        /// </summary>
        /// <returns>An IEnumerable containing all EasingFunctionPingPong enum values.</returns>
        public static IEnumerable<EasingFunctionPingPong> LoopThroughEasingFunctionsPingPong()
        {
            // Use cached list if available.
            if (_easingFunctionsPingPong != null && _easingFunctionsPingPong.Count != 0) return _easingFunctionsPingPong;

            // Create and populate the list on first call.
            _easingFunctionsPingPong = new List<EasingFunctionPingPong>();
            foreach (EasingFunctionPingPong easingFunction in Enum.GetValues(typeof(EasingFunctionPingPong)))
            {
                _easingFunctionsPingPong.Add(easingFunction);
            }
            return _easingFunctionsPingPong;
        }

        /// <summary>
        /// Returns an enumerable collection of all values defined in the EasingFunctionIdle enum (non-PingPong functions).
        /// Caches the result on the first call for subsequent efficiency.
        /// </summary>
        /// <returns>An IEnumerable containing all EasingFunctionIdle enum values.</returns>
        public static IEnumerable<EasingFunctionIdle> LoopThroughEasingFunctionsIdle()
        {
            // Use cached list if available.
            if (_easingFunctionsIdle != null && _easingFunctionsIdle.Count != 0) return _easingFunctionsIdle;

            // Create and populate the list on first call.
            _easingFunctionsIdle = new List<EasingFunctionIdle>();
            foreach (EasingFunctionIdle easingFunction in Enum.GetValues(typeof(EasingFunctionIdle)))
            {
                _easingFunctionsIdle.Add(easingFunction);
            }
            return _easingFunctionsIdle;
        }

        // --- Unclamped Lerp Wrappers ---
        // These methods simply provide a consistent "Lerp" name while calling the
        // corresponding "LerpUnclamped" methods from Unity. Unclamped versions allow
        // the interpolation factor 't' to go outside the 0-1 range, which can be useful
        // for effects like overshoot when combined with certain easing functions (though
        // the main EaseData.Evaluate method usually clamps 't' beforehand).

        /// <summary>Performs linear interpolation between two Vector2 values without clamping the factor 't'.</summary>
        /// <param name="a">Start value.</param> <param name="b">End value.</param> <param name="t">Interpolation factor.</param> <returns>Interpolated Vector2.</returns>
        public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
        {
            return Vector2.LerpUnclamped(a, b, t);
        }

        /// <summary>Performs linear interpolation between two Vector3 values without clamping the factor 't'.</summary>
        /// <param name="a">Start value.</param> <param name="b">End value.</param> <param name="t">Interpolation factor.</param> <returns>Interpolated Vector3.</returns>
        public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            return Vector3.LerpUnclamped(a, b, t);
        }

        /// <summary>Performs linear interpolation between two Quaternion values without clamping the factor 't'.</summary>
        /// <param name="a">Start value.</param> <param name="b">End value.</param> <param name="t">Interpolation factor.</param> <returns>Interpolated Quaternion.</returns>
        public static Quaternion Lerp(Quaternion a, Quaternion b, float t)
        {
            return Quaternion.LerpUnclamped(a, b, t);
        }

        /// <summary>Performs linear interpolation between two Color values without clamping the factor 't'.</summary>
        /// <param name="a">Start value.</param> <param name="b">End value.</param> <param name="t">Interpolation factor.</param> <returns>Interpolated Color.</returns>
        public static Color Lerp(Color a, Color b, float t)
        {
            return Color.LerpUnclamped(a, b, t);
        }

        /// <summary>Performs linear interpolation between two float values without clamping the factor 't'.</summary>
        /// <param name="a">Start value.</param> <param name="b">End value.</param> <param name="t">Interpolation factor.</param> <returns>Interpolated float.</returns>
        public static float Lerp(float a, float b, float t)
        {
            return Mathf.LerpUnclamped(a, b, t);
        }

        /// <summary>Performs linear interpolation between two angles (in degrees) correctly handling wrapping. Uses Mathf.LerpAngle.</summary>
        /// <param name="a">Start angle (degrees).</param> <param name="b">End angle (degrees).</param> <param name="t">Interpolation factor (clamped 0-1 by LerpAngle).</param> <returns>Interpolated angle (degrees).</returns>
        public static float LerpAngle(float a, float b, float t)
        {
            // Note: Mathf.LerpAngle inherently clamps 't' between 0 and 1.
            return Mathf.LerpAngle(a, b, t);
        }

        /// <summary>Performs linear interpolation between two Vector4 values without clamping the factor 't'.</summary>
        /// <param name="a">Start value.</param> <param name="b">End value.</param> <param name="t">Interpolation factor.</param> <returns>Interpolated Vector4.</returns>
        public static Vector4 Lerp(Vector4 a, Vector4 b, float t)
        {
            return Vector4.LerpUnclamped(a, b, t);
        }

        /// <summary>Performs linear interpolation between two Bounds values by interpolating their centers and sizes.</summary>
        /// <param name="a">Start Bounds.</param> <param name="b">End Bounds.</param> <param name="t">Interpolation factor.</param> <returns>Interpolated Bounds.</returns>
        public static Bounds Lerp(Bounds a, Bounds b, float t)
        {
            // Interpolate center and size separately using the Vector3 Lerp wrapper.
            return new Bounds(Lerp(a.center, b.center, t),
                              Lerp(a.size, b.size, t));
        }

        /// <summary>Performs linear interpolation between two Rect values by interpolating their positions and sizes.</summary>
        /// <param name="a">Start Rect.</param> <param name="b">End Rect.</param> <param name="t">Interpolation factor.</param> <returns>Interpolated Rect.</returns>
        public static Rect Lerp(Rect a, Rect b, float t)
        {
            // Interpolate position and size separately using the Vector2 Lerp wrapper.
            return new Rect(Lerp(a.position, b.position, t),
                            Lerp(a.size, b.size, t));
        }
    }
}