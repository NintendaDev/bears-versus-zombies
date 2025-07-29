using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.Editor
{
    /// <summary>
    /// A custom toggle switch UI element with animated visuals and color transitions.
    /// Can be bound to a SerializedProperty or used with a callback.
    /// </summary>
    public class OM_Switcher : BaseBoolField
    {
        private static readonly Color OnColor = new Color(0.26f, 0.8f, 0.2f);
        private static readonly Color OffColor = new Color(0.8f, 0.8f, 0.8f);

        private readonly VisualElement _checkboxContainer;
        private readonly VisualElement _checkboxHandle;

        /// <summary>
        /// Initializes the switcher element with a label and sets up styles and visuals.
        /// </summary>
        /// <param name="label">The text label for the switcher.</param>
        protected OM_Switcher(string label) : base(label)
        {
            this.AddStyleSheet("OM_Switcher").AddClassNames("jt-switcher");

            _checkboxContainer = new VisualElement()
                .AddClassNames("jt-switcher-container")
                .AddTo(this);

            _checkboxHandle = new VisualElement()
                .SetPickingMode(PickingMode.Ignore)
                .AddClassNames("jt-switcher-handle")
                .AddTo(_checkboxContainer);

            this.RegisterValueChangedCallback(OnChanged);

            // Prevent animation on initial setup
            _checkboxHandle.AddToClassList("no-animation");

            schedule.Execute(() =>
            {
                UpdateVisual();
                _checkboxHandle.RemoveFromClassList("no-animation");
            }).ExecuteLater(100);
        }

        /// <summary>
        /// Triggered when the value of the switcher changes.
        /// Updates visual state.
        /// </summary>
        /// <param name="evt">The change event data.</param>
        private void OnChanged(ChangeEvent<bool> evt)
        {
            schedule.Execute(UpdateVisual).ExecuteLater(20);
        }

        /// <summary>
        /// Updates the handle position and background color based on current toggle state.
        /// </summary>
        private void UpdateVisual()
        {
            _checkboxHandle.style.top = 1;
            _checkboxHandle.style.left = value ? 15 : 2;
            _checkboxContainer.style.backgroundColor = value ? OnColor : OffColor;
        }

        /// <summary>
        /// Creates a switcher bound to a SerializedProperty and triggers a callback on change.
        /// </summary>
        /// <param name="label">Label for the switcher.</param>
        /// <param name="property">SerializedProperty to bind to.</param>
        /// <param name="onChangeCallback">Callback executed when the switch value changes.</param>
        /// <returns>A new OM_Switcher instance.</returns>
        public static OM_Switcher CreateSwitcher(string label, SerializedProperty property, Action<bool> onChangeCallback)
        {
            var switcher = new OM_Switcher(label);
            switcher.BindProperty(property);
            switcher.Bind(property.serializedObject);
            switcher.RegisterValueChangedCallback(e =>
            {
                onChangeCallback?.Invoke(e.newValue);
            });
            return switcher;
        }

        /// <summary>
        /// Creates an unbound switcher with a change callback.
        /// </summary>
        /// <param name="label">Label for the switcher.</param>
        /// <param name="onChangeCallback">Callback executed when the switch value changes.</param>
        /// <returns>A new OM_Switcher instance.</returns>
        public static OM_Switcher CreateSwitcher(string label, Action<bool> onChangeCallback)
        {
            var switcher = new OM_Switcher(label);
            switcher.RegisterValueChangedCallback(e =>
            {
                onChangeCallback?.Invoke(e.newValue);
            });
            return switcher;
        }
    }
}
