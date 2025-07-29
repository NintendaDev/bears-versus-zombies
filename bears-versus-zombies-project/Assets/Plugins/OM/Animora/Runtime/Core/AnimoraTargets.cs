using System.Collections.Generic;
using System.Linq;
using OM.TimelineCreator.Runtime;
using UnityEngine;

namespace OM.Animora.Runtime
{
    /// <summary>
    /// Abstract base class for defining how targets are specified for an Animora clip or action.
    /// Derived classes implement specific targeting mechanisms (e.g., direct references, ID lookups).
    /// Provides a common interface for error checking.
    /// </summary>
    [System.Serializable] // Allows derived classes to be serialized by Unity.
    public abstract class AnimoraTargetBase
    {
        /// <summary>
        /// Checks if the current target configuration is valid and reports any errors.
        /// Derived classes must implement this to validate their specific targeting method.
        /// </summary>
        /// <param name="error">An output parameter containing a description of the error if validation fails.</param>
        /// <returns>True if the configuration has errors, false otherwise.</returns>
        public virtual bool HasErrors(out string error)
        {
            // Default implementation assumes no errors.
            error = string.Empty;
            return false;
        }

        // Possible future extension:
        // public abstract IEnumerable<T> GetTargets<T>() where T : Component;
        // Would require derived classes to implement how to retrieve the actual target components.
    }

    /// <summary>
    /// Defines the different methods available for specifying targets.
    /// Currently includes Direct referencing and potentially ID-based lookup.
    /// </summary>
    // Note: This enum doesn't seem to be directly used by AnimoraTargets<T> in the provided code,
    // but it might be intended for future use or other targeting systems derived from AnimoraTargetBase.
    public enum AnimoraTargetType
    {
        /// <summary>
        /// Targets are specified using an ID or tag, requiring a lookup mechanism at runtime. (Conceptual)
        /// </summary>
        Id,
        /// <summary>
        /// Targets are specified by directly assigning Component references in the Inspector.
        /// </summary>
        Direct,
    }

    /// <summary>
    /// Implements the 'Direct' targeting method using a serialized list of component references.
    /// Provides methods to get, set, add, and validate these direct references.
    /// Handles drag-and-drop functionality in the editor to populate the target list.
    /// </summary>
    /// <typeparam name="T">The specific type of <see cref="Component"/> being targeted (e.g., Transform, Light).</typeparam>
    [System.Serializable] // Allows this class to be serialized when used as a field.
    public class AnimoraTargets<T> : AnimoraTargetBase where T : Component // Derives from base and requires T to be a Component.
    {
        /// <summary>
        /// The serialized list storing direct references to the target components.
        /// Configured via the Unity Inspector.
        /// </summary>
        [SerializeField]
        [Tooltip("Direct references to the components this clip/action will affect.")]
        private List<T> directTargets = new List<T>(); // Initialize with empty list

        /// <summary>
        /// Gets the list of directly referenced target components.
        /// </summary>
        /// <returns>The list of target components of type <typeparamref name="T"/>.</returns>
        public List<T> GetTargets()
        {
            // Ensure the list is never null, return empty list if it somehow becomes null.
            directTargets ??= new List<T>();
            return directTargets;
        }

        /// <summary>
        /// Gets the first target component in the list.
        /// Useful when an action or clip only needs to operate on a single primary target.
        /// </summary>
        /// <returns>The first target component, or null if the list is empty.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if the list is empty.</exception>
        public T GetFirstTarget()
        {
            // Ensure list exists and has elements before accessing.
            if (directTargets == null || directTargets.Count == 0)
            {
                // Consider returning null or throwing a more specific exception if appropriate.
                // throw new System.InvalidOperationException("Target list is empty.");
                 return null;
            }
            return directTargets[0];
        }

        /// <summary>
        /// Validates the list of direct targets. Checks if the list is null, empty, or contains any null references.
        /// Overrides the base class method.
        /// </summary>
        /// <param name="error">Output parameter containing the error message if validation fails.</param>
        /// <returns>True if errors are found, false otherwise.</returns>
        public override bool HasErrors(out string error)
        {
            error = string.Empty;
            // Check for null list, empty list, or any null elements within the list.
            if (directTargets == null || directTargets.Count == 0 || directTargets.Any(x => x == null))
            {
                error = "Direct targets are not set or contain null entries.";
                return true; // Errors found
            }
            return false; // No errors
        }

        /// <summary>
        /// Gets the target component at a specific index in the list.
        /// Includes bounds checking and error logging for invalid indices.
        /// </summary>
        /// <param name="i">The zero-based index of the target to retrieve.</param>
        /// <returns>The target component at the specified index, or null if the index is invalid or the list is null.</returns>
        public T GetTargetAt(int i)
        {
             directTargets ??= new List<T>(); // Ensure list exists
            // Check if the index is within the valid range.
            if (i >= 0 && i < directTargets.Count)
            {
                return directTargets[i]; // Return the element at the index.
            }
            // Log an error if the index is out of range.
            Debug.LogError($"Index {i} is out of range for targets list (Count: {directTargets.Count}). Cannot get target.", (directTargets.Count > 0 ? directTargets[0] as Object : null)); // Log error with context object if possible
            return null; // Return null for invalid index.
        }

        /// <summary>
        /// Adds a target component to the end of the list.
        /// </summary>
        /// <param name="target">The target component of type <typeparamref name="T"/> to add.</param>
        public void AddTarget(T target)
        {
            directTargets ??= new List<T>(); // Ensure list exists
            if (target != null) // Optionally prevent adding null targets explicitly
            {
                 directTargets.Add(target);
            }
        }

        /// <summary>
        /// Sets the entire list of targets, replacing the current list.
        /// Filters out any null entries from the input list.
        /// </summary>
        /// <param name="targets">The new list of targets.</param>
        public void SetTargets(List<T> targets)
        {
            // If the input list is null, do nothing (or clear the list?). Current: do nothing.
            if (targets == null) return;

            // Ensure the internal list exists.
            directTargets ??= new List<T>();
            // Clear the current list before adding new targets.
            directTargets.Clear();

            // Add only non-null targets from the input list.
            foreach (var target in targets)
            {
                if (target != null)
                {
                    directTargets.Add(target);
                }
            }
        }

        /// <summary>
        /// Handles drag-and-drop operations onto the associated clip/action in the editor.
        /// Attempts to extract compatible components (<typeparamref name="T"/>) from the dropped objects
        /// (which can be Components, GameObjects, or potentially other asset types) and adds them to the target list.
        /// </summary>
        /// <param name="objectReferences">An array of objects dropped onto the UI element.</param>
        /// <param name="player">The timeline player instance (used for context, primarily for `RecordUndo`).</param>
        /// <returns>True if at least one compatible target was successfully added, false otherwise.</returns>
        public bool OnDrop(Object[] objectReferences, IOM_TimelinePlayer<AnimoraClip> player)
        {
             if (objectReferences == null || objectReferences.Length == 0) return false;

            // Record Undo step before modifying the target list.
            player?.RecordUndo("Drop Targets"); // Use player context to record Undo

            // Ensure the internal list exists.
            directTargets ??= new List<T>();

            bool addedTarget = false; // Flag to track if any targets were added

            // Iterate through each object dropped.
            foreach (var o in objectReferences)
            {
                 if (o == null) continue; // Skip null objects

                // Case 1: Dropped object is already the correct component type.
                if (o is T targetComponentT)
                {
                    // Avoid adding duplicates.
                    if (!directTargets.Contains(targetComponentT))
                    {
                        directTargets.Add(targetComponentT);
                        addedTarget = true;
                    }
                    continue; // Move to the next dropped object
                }

                // Case 2: Dropped object is a GameObject. Try to get the component from it.
                if (o is GameObject gameObject)
                {
                    // Attempt to get the required component type T from the GameObject.
                    var component = gameObject.GetComponent<T>();
                    if (component != null)
                    {
                         // Avoid adding duplicates.
                        if (!directTargets.Contains(component))
                        {
                            directTargets.Add(component);
                            addedTarget = true;
                        }
                    }
                     // Optional: Could also check components in children using GetComponentInChildren<T>()
                    continue; // Move to the next dropped object
                }

                // Case 3: Dropped object is some other Component. Try GetComponent<T> on its GameObject.
                if (o is Component componentSource)
                {
                     // Check if the dropped component *is* the target type first (redundant with case 1, but safe).
                     if (componentSource is T targetComponentDirect) {
                          if (!directTargets.Contains(targetComponentDirect))
                          {
                               directTargets.Add(targetComponentDirect);
                               addedTarget = true;
                          }
                     }
                     else // If not, try getting the target type from the same GameObject
                     {
                          var targetComponentFromOther = componentSource.GetComponent<T>();
                          if (targetComponentFromOther != null)
                          {
                               if (!directTargets.Contains(targetComponentFromOther))
                               {
                                    directTargets.Add(targetComponentFromOther);
                                    addedTarget = true;
                               }
                          }
                     }
                }

                // If the dropped object is none of the above, log a warning (optional).
                // Debug.LogWarning($"Dropped object {o.name} of type {o.GetType()} is not compatible with target type {typeof(T)}.");
            }

            // Return true if at least one target was successfully added from the drop operation.
            return addedTarget;
        }

        public bool HasError(out string error)
        {
            error = string.Empty;
            // Check if the list is null or empty.
            if (directTargets == null || directTargets.Count == 0)
            {
                error = "Direct targets are not set.";
                return true; // Errors found
            }
            // Check for any null entries in the list.
            if (directTargets.Any(x => x == null))
            {
                error = "Direct targets contain null entries.";
                return true; // Errors found
            }
            return false; // No errors
        }
    }
}