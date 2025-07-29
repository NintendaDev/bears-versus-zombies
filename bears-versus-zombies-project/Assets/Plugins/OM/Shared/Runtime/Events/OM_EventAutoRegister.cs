using System.Reflection;
using UnityEngine;

namespace OM
{
    /// <summary>
    /// Provides extension methods to automatically register and unregister event listeners
    /// based on the IOM_EventListener&lt;T&gt; interface.
    /// </summary>
    public static class OM_EventAutoRegister
    {
        /// <summary>
        /// Registers all event listener interfaces implemented by the given MonoBehaviour.
        /// </summary>
        /// <param name="target">The MonoBehaviour implementing IOM_EventListener&lt;T&gt;.</param>
        public static void RegisterListeners(this MonoBehaviour target)
        {
            var interfaces = target.GetType().GetInterfaces();
            foreach (var i in interfaces)
            {
                if (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IOM_EventListener<>))
                {
                    var eventType = i.GetGenericArguments()[0];
                    var method = typeof(OM_EventAutoRegister)
                        .GetMethod("SubscribeGeneric", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                        ?.MakeGenericMethod(eventType);
                    method?.Invoke(null, new object[] { target });
                }
            }
        }

        /// <summary>
        /// Generic method to subscribe a listener to a typed event.
        /// </summary>
        /// <typeparam name="T">The event type.</typeparam>
        /// <param name="listener">The listener instance.</param>
        private static void SubscribeGeneric<T>(IOM_EventListener<T> listener)
        {
            OM_EventsManager.Subscribe<T>(listener.OnEvent);
        }

        /// <summary>
        /// Unregisters all event listener interfaces implemented by the given MonoBehaviour.
        /// </summary>
        /// <param name="target">The MonoBehaviour implementing IOM_EventListener&lt;T&gt;.</param>
        public static void UnregisterListeners(this MonoBehaviour target)
        {
            var interfaces = target.GetType().GetInterfaces();
            foreach (var i in interfaces)
            {
                if (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IOM_EventListener<>))
                {
                    var eventType = i.GetGenericArguments()[0];
                    var method = typeof(OM_EventAutoRegister)
                        .GetMethod("UnsubscribeGeneric", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                        ?.MakeGenericMethod(eventType);
                    method?.Invoke(null, new object[] { target });
                }
            }
        }

        /// <summary>
        /// Generic method to unsubscribe a listener from a typed event.
        /// </summary>
        /// <typeparam name="T">The event type.</typeparam>
        /// <param name="listener">The listener instance.</param>
        private static void UnsubscribeGeneric<T>(IOM_EventListener<T> listener)
        {
            OM_EventsManager.Unsubscribe<T>(listener.OnEvent);
        }
    }
}
