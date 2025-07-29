using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace OM.Editor
{
    /// <summary>
    /// An EditorWindow for debugging and managing runtime events published via OM_EventsManager.
    /// Shows recently triggered events, listener counts, and allows clearing events.
    /// </summary>
    public class OM_EventManagerDebuggerWindow : EditorWindow
    {
        private static readonly Dictionary<string, double> RecentlyFired = new();
        private const double FlashDuration = 1.0;

        private static readonly Queue<(string eventName, double time)> LastEvents = new();
        private const int MaxTrackedEvents = 10;

        private Vector2 _scrollPos;

        /// <summary>
        /// Opens the Event Manager Debugger window.
        /// </summary>
        [MenuItem("Window/OM/Event Manager Debugger")]
        public static void OpenWindow()
        {
            GetWindow<OM_EventManagerDebuggerWindow>("OM Event Manager");
        }

        private void OnEnable()
        {
            OM_EventsManager.OnEventPublished = (eventName) =>
            {
                double now = EditorApplication.timeSinceStartup;
                RecentlyFired[eventName] = now;

                LastEvents.Enqueue((eventName, Time.timeSinceLevelLoad));
                while (LastEvents.Count > MaxTrackedEvents)
                    LastEvents.Dequeue();

                Repaint();
            };

            EditorApplication.update += Repaint;
            EditorApplication.playModeStateChanged += OnModeStateChanged;
        }

        private void OnDisable()
        {
            OM_EventsManager.OnEventPublished = null;
            EditorApplication.update -= Repaint;
            EditorApplication.playModeStateChanged -= OnModeStateChanged;
        }

        /// <summary>
        /// Clears event tracking when play mode changes.
        /// </summary>
        private void OnModeStateChanged(PlayModeStateChange modeState)
        {
            RecentlyFired.Clear();
            LastEvents.Clear();
            Repaint();
        }

        private void OnGUI()
        {
            GUILayout.Label("🧠 Event Manager Debugger", EditorStyles.boldLabel);
            GUILayout.Space(10);

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            DrawTypedEvents();
            DrawNamedEvents();
            DrawLastEventsList();

            EditorGUILayout.EndScrollView();

            GUILayout.Space(10);
            if (GUILayout.Button("🧹 Clear All Events"))
            {
                OM_EventsManager.ClearAllEvents();
            }
        }

        /// <summary>
        /// Displays a list of the most recently triggered events.
        /// </summary>
        private void DrawLastEventsList()
        {
            GUILayout.Label("🕒 Recently Triggered Events", EditorStyles.boldLabel);

            if (LastEvents.Count == 0)
            {
                GUILayout.Label("No events fired yet.");
                return;
            }

            foreach (var (eventName, time) in LastEvents.Reverse())
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(eventName, GUILayout.Width(250));
                GUILayout.Label($"at {time:0.00}s");
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(10);
        }

        /// <summary>
        /// Draws all typed events (generic events with type info) currently tracked.
        /// </summary>
        private void DrawTypedEvents()
        {
            GUILayout.Label("📦 Typed Events", EditorStyles.boldLabel);
            var typed = OM_EventsManager.Debug_GetTypedEvents();
            if (typed.Count == 0) GUILayout.Label("None");

            foreach (var key in typed.Keys.ToList())
            {
                var del = typed[key];
                string eventId = key.Name;

                double timeSinceFired = RecentlyFired.TryGetValue(eventId, out var lastTime)
                    ? EditorApplication.timeSinceStartup - lastTime
                    : double.MaxValue;

                Color bgColor = GUI.backgroundColor;

                if (timeSinceFired < FlashDuration)
                {
                    float t = 1f - (float)(timeSinceFired / FlashDuration);
                    GUI.backgroundColor = Color.Lerp(Color.white, Color.green, t);
                }

                EditorGUILayout.BeginHorizontal("box");
                GUILayout.Label(eventId, GUILayout.Width(250));
                int listenerCount = del?.GetInvocationList().Length ?? 0;
                GUILayout.Label($"Listeners: {listenerCount}");

                if (GUILayout.Button("❌", GUILayout.Width(25)))
                    OM_EventsManager.ClearTypedEventByType(key);
                EditorGUILayout.EndHorizontal();

                GUI.backgroundColor = bgColor;
            }

            GUILayout.Space(10);
        }

        /// <summary>
        /// Draws all named (string-identified) events currently tracked.
        /// </summary>
        private void DrawNamedEvents()
        {
            GUILayout.Label("🔖 Named Events", EditorStyles.boldLabel);
            var named = OM_EventsManager.Debug_GetNamedEvents();
            if (named.Count == 0) GUILayout.Label("None");

            foreach (var key in named.Keys.ToList())
            {
                var del = named[key];
                string eventId = key;

                double timeSinceFired = RecentlyFired.TryGetValue(eventId, out var lastTime)
                    ? EditorApplication.timeSinceStartup - lastTime
                    : double.MaxValue;

                Color bgColor = GUI.backgroundColor;

                if (timeSinceFired < FlashDuration)
                {
                    float t = 1f - (float)(timeSinceFired / FlashDuration);
                    GUI.backgroundColor = Color.Lerp(Color.white, Color.green, t);
                }

                EditorGUILayout.BeginHorizontal("box");
                GUILayout.Label(eventId, GUILayout.Width(250));
                int listenerCount = del?.GetInvocationList().Length ?? 0;
                GUILayout.Label($"Listeners: {listenerCount}");

                if (GUILayout.Button("❌", GUILayout.Width(25)))
                    OM_EventsManager.ClearNamedEvent(key);
                EditorGUILayout.EndHorizontal();

                GUI.backgroundColor = bgColor;
            }
        }
    }
}
