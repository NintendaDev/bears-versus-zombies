using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace OM.Editor
{
    /// <summary>
    /// A visual element that automatically draws all visible child properties of a serialized object.
    /// Supports optional filtering and a global change callback.
    /// </summary>
    public class OM_DrawAllPropertiesElement : VisualElement
    {
        private readonly SerializedProperty _property;
        private readonly Action<SerializedPropertyChangeEvent> _onAnyChangedEvent;

        /// <summary>
        /// Constructs the element and adds property fields for all serialized child properties.
        /// </summary>
        /// <param name="property">The root serialized property to iterate over.</param>
        /// <param name="onAnyChanged">Callback invoked when any property value changes.</param>
        /// <param name="canDraw">Optional filter to decide whether a property should be drawn.</param>
        public OM_DrawAllPropertiesElement(SerializedProperty property, Action<SerializedPropertyChangeEvent> onAnyChanged, Func<SerializedProperty, bool> canDraw = null)
        {
            _property = property;
            _onAnyChangedEvent = onAnyChanged;

            foreach (var prop in property.GetAllProperties())
            {
                if (canDraw?.Invoke(prop) == false) continue;

                var field = new PropertyField(prop);
                field.RegisterValueChangeCallback(OnAnyChanged);
                Add(field);
            }
        }

        /// <summary>
        /// Internal method that fires the change event callback when a field changes.
        /// </summary>
        /// <param name="evt">The change event triggered by a field.</param>
        private void OnAnyChanged(SerializedPropertyChangeEvent evt)
        {
            _onAnyChangedEvent?.Invoke(evt);
        }
    }
}