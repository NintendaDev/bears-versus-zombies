using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OM
{
    /// <summary>
    /// A handle that represents a running coroutine, allowing it to be stopped externally.
    /// </summary>
    public readonly struct OM_CoroutineHandle
    {
        private readonly object _target;
        private readonly Coroutine _coroutine;

        /// <summary>
        /// Initializes a new instance of the <see cref="OM_CoroutineHandle"/> struct.
        /// </summary>
        /// <param name="coroutine">The coroutine instance.</param>
        /// <param name="target">The owner of the coroutine.</param>
        public OM_CoroutineHandle(Coroutine coroutine, object target)
        {
            _coroutine = coroutine;
            _target = target;
        }

        /// <summary>
        /// Stops the coroutine associated with this handle.
        /// </summary>
        public void Stop()
        {
            if (_coroutine != null)
            {
                OM_CoroutineManager.StopCoroutine(this, _coroutine);
            }
        }
    }

    /// <summary>
    /// Manages coroutines globally with support for grouping and targeted stopping.
    /// </summary>
    public class OM_CoroutineManager : MonoBehaviour
    {
        private static OM_CoroutineManager _instance;

        /// <summary>
        /// Gets the singleton instance of the coroutine manager.
        /// </summary>
        public static OM_CoroutineManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<OM_CoroutineManager>();
                    if (_instance == null)
                    {
                        var go = new GameObject("OM_CoroutineManager");
                        _instance = go.AddComponent<OM_CoroutineManager>();
                    }
                }

                if (_instance == null) Debug.LogError("No Coroutine Manager Found");

                return _instance;
            }
        }

        private readonly Dictionary<object, List<Coroutine>> _coroutines = new();

        /// <summary>
        /// Stops all coroutines started by the given target.
        /// </summary>
        /// <param name="target">The object that started the coroutines.</param>
        public static void StopCoroutines(object target)
        {
            if (Instance._coroutines.TryGetValue(target, out var coroutines))
            {
                foreach (var coroutine in coroutines)
                {
                    Instance.StopCoroutine(coroutine);
                }
                coroutines.Clear();
            }
        }

        /// <summary>
        /// Stops a specific coroutine started by a target.
        /// </summary>
        /// <param name="target">The owner of the coroutine.</param>
        /// <param name="routine">The coroutine instance.</param>
        public static void StopCoroutine(object target, Coroutine routine)
        {
            if (Instance._coroutines.TryGetValue(target, out var coroutines))
            {
                if (coroutines.Contains(routine))
                {
                    Instance.StopCoroutine(routine);
                    coroutines.Remove(routine);
                }
            }
        }

        /// <summary>
        /// Starts a coroutine associated with a specific target and returns a handle.
        /// </summary>
        /// <param name="target">The object starting the coroutine.</param>
        /// <param name="enumerator">A function returning the coroutine enumerator.</param>
        /// <returns>A handle for controlling the coroutine.</returns>
        public static OM_CoroutineHandle StartCoroutine(object target, Func<IEnumerator> enumerator)
        {
            var startCoroutine = Instance.StartCoroutine(enumerator());

            if (Instance._coroutines.TryGetValue(target, out var coroutines))
            {
                coroutines.Add(startCoroutine);
            }
            else
            {
                Instance._coroutines.Add(target, new List<Coroutine> { startCoroutine });
            }

            return new OM_CoroutineHandle(startCoroutine, target);
        }
    }
}
