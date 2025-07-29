using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.Editor
{
    /// <summary>
    /// A UIElements Manipulator that attaches keyboard shortcut handling to a VisualElement.
    /// It allows defining specific actions to be triggered when certain KeyCodes are pressed,
    /// typically in combination with the command/control modifier key.
    /// It also supports a general callback for any KeyDown event.
    /// </summary>
    public class OM_ElementShortcutsManipulator : Manipulator
    {
        /// <summary>
        /// Dictionary mapping specific KeyCodes to parameterless Actions that should be executed
        /// when the corresponding key is pressed (usually along with the command/control modifier).
        /// </summary>
        private readonly Dictionary<KeyCode, Action> _actions;

        /// <summary>
        /// An optional Action that is invoked every time any KeyDown event occurs on the target element.
        /// Receives the KeyDownEvent arguments, allowing for more general key handling or preprocessing.
        /// Can be null if no general keydown handling is needed.
        /// </summary>
        private readonly Action<KeyDownEvent> _keydown;

        /// <summary>
        /// Initializes a new instance of the OM_ElementShortcutsManipulator.
        /// </summary>
        /// <param name="actions">A dictionary mapping KeyCodes to the Actions to execute.</param>
        /// <param name="keydown">An optional Action to handle any KeyDown event.</param>
        public OM_ElementShortcutsManipulator(Dictionary<KeyCode, Action> actions, Action<KeyDownEvent> keydown = null) // Made keydown optional
        {
            _actions = actions ?? new Dictionary<KeyCode, Action>(); // Ensure actions is not null
            _keydown = keydown;
        }

        /// <summary>
        /// Registers the necessary keyboard event callbacks on the target VisualElement.
        /// Called automatically when the manipulator is added to an element.
        /// </summary>
        protected override void RegisterCallbacksOnTarget()
        {
            // Listen for key down and key up events on the element this manipulator is attached to.
            target.RegisterCallback<KeyDownEvent>(OnKeyDown);
            target.RegisterCallback<KeyUpEvent>(OnKeyUp); // Register KeyUp, although it's currently unused.
        }

        /// <summary>
        /// Unregisters the keyboard event callbacks from the target VisualElement.
        /// Called automatically when the manipulator is removed from an element or the element is removed from the hierarchy.
        /// </summary>
        protected override void UnregisterCallbacksFromTarget()
        {
            // Stop listening to the events to prevent memory leaks and unexpected behavior.
            target.UnregisterCallback<KeyDownEvent>(OnKeyDown);
            target.UnregisterCallback<KeyUpEvent>(OnKeyUp);
        }

        /// <summary>
        /// Handles the KeyUpEvent. Currently has no implementation but could be used for key release logic.
        /// </summary>
        /// <param name="evt">The KeyUpEvent arguments.</param>
        private void OnKeyUp(KeyUpEvent evt)
        {
            // Placeholder for potential key release logic (e.g., stopping an action started on key down).
        }

        /// <summary>
        /// Handles the KeyDownEvent. Invokes the general keydown callback and checks for specific shortcut combinations.
        /// </summary>
        /// <param name="evt">The KeyDownEvent arguments.</param>
        private void OnKeyDown(KeyDownEvent evt)
        {
            // 1. Invoke the general keydown callback if it exists.
            _keydown?.Invoke(evt);

            // 2. Check for specific shortcuts defined in the _actions dictionary.
            // --- IMPORTANT: This implementation REQUIRES the command/control key modifier ---
            // Only proceed if the command (macOS) or control (Windows/Linux) key is held down.
            if (!evt.commandKey) return;

            // 3. Iterate through the registered shortcut actions.
            if (_actions != null) // Check if actions dictionary exists
            {
                foreach (var pair in _actions)
                {
                    // If the pressed key matches a key in the dictionary...
                    if (pair.Key == evt.keyCode)
                    {
                        // ...invoke the corresponding action.
                        pair.Value?.Invoke();
                        // Optional: Stop propagation if the shortcut was handled?
                        // evt.StopPropagation();
                        // Consider if multiple actions for the same key (with different modifiers handled elsewhere) are needed.
                        // If only one action per key+command is intended, could 'break' here.
                    }
                }
            }
        }
    }
}
