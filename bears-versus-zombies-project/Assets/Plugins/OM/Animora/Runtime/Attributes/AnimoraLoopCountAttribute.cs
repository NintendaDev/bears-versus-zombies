using System;
using UnityEngine;

namespace OM.Animora.Runtime
{
    /// <summary>
    /// A custom Unity PropertyAttribute used to decorate integer fields representing loop counts.
    /// It allows specifying a minimum value and an associated message (e.g., to indicate
    /// that a specific value like 0 or -1 means "endless loop").
    /// This attribute requires a custom PropertyDrawer (<see cref="AnimoraLoopCountDrawer"/>, not shown here)
    /// to modify the inspector GUI based on these properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)] // This attribute can only be applied to fields.
    public class AnimoraLoopCountAttribute : PropertyAttribute
    {
        /// <summary>
        /// Gets the minimum allowed value for the loop count field.
        /// </summary>
        public int Min { get; }

        /// <summary>
        /// Gets the optional message associated with the minimum value.
        /// This can be used by a custom PropertyDrawer to display helpful text,
        /// such as "0 = endless loop" or "-1 = infinite".
        /// </summary>
        public string MinMessage { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimoraLoopCountAttribute"/> class.
        /// </summary>
        /// <param name="min">The minimum allowed integer value for the loop count field.</param>
        /// <param name="minMessage">An optional descriptive message associated with the minimum value (e.g., "0 = endless"). Defaults to null.</param>
        public AnimoraLoopCountAttribute(int min, string minMessage = null)
        {
            Min = min;
            MinMessage = minMessage;
        }
    }
}