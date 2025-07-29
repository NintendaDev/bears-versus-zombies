using UnityEngine;

namespace OM.Animora.Runtime
{
    /// <summary>
    /// A simple serializable class that groups an <see cref="AnimoraValue{T}"/> instance
    /// with an explicit boolean `enabled` flag.
    /// This pattern is often used in inspectors to allow users to enable or disable
    /// optional features or overrides represented by the `AnimoraValue`.
    /// </summary>
    /// <typeparam name="T">The struct type of the value managed by the internal <see cref="AnimoraValue{T}"/>.</typeparam>
    [System.Serializable] // Allows this class to be serialized by Unity when used as a field.
    public class AnimoraValueGroup<T> where T : struct // Constraint: T must be a value type (struct).
    {
        /// <summary>
        /// The core <see cref="AnimoraValue{T}"/> instance being grouped.
        /// Stores the actual value (fixed or randomized).
        /// </summary>
        [SerializeField] private AnimoraValue<T> value;

        /// <summary>
        /// Flag indicating whether this group (and therefore the associated `value`) is enabled.
        /// Logic using this group should typically check this flag before accessing or using the `value`.
        /// </summary>
        [SerializeField] private bool enabled = false; // Default to disabled

        /// <summary>
        /// Public property providing read-only access to the internal <see cref="AnimoraValue{T}"/> instance.
        /// </summary>
        public AnimoraValue<T> Value => value;

        /// <summary>
        /// Public property providing read-only access to the enabled state of this group.
        /// </summary>
        public bool Enabled => enabled;

        /// <summary>
        /// Public method to explicitly set the enabled state of this group.
        /// Primarily useful if modifying the state programmatically.
        /// </summary>
        /// <param name="isEnabled">The desired enabled state.</param>
        public void SetEnabled(bool isEnabled)
        {
            this.enabled = isEnabled;
        }

        /// <summary>
        /// Constructor to initialize the group with a pre-existing <see cref="AnimoraValue{T}"/> instance.
        /// The group will be disabled by default.
        /// </summary>
        /// <param name="value">The <see cref="AnimoraValue{T}"/> instance to wrap.</param>
        public AnimoraValueGroup(AnimoraValue<T> value)
        {
            this.value = value;
            this.enabled = false; // Default to disabled unless specified otherwise
        }

        /// <summary>
        /// Constructor to initialize the group with a pre-existing <see cref="AnimoraValue{T}"/> instance
        /// and an explicit enabled state.
        /// </summary>
        /// <param name="value">The <see cref="AnimoraValue{T}"/> instance to wrap.</param>
        /// <param name="isEnabled">The initial enabled state for the group.</param>
        public AnimoraValueGroup(AnimoraValue<T> value, bool isEnabled)
        {
            this.value = value;
            this.enabled = isEnabled;
        }

        /// <summary>
        /// Convenience method to get the effective value from the internal <see cref="AnimoraValue{T}"/> instance.
        /// This directly calls `value.GetValue()`.
        /// Note: This method does NOT check the `Enabled` flag of the group itself. The caller
        /// is responsible for checking `AnimoraValueGroup.Enabled` before calling `GetValue`.
        /// </summary>
        /// <param name="forceRandomize">If true, forces the internal <see cref="AnimoraValue{T}"/> to potentially generate a new random value.</param>
        /// <returns>The effective value from the internal <see cref="AnimoraValue{T}"/>.</returns>
        public T GetValue(bool forceRandomize = false) // Default forceRandomize to false is common
        {
            // Return default if 'value' is somehow null (shouldn't happen with struct constraint if initialized)
            return value != null ? value.GetValue(forceRandomize) : default(T);
        }
    }
}