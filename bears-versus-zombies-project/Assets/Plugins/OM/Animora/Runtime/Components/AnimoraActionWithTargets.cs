using System.Collections.Generic;
using UnityEngine;

namespace OM.Animora.Runtime
{
    [System.Serializable]
    public abstract class AnimoraActionWithTargets<T> : AnimoraAction where T : Component
    {
        [SerializeField] private bool useCustomTargets = false;
        [SerializeField] private T[] customTargets;

        /// <summary>
        /// Helper method to safely cast or get components of type T from a collection of generic Components.
        /// Logs a warning for any components that fail the cast/retrieval.
        /// Yields only the successfully converted components.
        /// </summary>
        /// <typeparam name="T">The desired component type.</typeparam>
        /// <param name="targets">The source collection of components.</param>
        /// <returns>An enumerable sequence of components cast to type T.</returns>
        public virtual IEnumerable<T> GetTargets(IEnumerable<Component> targets)
        {
            if (useCustomTargets)
            {
                foreach (var target in customTargets)
                {
                    yield return target;
                }
                yield break;
            }

            foreach (var target in targets)
            {
                if (target is T t) // Direct cast
                {
                    yield return t;
                    continue;
                }
                if (target.TryGetComponent(out T result)) // Try GetComponent
                {
                    yield return result;
                    continue;
                }
                else // Log warning if incompatible
                {
                    Debug.LogWarning($"Target {target} is not a {typeof(T)}. Skipping action logic for this target.");
                }
            }
        }
    }
}