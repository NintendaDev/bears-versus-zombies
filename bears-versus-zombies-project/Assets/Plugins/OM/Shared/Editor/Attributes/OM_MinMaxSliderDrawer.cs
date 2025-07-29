using UnityEditor;
using UnityEngine;

namespace OM.Editor
{
    /// <summary>
    /// Custom property drawer for fields marked with <see cref="OM_MinMaxSliderAttribute"/>.
    /// Renders a min-max slider with float fields using a <see cref="Vector2"/> property.
    /// </summary>
    [CustomPropertyDrawer(typeof(OM_MinMaxSliderAttribute))]
    public class OM_MinMaxSliderDrawer : PropertyDrawer
    {
        /// <summary>
        /// Draws the min-max slider UI for <see cref="Vector2"/> properties.
        /// </summary>
        /// <param name="position">Screen rectangle to draw the property GUI in.</param>
        /// <param name="property">The serialized property being drawn.</param>
        /// <param name="label">The label displayed beside the property.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Cast the attribute to access min/max range values
            OM_MinMaxSliderAttribute minMax = attribute as OM_MinMaxSliderAttribute;

            // Only works with Vector2 properties
            if (property.propertyType == SerializedPropertyType.Vector2)
            {
                EditorGUI.BeginProperty(position, label, property);

                // Draw the property label
                position = EditorGUI.PrefixLabel(position, label);

                // Layout calculations
                float fieldWidth = 50f;
                float sliderWidth = position.width - (fieldWidth * 2) - 4;

                Rect minRect = new Rect(position.x, position.y, fieldWidth, position.height);
                Rect sliderRect = new Rect(position.x + fieldWidth + 2, position.y, sliderWidth, position.height);
                Rect maxRect = new Rect(position.x + fieldWidth + sliderWidth + 4, position.y, fieldWidth, position.height);

                // Get and draw the values
                Vector2 range = property.vector2Value;
                range.x = EditorGUI.FloatField(minRect, range.x);
                range.y = EditorGUI.FloatField(maxRect, range.y);

                // Draw the min-max slider
                EditorGUI.MinMaxSlider(sliderRect, ref range.x, ref range.y, minMax.Min, minMax.Max);

                // Clamp the values to ensure correctness
                range.x = Mathf.Clamp(range.x, minMax.Min, range.y);
                range.y = Mathf.Clamp(range.y, range.x, minMax.Max);

                // Apply the modified range
                property.vector2Value = range;

                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use MinMaxSlider with Vector2.");
            }
        }
    }
}
