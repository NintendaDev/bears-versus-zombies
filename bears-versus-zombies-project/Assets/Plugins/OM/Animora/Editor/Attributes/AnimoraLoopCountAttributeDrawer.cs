using OM.Animora.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.Animora.Editor
{
    /// <summary>
    /// Custom property drawer for <see cref="AnimoraLoopCountAttribute"/>.
    /// Displays a help message if the int value is below the defined minimum.
    /// Supports both IMGUI and UI Toolkit.
    /// </summary>
    [CustomPropertyDrawer(typeof(AnimoraLoopCountAttribute), true)]
    public class AnimoraLoopCountAttributeDrawer : PropertyDrawer
    {
        private HelpBox _helpBox;

        /// <summary>
        /// Calculates the height required for the property, including space for the help box if shown.
        /// </summary>
        /// <param name="property">The serialized int property.</param>
        /// <param name="label">The label to display.</param>
        /// <returns>The height in pixels needed for the property and help box.</returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var propertyAttribute = (AnimoraLoopCountAttribute)this.attribute;

            if (property.intValue < propertyAttribute.Min)
            {
                return EditorGUIUtility.singleLineHeight * 2;
            }

            return EditorGUIUtility.singleLineHeight;
        }

        /// <summary>
        /// Renders the property field and an error help box if the value is too low (IMGUI).
        /// </summary>
        /// <param name="position">The rectangle area to draw in.</param>
        /// <param name="property">The serialized int property.</param>
        /// <param name="label">The property's label.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var propertyAttribute = (AnimoraLoopCountAttribute)this.attribute;

            if (property.intValue < propertyAttribute.Min)
            {
                EditorGUI.HelpBox(position, propertyAttribute.MinMessage, MessageType.Error);
                position.y += EditorGUIUtility.singleLineHeight;
            }

            EditorGUI.PropertyField(position, property, label);
        }

        /// <summary>
        /// Creates the UI Toolkit-based property field with a conditional help box.
        /// </summary>
        /// <param name="property">The serialized int property.</param>
        /// <returns>A <see cref="VisualElement"/> containing the UI representation.</returns>
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var propertyAttribute = (AnimoraLoopCountAttribute)this.attribute;

            var root = new VisualElement();

            var propertyField = new PropertyField(property);
            propertyField.Bind(property.serializedObject);
            root.Add(propertyField);

            _helpBox = new HelpBox(propertyAttribute.MinMessage, HelpBoxMessageType.Info);
            _helpBox.style.display = property.intValue <= propertyAttribute.Min ? DisplayStyle.Flex : DisplayStyle.None;
            root.Add(_helpBox);

            propertyField.RegisterValueChangeCallback(e =>
            {
                _helpBox.style.display = property.intValue <= propertyAttribute.Min ? DisplayStyle.Flex : DisplayStyle.None;
            });

            return root;
        }
    }
}
