using System;
using System.Collections.Generic;
using UnityEngine;

namespace OM
{
    /// <summary>
    /// A static event manager supporting both typed and named event subscriptions.
    /// Provides publish-subscribe functionality for decoupled communication.
    /// </summary>
    public static class OM_EventsManager
    {
#if UNITY_EDITOR
        /// <summary>
        /// Invoked in the Editor whenever an event is published (typed or named).
        /// Used for debugging and visualization purposes.
        /// </summary>
        public static Action<string> OnEventPublished;
#endif

        private static readonly Dictionary<Type, Delegate> TypedEvents = new();
        private static readonly Dictionary<string, Action> SimpleNamedEvents = new();
        private static readonly Dictionary<string, Delegate> GenericNamedEvents = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ResetStatics()
        {
            TypedEvents.Clear();
            SimpleNamedEvents.Clear();
            GenericNamedEvents.Clear();
        }

        /// <summary>
        /// Subscribes a callback to a typed event.
        /// </summary>
        public static void Subscribe<T>(Action<T> callback)
        {
            if (TypedEvents.TryGetValue(typeof(T), out var del))
                TypedEvents[typeof(T)] = Delegate.Combine(del, callback);
            else
                TypedEvents[typeof(T)] = callback;
        }

        /// <summary>
        /// Unsubscribes a callback from a typed event.
        /// </summary>
        public static void Unsubscribe<T>(Action<T> callback)
        {
            if (TypedEvents.TryGetValue(typeof(T), out var del))
            {
                var newDel = Delegate.Remove(del, callback);
                if (newDel == null)
                    TypedEvents.Remove(typeof(T));
                else
                    TypedEvents[typeof(T)] = newDel;
            }
        }

        /// <summary>
        /// Subscribes a callback to a named event with no parameters.
        /// </summary>
        public static void Subscribe(string key, Action callback)
        {
            if (SimpleNamedEvents.TryGetValue(key, out var existing))
                SimpleNamedEvents[key] = (Action)Delegate.Combine(existing, callback);
            else
                SimpleNamedEvents[key] = callback;
        }

        /// <summary>
        /// Subscribes a callback to a named event with a parameter.
        /// </summary>
        public static void Subscribe<T>(string key, Action<T> callback)
        {
            if (GenericNamedEvents.TryGetValue(key, out var existing))
            {
                if (existing is not Action<T>)
                    throw new InvalidOperationException($"Key '{key}' is already registered with a different parameter type.");

                GenericNamedEvents[key] = Delegate.Combine(existing, callback);
            }
            else
            {
                GenericNamedEvents[key] = callback;
            }
        }

        /// <summary>
        /// Unsubscribes a callback from a named event with no parameters.
        /// </summary>
        public static void Unsubscribe(string key, Action callback)
        {
            if (SimpleNamedEvents.TryGetValue(key, out var existing))
            {
                var updated = Delegate.Remove(existing, callback);
                if (updated == null)
                    SimpleNamedEvents.Remove(key);
                else
                    SimpleNamedEvents[key] = (Action)updated;
            }
        }

        /// <summary>
        /// Unsubscribes a callback from a named event with parameters.
        /// </summary>
        public static void Unsubscribe<T>(string key, Action<T> callback)
        {
            if (GenericNamedEvents.TryGetValue(key, out var existing))
            {
                if (existing is not Action<T>)
                    throw new InvalidOperationException($"Key '{key}' is not registered with Action<{typeof(T).Name}>.");

                var updated = Delegate.Remove(existing, callback);
                if (updated == null)
                    GenericNamedEvents.Remove(key);
                else
                    GenericNamedEvents[key] = updated;
            }
        }

        /// <summary>
        /// Publishes a typed event.
        /// </summary>
        public static void Publish<T>(T data)
        {
            if (TypedEvents.TryGetValue(typeof(T), out var del))
                (del as Action<T>)?.Invoke(data);

#if UNITY_EDITOR
            OnEventPublished?.Invoke(typeof(T).Name);
#endif
        }

        /// <summary>
        /// Publishes a named event with no parameters.
        /// </summary>
        public static void Publish(string key)
        {
            if (SimpleNamedEvents.TryGetValue(key, out var action))
                action?.Invoke();

#if UNITY_EDITOR
            OnEventPublished?.Invoke(key);
#endif
        }

        /// <summary>
        /// Publishes a named event with a parameter. Optionally also triggers any matching simple event.
        /// </summary>
        public static void Publish<T>(string key, T arg, bool triggerSimpleEventsIfExists = true)
        {
            if (GenericNamedEvents.TryGetValue(key, out var del) && del is Action<T> action)
            {
                action(arg);
            }

            if (triggerSimpleEventsIfExists && SimpleNamedEvents.TryGetValue(key, out var simpleAction))
            {
                simpleAction?.Invoke();
            }

#if UNITY_EDITOR
            OnEventPublished?.Invoke(key);
#endif
        }

        /// <summary>
        /// Clears all stored events of all types.
        /// </summary>
        public static void ClearAllEvents()
        {
            TypedEvents.Clear();
            SimpleNamedEvents.Clear();
            GenericNamedEvents.Clear();
        }

        /// <summary>
        /// Removes all listeners associated with a named key (both simple and generic).
        /// </summary>
        public static void UnsubscribeAll(string key)
        {
            SimpleNamedEvents.Remove(key);
            GenericNamedEvents.Remove(key);
        }

        /// <summary>
        /// Clears a specific typed event by its type.
        /// </summary>
        public static void ClearTypedEvent<T>() => TypedEvents.Remove(typeof(T));

        /// <summary>
        /// Clears all named event listeners associated with the given key.
        /// </summary>
        public static void ClearNamedEvent(string key)
        {
            SimpleNamedEvents.Remove(key);
            GenericNamedEvents.Remove(key);
        }

        /// <summary>
        /// Checks if a typed event has listeners.
        /// </summary>
        public static bool HasTypedEvent<T>() => TypedEvents.ContainsKey(typeof(T));

        /// <summary>
        /// Checks if a named event has listeners.
        /// </summary>
        public static bool HasNamedEvent(string key)
        {
            return SimpleNamedEvents.ContainsKey(key) || GenericNamedEvents.ContainsKey(key);
        }

        /// <summary>
        /// Logs all subscribed events and their listener counts to the console.
        /// </summary>
        public static void LogAllEvents()
        {
            Debug.Log("=== EventManager Subscriptions ===");

            Debug.Log($"[Typed] Count: {TypedEvents.Count}");
            foreach (var e in TypedEvents)
                Debug.Log($"  Type: {e.Key.Name} → Listeners: {e.Value?.GetInvocationList().Length}");

            Debug.Log($"[Named] Count: {SimpleNamedEvents.Count}");
            foreach (var e in SimpleNamedEvents)
                Debug.Log($"  Key: \"{e.Key}\" → Listeners: {e.Value?.GetInvocationList().Length}");

            Debug.Log($"[Generic Named] Count: {GenericNamedEvents.Count}");
            foreach (var e in GenericNamedEvents)
                Debug.Log($"  Key: \"{e.Key}\" → Listeners: {e.Value?.GetInvocationList().Length}");
        }

#if UNITY_EDITOR
        /// <summary>
        /// Returns all typed events for editor debugging.
        /// </summary>
        public static Dictionary<Type, Delegate> Debug_GetTypedEvents() => TypedEvents;

        /// <summary>
        /// Returns all named events (simple + generic) for editor debugging.
        /// </summary>
        public static Dictionary<string, Delegate> Debug_GetNamedEvents()
        {
            var combined = new Dictionary<string, Delegate>();

            foreach (var pair in SimpleNamedEvents)
                combined[pair.Key] = pair.Value;

            foreach (var pair in GenericNamedEvents)
                combined[pair.Key] = pair.Value;

            return combined;
        }

        /// <summary>
        /// Clears a specific typed event by its System.Type.
        /// </summary>
        public static void ClearTypedEventByType(Type type) => TypedEvents.Remove(type);
#endif
    }
}
