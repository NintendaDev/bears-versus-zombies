using UnityEditor;
using UnityEngine;

namespace OM.Editor
{
    /// <summary>
    /// Custom property drawer for fields marked with <see cref="OM_CheckForNullAttribute"/>.
    /// Displays a help box in the Inspector if the reference is null.
    /// </summary>
    [CustomPropertyDrawer(typeof(OM_CheckForNullAttribute))]
    public class OM_CheckForNullAttributeDrawer : PropertyDrawer
    {
        /// <summary>
        /// Draws the property field and a help box if the reference is null.
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the property GUI.</param>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        /// <param name="label">The label of this property.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var propertyRect = new Rect(position)
            {
                height = EditorGUIUtility.singleLineHeight
            };

            EditorGUI.PropertyField(propertyRect, property, label);
            
            if (property.objectReferenceValue == null)
            {
                var rect = new Rect(position);
                rect.y += EditorGUIUtility.singleLineHeight;
                rect.height = EditorGUIUtility.singleLineHeight * 2;

                EditorGUI.HelpBox(rect, "Reference is null", MessageType.Error);
            }
        }

        /// <summary>
        /// Calculates the height needed for the property including the help box if the reference is null.
        /// </summary>
        /// <param name="property">The SerializedProperty whose height is being calculated.</param>
        /// <param name="label">The label of the property.</param>
        /// <returns>The height in pixels needed to display the property.</returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = EditorGUIUtility.singleLineHeight;
            if (property.objectReferenceValue == null)
                height += EditorGUIUtility.singleLineHeight * 2;
            return height;
        }
    }
}
