using System;
using System.Collections.Generic;
using UnityEngine;

namespace OM
{
    /// <summary>
    /// Provides a static library of mathematical easing functions based on Robert Penner's easing equations.
    /// Includes standard easing types (Linear, Sine, Cubic, etc.) with In, Out, and InOut variations,
    /// as well as Elastic, Back, and Bounce effects. Also provides PingPong versions of these functions.
    /// Allows retrieval of function delegates and direct evaluation.
    /// </summary>
    public static class EaseLibrary
    {
        // --- Constants used in easing calculations ---
        private const float Pi = Mathf.PI;
        private const float TwoPi = 2 * Mathf.PI;
        private const float S = 1.70158f; // Constant used in Back easing functions (overshoot amount)
        private const float BounceFactor = 7.5625f; // Constant factor used in Bounce calculations
        private const float Divisor = 1 / 2.75f; // Constant divisor used in Bounce calculations

        /// <summary>
        /// Dictionary mapping EasingFunction enum values to their corresponding static implementation methods.
        /// Used internally by GetEasingFunction and Evaluate for efficient function lookup.
        /// </summary>
        private static readonly Dictionary<EasingFunction, Func<float, float>> EasingFunctions = new()
        {
            // Linear
            { EasingFunction.Linear, EaseLinear },
            // Sine
            { EasingFunction.InSine, EaseInSine },
            { EasingFunction.OutSine, EaseOutSine },
            { EasingFunction.InOutSine, EaseInOutSine },
            // Cubic
            { EasingFunction.InCubic, EaseInCubic },
            { EasingFunction.OutCubic, EaseOutCubic },
            { EasingFunction.InOutCubic, EaseInOutCubic },
            // Quint
            { EasingFunction.InQuint, EaseInQuint },
            { EasingFunction.OutQuint, EaseOutQuint },
            { EasingFunction.InOutQuint, EaseInOutQuint },
            // Circ
            { EasingFunction.InCirc, EaseInCirc },
            { EasingFunction.OutCirc, EaseOutCirc },
            { EasingFunction.InOutCirc, EaseInOutCirc },
            // Elastic
            { EasingFunction.InElastic, EaseInElastic },
            { EasingFunction.OutElastic, EaseOutElastic },
            { EasingFunction.InOutElastic, EaseInOutElastic },
            // Quart
            { EasingFunction.InQuart, EaseInQuart },
            { EasingFunction.OutQuart, EaseOutQuart },
            { EasingFunction.InOutQuart, EaseInOutQuart },
            // Expo
            { EasingFunction.InExpo, EaseInExpo },
            { EasingFunction.OutExpo, EaseOutExpo },
            { EasingFunction.InOutExpo, EaseInOutExpo },
            // Quad
            { EasingFunction.InQuad, EaseInQuad },
            { EasingFunction.OutQuad, EaseOutQuad },
            { EasingFunction.InOutQuad, EaseInOutQuad },
            // Back
            { EasingFunction.InBack, EaseInBack },
            { EasingFunction.OutBack, EaseOutBack },
            { EasingFunction.InOutBack, EaseInOutBack },
            // Bounce
            { EasingFunction.InBounce, EaseInBounce },
            { EasingFunction.OutBounce, EaseOutBounce },
            { EasingFunction.InOutBounce, EaseInOutBounce },

            // --- PingPong Versions ---
            { EasingFunction.LinearPingPong, EaseLinearPingPong },
            { EasingFunction.InSinePingPong, EaseInSinePingPong },
            { EasingFunction.OutSinePingPong, EaseOutSinePingPong },
            { EasingFunction.InOutSinePingPong, EaseInOutSinePingPong },
            { EasingFunction.InCubicPingPong, EaseInCubicPingPong },
            { EasingFunction.OutCubicPingPong, EaseOutCubicPingPong },
            { EasingFunction.InOutCubicPingPong, EaseInOutCubicPingPong },
            { EasingFunction.InQuintPingPong, EaseInQuintPingPong },
            { EasingFunction.OutQuintPingPong, EaseOutQuintPingPong },
            { EasingFunction.InOutQuintPingPong, EaseInOutQuintPingPong },
            { EasingFunction.InCircPingPong, EaseInCircPingPong },
            { EasingFunction.OutCircPingPong, EaseOutCircPingPong },
            { EasingFunction.InOutCircPingPong, EaseInOutCircPingPong },
            { EasingFunction.InElasticPingPong, EaseInElasticPingPong },
            { EasingFunction.OutElasticPingPong, EaseOutElasticPingPong },
            { EasingFunction.InOutElasticPingPong, EaseInOutElasticPingPong },
            { EasingFunction.InQuartPingPong, EaseInQuartPingPong },
            { EasingFunction.OutQuartPingPong, EaseOutQuartPingPong },
            { EasingFunction.InOutQuartPingPong, EaseInOutQuartPingPong },
            { EasingFunction.InExpoPingPong, EaseInExpoPingPong },
            { EasingFunction.OutExpoPingPong, EaseOutExpoPingPong },
            { EasingFunction.InOutExpoPingPong, EaseInOutExpoPingPong },
            { EasingFunction.InQuadPingPong, EaseInQuadPingPong },
            { EasingFunction.OutQuadPingPong, EaseOutQuadPingPong },
            { EasingFunction.InOutQuadPingPong, EaseInOutQuadPingPong },
            { EasingFunction.InBackPingPong, EaseInBackPingPong },
            { EasingFunction.OutBackPingPong, EaseOutBackPingPong },
            { EasingFunction.InOutBackPingPong, EaseInOutBackPingPong },
            { EasingFunction.InBouncePingPong, EaseInBouncePingPong },
            { EasingFunction.OutBouncePingPong, EaseOutBouncePingPong },
            { EasingFunction.InOutBouncePingPong, EaseInOutBouncePingPong }
        };

        /// <summary>
        /// Gets the delegate (Func<float, float>) for the specified easing function type.
        /// </summary>
        /// <param name="easingFunction">The enum value representing the desired easing function.</param>
        /// <returns>A Func delegate that takes a float (time) and returns a float (eased value). Returns EaseLinear if the specified type is not found.</returns>
        public static Func<float, float> GetEasingFunction(EasingFunction easingFunction)
        {
            // Try to retrieve the function from the dictionary. If not found, default to Linear.
            return EasingFunctions.TryGetValue(easingFunction, out var easingFunc) ? easingFunc : EaseLinear;
        }

        /// <summary>
        /// Evaluates the specified easing function at time 't'.
        /// </summary>
        /// <param name="t">The input time/progress, typically clamped between 0.0 and 1.0 before calling this.</param>
        /// <param name="easingFunction">The enum value representing the desired easing function.</param>
        /// <returns>The calculated eased value corresponding to the input time 't'.</returns>
        public static float Evaluate(float t, EasingFunction easingFunction)
        {
            // Get the appropriate function delegate and invoke it with time 't'.
            var easingFunc = GetEasingFunction(easingFunction);
            return easingFunc(t);
        }

        // --- Easing Function Implementations ---

        /// <summary>Linear easing (no acceleration).</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value (0 to 1).</returns>
        public static float EaseLinear(float t) => Mathf.LerpUnclamped(0, 1, t); // Note: LerpUnclamped used for consistency, though Lerp would work for t in [0,1].

        /// <summary>Ease-in sine (starts slow, accelerates).</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value (0 to 1).</returns>
        public static float EaseInSine(float t) => 1 - Mathf.Cos((t * Pi) / 2);

        /// <summary>Ease-out sine (starts fast, decelerates).</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value (0 to 1).</returns>
        public static float EaseOutSine(float t) => Mathf.Sin((t * Pi) / 2);

        /// <summary>Ease-in-out sine (slow start and end, fast middle).</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value (0 to 1).</returns>
        public static float EaseInOutSine(float t) => -0.5f * (Mathf.Cos(Pi * t) - 1);

        /// <summary>Ease-in cubic (starts slow, accelerates faster than sine).</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value (0 to 1).</returns>
        public static float EaseInCubic(float t) => t * t * t;

        /// <summary>Ease-out cubic (starts fast, decelerates faster than sine).</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value (0 to 1).</returns>
        public static float EaseOutCubic(float t) => 1 - Mathf.Pow(1 - t, 3);

        /// <summary>Ease-in-out cubic (slow start and end, fast middle).</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value (0 to 1).</returns>
        public static float EaseInOutCubic(float t) => t < 0.5f ? 4 * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 3) / 2;

        /// <summary>Ease-in quintic (starts very slow, accelerates sharply).</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value (0 to 1).</returns>
        public static float EaseInQuint(float t) => t * t * t * t * t;

        /// <summary>Ease-out quintic (starts very fast, decelerates sharply).</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value (0 to 1).</returns>
        public static float EaseOutQuint(float t) => 1 - Mathf.Pow(1 - t, 5);

        /// <summary>Ease-in-out quintic (very slow start and end, sharp acceleration/deceleration).</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value (0 to 1).</returns>
        public static float EaseInOutQuint(float t) => t < 0.5f ? 16 * t * t * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 5) / 2;

        /// <summary>Ease-in circular (starts slow, accelerates based on a circular path).</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value (0 to 1).</returns>
        public static float EaseInCirc(float t) => 1 - Mathf.Sqrt(1 - t * t);

        /// <summary>Ease-out circular (starts fast, decelerates based on a circular path).</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value (0 to 1).</returns>
        public static float EaseOutCirc(float t) => Mathf.Sqrt(1 - (t - 1) * (t - 1));

        /// <summary>Ease-in-out circular (slow start and end, circular acceleration/deceleration).</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value (0 to 1).</returns>
        public static float EaseInOutCirc(float t) => t < 0.5f
            ? (1 - Mathf.Sqrt(1 - 4 * t * t)) / 2
            : (Mathf.Sqrt(1 - (-2 * t + 2) * (-2 * t + 2)) + 1) / 2;

        /// <summary>Ease-in elastic (starts with an elastic "snap" inward).</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value (can go outside 0-1 range).</returns>
        public static float EaseInElastic(float t) =>
            t == 0 || t == 1 ? t : -Mathf.Pow(2, 10 * t - 10) * Mathf.Sin((t * 10 - 10.75f) * TwoPi / 3);

        /// <summary>Ease-out elastic (ends with an elastic "snap" outward).</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value (can go outside 0-1 range).</returns>
        public static float EaseOutElastic(float t) =>
            t == 0 || t == 1 ? t : Mathf.Pow(2, -10 * t) * Mathf.Sin((t * 10 - 0.75f) * TwoPi / 3) + 1;

        /// <summary>Ease-in-out elastic (combines inward and outward elastic snaps).</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value (can go outside 0-1 range).</returns>
        public static float EaseInOutElastic(float t)
        {
            if (t == 0 || t == 1) return t;
            t *= 2; // Scale t to 0-2 range for the combined calculation
            return t < 1
                ? -0.5f * Mathf.Pow(2, 10 * t - 10) * Mathf.Sin((t * 10 - 10.75f) * TwoPi / 3) // In part
                : Mathf.Pow(2, -10 * t + 10) * Mathf.Sin((t * 10 - 10.75f) * TwoPi / 3) * 0.5f + 1; // Out part (adjusted from original for symmetry)
        }

        /// <summary>Ease-in quartic (t^4, similar to cubic but steeper).</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value (0 to 1).</returns>
        public static float EaseInQuart(float t) => t * t * t * t;

        /// <summary>Ease-out quartic (similar to cubic but steeper).</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value (0 to 1).</returns>
        public static float EaseOutQuart(float t) => 1 - Mathf.Pow(1 - t, 4);

        /// <summary>Ease-in-out quartic (similar to cubic but steeper).</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value (0 to 1).</returns>
        public static float EaseInOutQuart(float t) => t < 0.5f ? 8 * t * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 4) / 2;

        /// <summary>Ease-in exponential (starts very slow, accelerates dramatically).</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value (0 to 1).</returns>
        public static float EaseInExpo(float t) => t == 0 ? 0 : Mathf.Pow(2, 10 * t - 10);

        /// <summary>Ease-out exponential (starts very fast, decelerates dramatically).</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value (0 to 1).</returns>
        public static float EaseOutExpo(float t) => t == 1 ? 1 : 1 - Mathf.Pow(2, -10 * t);

        /// <summary>Ease-in-out exponential (dramatic acceleration and deceleration).</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value (0 to 1).</returns>
        public static float EaseInOutExpo(float t)
        {
            if (t == 0) return 0;
            if (t == 1) return 1;
            t *= 2; // Scale t to 0-2 range
            return t < 1 ? Mathf.Pow(2, 10 * t - 10) / 2 : (2 - Mathf.Pow(2, -10 * t + 10)) / 2;
        }

        /// <summary>Ease-in quadratic (t^2).</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value (0 to 1).</returns>
        public static float EaseInQuad(float t) => t * t;

        /// <summary>Ease-out quadratic.</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value (0 to 1).</returns>
        public static float EaseOutQuad(float t) => 1 - (1 - t) * (1 - t);

        /// <summary>Ease-in-out quadratic.</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value (0 to 1).</returns>
        public static float EaseInOutQuad(float t) => t < 0.5f ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2;

        /// <summary>Ease-in back (starts by moving slightly backward before moving forward).</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value (can go below 0).</returns>
        public static float EaseInBack(float t) => t * t * ((S + 1) * t - S);

        /// <summary>Ease-out back (overshoots the end slightly before settling).</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value (can go above 1).</returns>
        public static float EaseOutBack(float t)
        {
            const float overshoot = S; // Use defined constant
            t -= 1f;
            return 1f + t * t * ((overshoot + 1f) * t + overshoot);
        }

        /// <summary>Ease-in-out back (combines the backward start and overshoot end).</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value (can go outside 0-1).</returns>
        public static float EaseInOutBack(float t)
        {
            const float overshoot = S * 1.525f; // Adjusted overshoot for InOut
            t *= 2;
            if (t < 1)
            {
                return 0.5f * (t * t * ((overshoot + 1) * t - overshoot));
            }
            t -= 2;
            return 0.5f * (t * t * ((overshoot + 1) * t + overshoot) + 2);
        }

        /// <summary>Ease-in bounce (simulates dropping onto the start point).</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value (0 to 1).</returns>
        public static float EaseInBounce(float t) => 1 - EaseOutBounce(1 - t); // In-bounce is the reverse of out-bounce

        /// <summary>Ease-out bounce (simulates bouncing to a stop at the end point).</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value (0 to 1).</returns>
        public static float EaseOutBounce(float t)
        {
            // Piecewise function simulating bounces
            if (t < Divisor) return BounceFactor * t * t;
            if (t < 2 * Divisor) return BounceFactor * (t -= 1.5f * Divisor) * t + 0.75f;
            if (t < 2.5f * Divisor) return BounceFactor * (t -= 2.25f * Divisor) * t + 0.9375f;
            return BounceFactor * (t -= 2.625f * Divisor) * t + 0.984375f;
        }

        /// <summary>Ease-in-out bounce (combines in and out bounce effects).</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value (0 to 1).</returns>
        public static float EaseInOutBounce(float t) =>
            t < 0.5f ? EaseInBounce(t * 2) * 0.5f : EaseOutBounce(t * 2 - 1) * 0.5f + 0.5f;


        // --- Ping Pong Versions ---

        /// <summary>Helper function to transform time 't' (0 to 1) into a ping-pong loop (0 -> 1 -> 0).</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Ping-ponged time (0 to 1).</returns>
        private static float PingPong(float t)
        {
            //return Mathf.PingPong(t * 2, 1); // Alternative implementation
             // Map t from [0, 1] to [0, 2], then PingPong maps [0, 2] to [0, 1, 0]
            t *= 2;
            return (t <= 1) ? t : 2 - t;
        }

        /// <summary>Linear easing applied over a ping-pong time transformation.</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value following a 0->1->0 path.</returns>
        public static float EaseLinearPingPong(float t) => EaseLinear(PingPong(t));

        /// <summary>Ease-in sine applied over a ping-pong time transformation.</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value following a 0->1->0 path with sine-in easing.</returns>
        public static float EaseInSinePingPong(float t) => EaseInSine(PingPong(t));

        /// <summary>Ease-out sine applied over a ping-pong time transformation.</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value following a 0->1->0 path with sine-out easing.</returns>
        public static float EaseOutSinePingPong(float t) => EaseOutSine(PingPong(t));

        /// <summary>Ease-in-out sine applied over a ping-pong time transformation.</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value following a 0->1->0 path with sine-in-out easing.</returns>
        public static float EaseInOutSinePingPong(float t) => EaseInOutSine(PingPong(t));

        /// <summary>Ease-in cubic applied over a ping-pong time transformation.</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value following a 0->1->0 path with cubic-in easing.</returns>
        public static float EaseInCubicPingPong(float t) => EaseInCubic(PingPong(t));

        /// <summary>Ease-out cubic applied over a ping-pong time transformation.</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value following a 0->1->0 path with cubic-out easing.</returns>
        public static float EaseOutCubicPingPong(float t) => EaseOutCubic(PingPong(t));

        /// <summary>Ease-in-out cubic applied over a ping-pong time transformation.</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value following a 0->1->0 path with cubic-in-out easing.</returns>
        public static float EaseInOutCubicPingPong(float t) => EaseInOutCubic(PingPong(t));

        /// <summary>Ease-in quint applied over a ping-pong time transformation.</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value following a 0->1->0 path with quint-in easing.</returns>
        public static float EaseInQuintPingPong(float t) => EaseInQuint(PingPong(t));

        /// <summary>Ease-out quint applied over a ping-pong time transformation.</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value following a 0->1->0 path with quint-out easing.</returns>
        public static float EaseOutQuintPingPong(float t) => EaseOutQuint(PingPong(t));

        /// <summary>Ease-in-out quint applied over a ping-pong time transformation.</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value following a 0->1->0 path with quint-in-out easing.</returns>
        public static float EaseInOutQuintPingPong(float t) => EaseInOutQuint(PingPong(t));

        /// <summary>Ease-in circ applied over a ping-pong time transformation.</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value following a 0->1->0 path with circ-in easing.</returns>
        public static float EaseInCircPingPong(float t) => EaseInCirc(PingPong(t));

        /// <summary>Ease-out circ applied over a ping-pong time transformation.</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value following a 0->1->0 path with circ-out easing.</returns>
        public static float EaseOutCircPingPong(float t) => EaseOutCirc(PingPong(t));

        /// <summary>Ease-in-out circ applied over a ping-pong time transformation.</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value following a 0->1->0 path with circ-in-out easing.</returns>
        public static float EaseInOutCircPingPong(float t) => EaseInOutCirc(PingPong(t));

        /// <summary>Ease-in elastic applied over a ping-pong time transformation.</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value following a 0->1->0 path with elastic-in easing.</returns>
        public static float EaseInElasticPingPong(float t) => EaseInElastic(PingPong(t));

        /// <summary>Ease-out elastic applied over a ping-pong time transformation.</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value following a 0->1->0 path with elastic-out easing.</returns>
        public static float EaseOutElasticPingPong(float t) => EaseOutElastic(PingPong(t));

        /// <summary>Ease-in-out elastic applied over a ping-pong time transformation.</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value following a 0->1->0 path with elastic-in-out easing.</returns>
        public static float EaseInOutElasticPingPong(float t) => EaseInOutElastic(PingPong(t));

        /// <summary>Ease-in quart applied over a ping-pong time transformation.</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value following a 0->1->0 path with quart-in easing.</returns>
        public static float EaseInQuartPingPong(float t) => EaseInQuart(PingPong(t));

        /// <summary>Ease-out quart applied over a ping-pong time transformation.</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value following a 0->1->0 path with quart-out easing.</returns>
        public static float EaseOutQuartPingPong(float t) => EaseOutQuart(PingPong(t));

        /// <summary>Ease-in-out quart applied over a ping-pong time transformation.</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value following a 0->1->0 path with quart-in-out easing.</returns>
        public static float EaseInOutQuartPingPong(float t) => EaseInOutQuart(PingPong(t));

        /// <summary>Ease-in expo applied over a ping-pong time transformation.</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value following a 0->1->0 path with expo-in easing.</returns>
        public static float EaseInExpoPingPong(float t) => EaseInExpo(PingPong(t));

        /// <summary>Ease-out expo applied over a ping-pong time transformation.</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value following a 0->1->0 path with expo-out easing.</returns>
        public static float EaseOutExpoPingPong(float t) => EaseOutExpo(PingPong(t));

        /// <summary>Ease-in-out expo applied over a ping-pong time transformation.</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value following a 0->1->0 path with expo-in-out easing.</returns>
        public static float EaseInOutExpoPingPong(float t) => EaseInOutExpo(PingPong(t));

        /// <summary>Ease-in quad applied over a ping-pong time transformation.</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value following a 0->1->0 path with quad-in easing.</returns>
        public static float EaseInQuadPingPong(float t) => EaseInQuad(PingPong(t));

        /// <summary>Ease-out quad applied over a ping-pong time transformation.</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value following a 0->1->0 path with quad-out easing.</returns>
        public static float EaseOutQuadPingPong(float t) => EaseOutQuad(PingPong(t));

        /// <summary>Ease-in-out quad applied over a ping-pong time transformation.</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value following a 0->1->0 path with quad-in-out easing.</returns>
        public static float EaseInOutQuadPingPong(float t) => EaseInOutQuad(PingPong(t));

        /// <summary>Ease-in back applied over a ping-pong time transformation.</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value following a 0->1->0 path with back-in easing.</returns>
        public static float EaseInBackPingPong(float t) => EaseInBack(PingPong(t));

        /// <summary>Ease-out back applied over a ping-pong time transformation.</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value following a 0->1->0 path with back-out easing.</returns>
        public static float EaseOutBackPingPong(float t) => EaseOutBack(PingPong(t));

        /// <summary>Ease-in-out back applied over a ping-pong time transformation.</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value following a 0->1->0 path with back-in-out easing.</returns>
        public static float EaseInOutBackPingPong(float t) => EaseInOutBack(PingPong(t));

        /// <summary>Ease-in bounce applied over a ping-pong time transformation.</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value following a 0->1->0 path with bounce-in easing.</returns>
        public static float EaseInBouncePingPong(float t) => EaseInBounce(PingPong(t));

        /// <summary>Ease-out bounce applied over a ping-pong time transformation.</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value following a 0->1->0 path with bounce-out easing.</returns>
        public static float EaseOutBouncePingPong(float t) => EaseOutBounce(PingPong(t));

        /// <summary>Ease-in-out bounce applied over a ping-pong time transformation.</summary>
        /// <param name="t">Input time (0 to 1).</param> <returns>Eased value following a 0->1->0 path with bounce-in-out easing.</returns>
        public static float EaseInOutBouncePingPong(float t) => EaseInOutBounce(PingPong(t));
    }
}