
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OM.Animora.Runtime
{
    public enum AnimoraValueType
    {
        Fixed = 0,
        Random = 1,
        Target = 2,
    }

    public enum AnimoraValueVector3Type
    {
        Position,
        LocalPosition,
        Scale,
        EulerAngles,
        LocalEulerAngles,
        AnchorPosition,
    }
    
    /// <summary>
    /// A generic wrapper class for a struct value <typeparamref name="T"/>.
    /// It allows the value to be either fixed or randomized between two boundary values (`randomValue1`, `randomValue2`).
    /// Includes logic to cache the calculated (potentially randomized) value and helper methods for randomization.
    /// </summary>
    /// <typeparam name="T">The struct type of the value being stored (e.g., float, Vector3, Color).</typeparam>
    [System.Serializable] // Allows this class to be serialized by Unity when used as a field.
    public class AnimoraValue<T> where T : struct // Constraint: T must be a value type (struct).
    {
        /// <summary>
        /// The primary, non-randomized value. This is used directly if <see cref="random"/> is false,
        /// or potentially as a base/default if randomization is enabled but fails or isn't applicable.
        /// </summary>
        [SerializeField] private AnimoraValueType valueType = AnimoraValueType.Fixed;
        [SerializeField] private AnimoraValueVector3Type vector3Type = AnimoraValueVector3Type.AnchorPosition;
        [SerializeField] private T value;

        /// <summary>
        /// Flag indicating whether the value should be randomized between <see cref="randomValue1"/> and <see cref="randomValue2"/>
        /// when <see cref="GetValue"/> or <see cref="Randomize"/> is called.
        /// </summary>
        //[SerializeField] private bool random = false;

        /// <summary>
        /// The first boundary value used for randomization if <see cref="random"/> is true.
        /// Typically represents the minimum value of the random range.
        /// </summary>
        [SerializeField] private T randomValue1;

        /// <summary>
        /// The second boundary value used for randomization if <see cref="random"/> is true.
        /// Typically represents the maximum value of the random range.
        /// </summary>
        [SerializeField] private T randomValue2;
        [SerializeField] private Transform targetTransform;

        /// <summary>
        /// Internal flag to track whether the <see cref="_currentValue"/> has been calculated
        /// (either set directly or randomized) since the last time <see cref="Randomize"/> was explicitly or implicitly called.
        /// Used by <see cref="GetValue"/> to avoid redundant randomization.
        /// </summary>
        private bool _calculated = false;

        /// <summary>
        /// Internal cache storing the most recently calculated value (either the fixed <see cref="value"/> or a randomized value).
        /// This value is returned by <see cref="GetValue"/> unless <paramref name="forceRandomize"/> is true.
        /// </summary>
        private T _currentValue;

        /// <summary>
        /// Public property to get or set the first randomization boundary value (<see cref="randomValue1"/>).
        /// </summary>
        public T RandomValue1 { get => randomValue1; set => randomValue1 = value; }
        /// <summary>
        /// Public property to get or set the second randomization boundary value (<see cref="randomValue2"/>).
        /// </summary>
        public T RandomValue2 { get => randomValue2; set => randomValue2 = value; }
        /// <summary>
        /// Public property to get or set the primary, non-randomized value (<see cref="value"/>).
        /// Setting this property does *not* automatically update the cached <see cref="_currentValue"/>;
        /// call <see cref="Randomize"/> or <see cref="GetValue"/> to refresh the cache if needed.
        /// </summary>
        public T Value { get => value; set => this.value = value; } // Note: Setter only changes 'value', not '_currentValue'

        /// <summary>
        /// Constructor for creating an AnimoraValue configured for randomization between two boundary values.
        /// </summary>
        /// <param name="value1">The first boundary value for randomization (e.g., min).</param>
        /// <param name="value2">The second boundary value for randomization (e.g., max).</param>
        public AnimoraValue(T value1, T value2)
        {
            // Note: This constructor doesn't use the primary 'value' field. It might be clearer
            // to set randomValue1/2 directly and maybe default 'value'.
            this.value = value1; // Sets the base value (might be confusing)
            this.randomValue1 = value1; // Set randomization boundary 1
            this.randomValue2 = value2; // Set randomization boundary 2
            //this.random = true; // Enable randomization by default for this constructor
            this.valueType = AnimoraValueType.Random; // Set the value type to Random
        }

        /// <summary>
        /// Constructor for creating an AnimoraValue with a fixed, non-randomized value.
        /// </summary>
        /// <param name="value">The fixed value.</param>
        public AnimoraValue(T value)
        {
            this.value = value; // Set the fixed value
            //this.random = false; // Ensure randomization is disabled
            this.valueType = AnimoraValueType.Fixed; // Set the value type to Fixed
            // Initialize random boundaries to the fixed value for consistency (optional)
            this.randomValue1 = value;
            this.randomValue2 = value;
        }

        /// <summary>
        /// Constructor allowing explicit setting of base value, randomization boundaries, and the randomization flag.
        /// </summary>
        /// <param name="value">The base value (used if random is false).</param>
        /// <param name="value1">The first boundary for randomization.</param>
        /// <param name="value2">The second boundary for randomization.</param>
        /// <param name="randomize">Whether to enable randomization.</param>
        public AnimoraValue(T value, T value1, T value2, bool randomize = false)
        {
            this.value = value; // Set the base value
            this.randomValue1 = value1; // Set randomization boundary 1
            this.randomValue2 = value2; // Set randomization boundary 2
            //this.random = randomize; // Set randomization flag based on parameter
            this.valueType = randomize ? AnimoraValueType.Random : AnimoraValueType.Fixed; // Set value type based on randomization
        }

        /// <summary>
        /// Gets the effective value, performing randomization if necessary or forced.
        /// Uses the cached value (<see cref="_currentValue"/>) if available and randomization is not forced.
        /// </summary>
        /// <param name="forceRandomize">If true, forces a recalculation (randomization if enabled) even if a value has already been calculated. If false, returns the cached value if available.</param>
        /// <returns>The effective value (potentially randomized).</returns>
        public T GetValue(bool forceRandomize = false) // Default forceRandomize to false (use cache) is often desired
        {
            // If randomization is forced OR if the value hasn't been calculated yet this cycle,
            // call Randomize() to update _currentValue.
            if (forceRandomize || !_calculated)
            {
                 Randomize();
            }
            // Return the cached current value.
            return _currentValue;
        }

        /// <summary>
        /// Calculates and caches the effective value (<see cref="_currentValue"/>).
        /// If <see cref="random"/> is true, it calls the static <see cref="GetRandomValue{TType}"/> helper
        /// to generate a randomized value between <see cref="randomValue1"/> and <see cref="randomValue2"/>.
        /// If <see cref="random"/> is false, it uses the fixed <see cref="value"/>.
        /// Sets the <see cref="_calculated"/> flag to true.
        /// </summary>
        public void Randomize()
        {
            // Determine the value based on the 'random' flag.
            switch (valueType)
            {
                case AnimoraValueType.Fixed:
                    _currentValue = value;
                    break;
                case AnimoraValueType.Random:
                    _currentValue = GetRandomValue<T>(randomValue1, randomValue2);
                    break;
                case AnimoraValueType.Target:
                    if (targetTransform != null)
                    {
                        switch (vector3Type)
                        {
                            case AnimoraValueVector3Type.Position:
                                _currentValue = (T)(object)targetTransform.position;
                                break;
                            case AnimoraValueVector3Type.LocalPosition:
                                _currentValue = (T)(object)targetTransform.localPosition;
                                break;
                            case AnimoraValueVector3Type.Scale:
                                _currentValue = (T)(object)targetTransform.localScale;
                                break;
                            case AnimoraValueVector3Type.EulerAngles:
                                _currentValue = (T)(object)targetTransform.eulerAngles;
                                break;
                            case AnimoraValueVector3Type.LocalEulerAngles:
                                _currentValue = (T)(object)targetTransform.localEulerAngles;
                                break;
                            case AnimoraValueVector3Type.AnchorPosition:
                                _currentValue = (T)(object)targetTransform.GetComponent<RectTransform>().anchoredPosition3D;
                                break;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Target transform is null. Cannot get value.");
                        _currentValue = value; // Fallback to fixed value
                    }
                    break;
            }
            //_currentValue = random
            //    ? GetRandomValue<T>(randomValue1, randomValue2) // Get a new random value
            //    : value; // Use the fixed base value
            // Mark the value as calculated/updated.
            _calculated = true;
        }

        /// <summary>
        /// Static helper method to generate a random value between two boundary values (`zero`, `one`)
        /// for supported struct types (float, Vector2, Vector3, Color).
        /// </summary>
        /// <typeparam name="TType">The struct type for which to generate a random value.</typeparam>
        /// <param name="zero">The first boundary value (e.g., min).</param>
        /// <param name="one">The second boundary value (e.g., max).</param>
        /// <returns>A randomized value of type <typeparamref name="TType"/>.</returns>
        /// <exception cref="System.NotSupportedException">Thrown if <typeparamref name="TType"/> is not one of the supported types (float, Vector2, Vector3, Color).</exception>
        public static TType GetRandomValue<TType>(TType zero, TType one) where TType : struct
        {
            // --- Handle float type ---
            if (typeof(TType) == typeof(float))
            {
                // Cast boundaries to float (boxing/unboxing involved).
                var result = Random.Range((float)(object)zero, (float)(object)one);
                // Cast result back to TType (unboxing).
                return (TType)(object)result;
            }
            // --- Handle Vector2 type ---
            if (typeof(TType) == typeof(Vector2))
            {
                // Cast boundaries to Vector2.
                var zeroValue = (Vector2)(object)zero;
                var oneValue = (Vector2)(object)one;
                // Generate random values for each component (x, y).
                return (TType)(object)new Vector2(
                    Random.Range(zeroValue.x, oneValue.x),
                    Random.Range(zeroValue.y, oneValue.y)
                );
            }
            // --- Handle Vector3 type ---
            if (typeof(TType) == typeof(Vector3))
            {
                // Cast boundaries to Vector3.
                var zeroValue = (Vector3)(object)zero;
                var oneValue = (Vector3)(object)one;
                // Generate random values for each component (x, y, z).
                return (TType)(object)new Vector3(
                    Random.Range(zeroValue.x, oneValue.x),
                    Random.Range(zeroValue.y, oneValue.y),
                    Random.Range(zeroValue.z, oneValue.z)
                );
            }
            // --- Handle Color type ---
            if (typeof(TType) == typeof(Color))
            {
                // Cast boundaries to Color.
                var zeroValue = (Color)(object)zero;
                var oneValue = (Color)(object)one;
                // Generate random values for each component (r, g, b, a).
                return (TType)(object)new Color(
                    Random.Range(zeroValue.r, oneValue.r),
                    Random.Range(zeroValue.g, oneValue.g),
                    Random.Range(zeroValue.b, oneValue.b),
                    Random.Range(zeroValue.a, oneValue.a)
                );
            }

            // If the type TType is not supported, throw an exception.
            throw new System.NotSupportedException($"Type {typeof(TType)} is not supported by GetRandomValue.");
        }
        
        public Type GetValueType() => typeof(T);
    }
}