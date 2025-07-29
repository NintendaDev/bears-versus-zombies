
using System;

namespace OM
{
    /// <summary>
    /// Provides extension methods for the EasingFunction-related enums
    /// (`EasingFunctionIdle`, `EasingFunctionPingPong`).
    /// These methods allow easy conversion from the specific enum types (Idle/PingPong)
    /// back to the comprehensive `EasingFunction` enum used by the `EaseLibrary`.
    /// </summary>
    public static class EasingLibraryExtension
    {
        /// <summary>
        /// Converts an `EasingFunctionIdle` enum value to its corresponding `EasingFunction` value.
        /// </summary>
        /// <param name="idle">The EasingFunctionIdle value to convert.</param>
        /// <returns>The equivalent EasingFunction value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the input `idle` value does not have a corresponding mapping in the `EasingFunction` enum (should not happen if enums are kept synchronized).</exception>
        public static EasingFunction CastToEasingFunction(this EasingFunctionIdle idle)
        {
            // Use a switch expression for concise mapping.
            return idle switch
            {
                // Map each Idle value to its direct counterpart in the main EasingFunction enum.
                EasingFunctionIdle.Linear => EasingFunction.Linear,
                EasingFunctionIdle.InSine => EasingFunction.InSine,
                EasingFunctionIdle.OutSine => EasingFunction.OutSine,
                EasingFunctionIdle.InOutSine => EasingFunction.InOutSine,
                EasingFunctionIdle.InCubic => EasingFunction.InCubic,
                EasingFunctionIdle.OutCubic => EasingFunction.OutCubic,
                EasingFunctionIdle.InOutCubic => EasingFunction.InOutCubic,
                EasingFunctionIdle.InQuint => EasingFunction.InQuint,
                EasingFunctionIdle.OutQuint => EasingFunction.OutQuint,
                EasingFunctionIdle.InOutQuint => EasingFunction.InOutQuint,
                EasingFunctionIdle.InCirc => EasingFunction.InCirc,
                EasingFunctionIdle.OutCirc => EasingFunction.OutCirc,
                EasingFunctionIdle.InOutCirc => EasingFunction.InOutCirc,
                EasingFunctionIdle.InElastic => EasingFunction.InElastic,
                EasingFunctionIdle.OutElastic => EasingFunction.OutElastic,
                EasingFunctionIdle.InOutElastic => EasingFunction.InOutElastic,
                EasingFunctionIdle.InQuart => EasingFunction.InQuart,
                EasingFunctionIdle.OutQuart => EasingFunction.OutQuart,
                EasingFunctionIdle.InOutQuart => EasingFunction.InOutQuart,
                EasingFunctionIdle.InExpo => EasingFunction.InExpo,
                EasingFunctionIdle.OutExpo => EasingFunction.OutExpo,
                EasingFunctionIdle.InOutExpo => EasingFunction.InOutExpo,
                EasingFunctionIdle.InQuad => EasingFunction.InQuad,
                EasingFunctionIdle.OutQuad => EasingFunction.OutQuad,
                EasingFunctionIdle.InOutQuad => EasingFunction.InOutQuad,
                EasingFunctionIdle.InBack => EasingFunction.InBack,
                EasingFunctionIdle.OutBack => EasingFunction.OutBack,
                EasingFunctionIdle.InOutBack => EasingFunction.InOutBack,
                EasingFunctionIdle.InBounce => EasingFunction.InBounce,
                EasingFunctionIdle.OutBounce => EasingFunction.OutBounce,
                EasingFunctionIdle.InOutBounce => EasingFunction.InOutBounce,
                // Fallback: If a value exists in EasingFunctionIdle but not here, throw an exception.
                _ => throw new ArgumentOutOfRangeException(nameof(idle), idle, "Enum value not mapped.")
            };
        }

        /// <summary>
        /// Converts an `EasingFunctionPingPong` enum value to its corresponding `EasingFunction` value.
        /// </summary>
        /// <param name="pingPong">The EasingFunctionPingPong value to convert.</param>
        /// <returns>The equivalent EasingFunction value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the input `pingPong` value does not have a corresponding mapping in the `EasingFunction` enum (should not happen if enums are kept synchronized).</exception>
        public static EasingFunction CastToEasingFunction(this EasingFunctionPingPong pingPong)
        {
            // Use a switch expression for concise mapping.
            return pingPong switch
            {
                 // Map each PingPong value to its direct counterpart in the main EasingFunction enum.
                EasingFunctionPingPong.LinearPingPong => EasingFunction.LinearPingPong,
                EasingFunctionPingPong.InSinePingPong => EasingFunction.InSinePingPong,
                EasingFunctionPingPong.OutSinePingPong => EasingFunction.OutSinePingPong,
                EasingFunctionPingPong.InOutSinePingPong => EasingFunction.InOutSinePingPong,
                EasingFunctionPingPong.InCubicPingPong => EasingFunction.InCubicPingPong,
                EasingFunctionPingPong.OutCubicPingPong => EasingFunction.OutCubicPingPong,
                EasingFunctionPingPong.InOutCubicPingPong => EasingFunction.InOutCubicPingPong,
                EasingFunctionPingPong.InQuintPingPong => EasingFunction.InQuintPingPong,
                EasingFunctionPingPong.OutQuintPingPong => EasingFunction.OutQuintPingPong,
                EasingFunctionPingPong.InOutQuintPingPong => EasingFunction.InOutQuintPingPong,
                EasingFunctionPingPong.InCircPingPong => EasingFunction.InCircPingPong,
                EasingFunctionPingPong.OutCircPingPong => EasingFunction.OutCircPingPong,
                EasingFunctionPingPong.InOutCircPingPong => EasingFunction.InOutCircPingPong,
                EasingFunctionPingPong.InElasticPingPong => EasingFunction.InElasticPingPong,
                EasingFunctionPingPong.OutElasticPingPong => EasingFunction.OutElasticPingPong,
                EasingFunctionPingPong.InOutElasticPingPong => EasingFunction.InOutElasticPingPong,
                EasingFunctionPingPong.InQuartPingPong => EasingFunction.InQuartPingPong,
                EasingFunctionPingPong.OutQuartPingPong => EasingFunction.OutQuartPingPong,
                EasingFunctionPingPong.InOutQuartPingPong => EasingFunction.InOutQuartPingPong,
                EasingFunctionPingPong.InExpoPingPong => EasingFunction.InExpoPingPong,
                EasingFunctionPingPong.OutExpoPingPong => EasingFunction.OutExpoPingPong,
                EasingFunctionPingPong.InOutExpoPingPong => EasingFunction.InOutExpoPingPong,
                EasingFunctionPingPong.InQuadPingPong => EasingFunction.InQuadPingPong,
                EasingFunctionPingPong.OutQuadPingPong => EasingFunction.OutQuadPingPong,
                EasingFunctionPingPong.InOutQuadPingPong => EasingFunction.InOutQuadPingPong,
                EasingFunctionPingPong.InBackPingPong => EasingFunction.InBackPingPong,
                EasingFunctionPingPong.OutBackPingPong => EasingFunction.OutBackPingPong,
                EasingFunctionPingPong.InOutBackPingPong => EasingFunction.InOutBackPingPong,
                EasingFunctionPingPong.InBouncePingPong => EasingFunction.InBouncePingPong,
                EasingFunctionPingPong.OutBouncePingPong => EasingFunction.OutBouncePingPong,
                EasingFunctionPingPong.InOutBouncePingPong => EasingFunction.InOutBouncePingPong,
                // Fallback: If a value exists in EasingFunctionPingPong but not here, throw an exception.
                _ => throw new ArgumentOutOfRangeException(nameof(pingPong), pingPong, "Enum value not mapped.")
            };
        }
    }
}