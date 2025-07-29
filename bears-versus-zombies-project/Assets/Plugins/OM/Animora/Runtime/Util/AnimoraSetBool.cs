// --- File: AnimoraSetBool.cs ---

// Namespace for the Animora runtime components, specific to the animation system
namespace OM.Animora.Runtime
{
    /// <summary>
    /// Represents a tri-state boolean setting, often used in inspectors
    /// where a feature can be explicitly enabled (True), explicitly disabled (False),
    /// or inherit/use a default value (Disabled).
    /// </summary>
    public enum AnimoraSetBool
    {
        /// <summary>
        /// The setting is disabled or inactive. When evaluated, this typically
        /// results in using a default boolean value provided by the context.
        /// </summary>
        Disabled,

        /// <summary>
        /// The setting is explicitly enabled. When evaluated, this results in `true`.
        /// </summary>
        True,

        /// <summary>
        /// The setting is explicitly disabled. When evaluated, this results in `false`.
        /// </summary>
        False
    }
}

// --- File: AnimoraSetBoolExtension.cs ---

// Namespace for the Animora runtime components, specific to the animation system
namespace OM.Animora.Runtime
{
    /// <summary>
    /// Provides extension methods for the <see cref="AnimoraSetBool"/> enum
    /// to easily convert its state into a standard boolean value.
    /// </summary>
    public static class AnimoraSetBoolExtension
    {
        /// <summary>
        /// Gets the boolean value represented by the <see cref="AnimoraSetBool"/> state.
        /// If the state is <see cref="AnimoraSetBool.Disabled"/>, it returns the provided <paramref name="defaultValue"/>.
        /// </summary>
        /// <param name="setBool">The <see cref="AnimoraSetBool"/> instance being evaluated.</param>
        /// <param name="defaultValue">The boolean value to return if <paramref name="setBool"/> is <see cref="AnimoraSetBool.Disabled"/>.</param>
        /// <returns>True if <paramref name="setBool"/> is <see cref="AnimoraSetBool.True"/>,
        /// false if <paramref name="setBool"/> is <see cref="AnimoraSetBool.False"/>,
        /// otherwise <paramref name="defaultValue"/>.</returns>
        public static bool GetValue(this AnimoraSetBool setBool, bool defaultValue)
        {
            // Use a switch expression for concise mapping
            return setBool switch
            {
                // If Disabled, return the provided default value.
                AnimoraSetBool.Disabled => defaultValue,
                // If True, return true.
                AnimoraSetBool.True => true,
                // If False, return false.
                AnimoraSetBool.False => false,
                // Default case (shouldn't be reached with enums, but good practice) returns default.
                _ => defaultValue
            };
        }

        /// <summary>
        /// Gets the boolean value represented by the <see cref="AnimoraSetBool"/> state,
        /// assuming <see cref="AnimoraSetBool.Disabled"/> evaluates to `false`.
        /// </summary>
        /// <param name="setBool">The <see cref="AnimoraSetBool"/> instance being evaluated.</param>
        /// <returns>True if <paramref name="setBool"/> is <see cref="AnimoraSetBool.True"/>, otherwise false.</returns>
        public static bool GetValue(this AnimoraSetBool setBool)
        {
            // Use a switch expression, mapping Disabled and False to false.
            return setBool switch
            {
                // If True, return true.
                AnimoraSetBool.True => true,
                // All other cases (Disabled, False) return false.
                _ => false
                 // Explicit cases:
                 // AnimoraSetBool.Disabled => false,
                 // AnimoraSetBool.False => false,
            };
        }

        /// <summary>
        /// Attempts to get the boolean value represented by the <see cref="AnimoraSetBool"/> state,
        /// indicating whether the state was explicitly set (True or False).
        /// </summary>
        /// <param name="setBool">The <see cref="AnimoraSetBool"/> instance being evaluated.</param>
        /// <param name="value">Outputs the boolean value (true for <see cref="AnimoraSetBool.True"/>, false otherwise) if the state was explicitly set.</param>
        /// <returns>True if the state was explicitly <see cref="AnimoraSetBool.True"/> or <see cref="AnimoraSetBool.False"/>; false if the state was <see cref="AnimoraSetBool.Disabled"/>.</returns>
        public static bool TryGetValue(this AnimoraSetBool setBool, out bool value)
        {
            // Get the boolean value, assuming Disabled maps to false.
            value = setBool.GetValue(); // Uses the GetValue() overload where Disabled is false
            // Return true only if the state was *not* Disabled.
            return setBool != AnimoraSetBool.Disabled;
        }
    }
}