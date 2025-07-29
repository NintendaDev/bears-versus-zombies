
using UnityEngine;

namespace OM
{
    /// <summary>
    /// Defines the type of easing mechanism to be used by an EaseData instance.
    /// Specifies whether to use a predefined mathematical EasingFunction or a
    /// user-defined AnimationCurve.
    /// </summary>
    public enum EaseDataType
    {
        /// <summary>
        /// Use a standard mathematical easing function defined in the EasingFunction enum.
        /// Evaluation will be handled by EaseLibrary.Evaluate().
        /// </summary>
        Ease = 0,

        /// <summary>
        /// Use a custom AnimationCurve defined by the user.
        /// Evaluation will be handled by AnimationCurve.Evaluate().
        /// </summary>
        AnimationCurve = 1,
    }

    /// <summary>
    /// Encapsulates easing data, allowing the selection between standard mathematical
    /// easing functions (EasingFunction) or a custom AnimationCurve.
    /// Provides a unified Evaluate method to get the eased value based on the selected type.
    /// Designed to be serialized and used in the Unity Inspector via the EasingDataPropertyDrawer.
    /// </summary>
    [System.Serializable] // Allows this class to be serialized by Unity and shown in the Inspector.
    public class EaseData
    {
        /// <summary>
        /// Determines whether this instance uses an EasingFunction (Ease) or an AnimationCurve.
        /// This field is controlled by the custom property drawer based on user selection.
        /// </summary>
        public EaseDataType easeDataType;

        /// <summary>
        /// The AnimationCurve used when easeDataType is set to AnimationCurve.
        /// Defaults to a basic curve (e.g., peaking at 0.5).
        /// This curve is edited directly in the Inspector when selected.
        /// </summary>
        public AnimationCurve animationCurve = new AnimationCurve(
            new Keyframe(0, 0, 0, 0),       // Start at (0,0) with flat tangent
            new Keyframe(.5f, 1, 0, 0),     // Peak at (0.5, 1) with flat tangent
            new Keyframe(1, 0, 0, 0));      // End at (1,0) with flat tangent

        /// <summary>
        /// The standard easing function used when easeDataType is set to Ease.
        /// Defaults to EasingFunction.Linear.
        /// This enum value is selected via the dropdown provided by the custom property drawer.
        /// </summary>
        public EasingFunction easeType = EasingFunction.Linear;

        /// <summary>
        /// Default constructor. Initializes with default values, typically setting
        /// the type to AnimationCurve with a default curve shape and Linear ease as fallback.
        /// </summary>
        public EaseData()
        {
            // Sensible defaults: often start with a curve, maybe linear or a simple bounce/peak.
            easeDataType = EaseDataType.AnimationCurve;
            animationCurve = new AnimationCurve(
                new Keyframe(0, 0, 0, 0),
                new Keyframe(.5f, 1, 0, 0),
                new Keyframe(1, 0, 0, 0));
            easeType = EasingFunction.Linear; // Default ease type if switched.
        }

        /// <summary>
        /// Constructor to initialize directly with an AnimationCurve.
        /// Sets the easeDataType to AnimationCurve.
        /// </summary>
        /// <param name="animationCurve">The AnimationCurve to use.</param>
        public EaseData(AnimationCurve animationCurve)
        {
            easeDataType = EaseDataType.AnimationCurve;
            this.animationCurve = animationCurve;
            // Keep a default easeType in case the user switches later in the editor.
            this.easeType = EasingFunction.Linear;
        }

        /// <summary>
        /// Constructor to initialize directly with an EasingFunction.
        /// Sets the easeDataType to Ease.
        /// </summary>
        /// <param name="easeType">The EasingFunction to use.</param>
        public EaseData(EasingFunction easeType)
        {
            easeDataType = EaseDataType.Ease;
            this.easeType = easeType;
            // Keep a default animationCurve in case the user switches later in the editor.
            this.animationCurve = new AnimationCurve(
                new Keyframe(0, 0, 0, 0),
                new Keyframe(.5f, 1, 0, 0),
                new Keyframe(1, 0, 0, 0));
        }

        /// <summary>
        /// Evaluates the easing effect at a given time `t`.
        /// Selects the appropriate evaluation method (AnimationCurve.Evaluate or EaseLibrary.Evaluate)
        /// based on the current `easeDataType`.
        /// Clamps the input time `t` between 0 and 1 before evaluation.
        /// </summary>
        /// <param name="t">The input time/progress, typically ranging from 0.0 to 1.0.</param>
        /// <returns>The evaluated easing value (output value) at time `t`.</returns>
        public float Evaluate(float t)
        {
            // Clamp t to the standard 0-1 range expected by easing functions and curves.
            float clampedT = Mathf.Clamp01(t);

            // Choose evaluation method based on the selected type.
            return easeDataType == EaseDataType.Ease
                ? EaseLibrary.Evaluate(clampedT, easeType)             // Use the standard easing function library.
                : animationCurve.Evaluate(clampedT);                    // Use the assigned AnimationCurve.
        }

        /// <summary>
        /// Provides a default EaseData instance, configured to use an AnimationCurve
        /// with a simple peak shape and Linear easing as the fallback EasingFunction type.
        /// Useful for creating new instances with sensible defaults.
        /// </summary>
        /// <returns>A new EaseData instance with default settings.</returns>
        public static EaseData GetDefault()
        {
            return new EaseData // Uses the default constructor's logic.
            {
                // Explicitly setting defaults here for clarity, matching the constructor.
                easeDataType = EaseDataType.AnimationCurve,
                animationCurve = AnimationCurve.EaseInOut(0,0,1,1),
                easeType = EasingFunction.Linear,
            };
        }
    }
}