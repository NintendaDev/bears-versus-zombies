using OM.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace OM.TimelineCreator.Editor.Drawers
{
    /// <summary>
    /// Custom editor for the <see cref="OM_TimelineSettings"/> ScriptableObject.
    /// This class defines how the settings are displayed in the Unity Inspector window
    /// when the OM_TimelineSettings asset is selected. It uses UIElements for rendering.
    /// </summary>
    [CustomEditor(typeof(OM_TimelineSettings), true)] // Apply this editor to OM_TimelineSettings assets
    public class OM_TimelineSettingsDrawer : UnityEditor.Editor // Inherits from the base Editor class
    {
        /// <summary>
        /// Creates the UIElements-based inspector GUI for the OM_TimelineSettings asset.
        /// This method is called by Unity to build the visual tree for the inspector.
        /// </summary>
        /// <returns>The root VisualElement containing the inspector UI.</returns>
        public override VisualElement CreateInspectorGUI()
        {
            // Initialize the root container for the settings group.
            OM_FoldoutGroup foldoutGroup = null;

            // Iterate through all serialized properties of the OM_TimelineSettings object.
            // GetAllProperties() is likely an extension method to simplify iteration.
            foreach (var childProperty in serializedObject.GetAllProperties())
            {
                // Skip the default "m_Script" property field that Unity adds for ScriptableObjects/MonoBehaviours.
                if (childProperty.name == "m_Script") continue;

                // If this is the first property (after m_Script), create the main foldout group.
                // All settings will be placed inside this group.
                if (foldoutGroup == null)
                {
                    // Create a foldout group using a helper method, providing a title.
                    // Pass a copy of the property to avoid iterator issues if the factory method uses it.
                    foldoutGroup = OM_FoldoutGroup.CreateSettingsGroup(childProperty.Copy(), "Timeline Settings", "Settings");
                }

                // Create a standard PropertyField for the current serialized property.
                // This automatically displays the appropriate UI control (e.g., toggle for bool, float field for float).
                var propertyField = new PropertyField(childProperty);
                // Bind the PropertyField to the serialized property. This enables two-way data binding:
                // UI changes update the property, and property changes update the UI.
                propertyField.Bind(childProperty.serializedObject);
                // Add the created PropertyField to the content area of the foldout group.
                foldoutGroup.AddToContent(propertyField);
            }

            // Return the created foldout group as the root of the inspector UI.
            // If no properties were found (except m_Script), this might return null,
            // though typically a ScriptableObject will have properties.
            return foldoutGroup;
        }

        /// <summary>
        /// Overrides the default IMGUI-based inspector rendering.
        /// Note: If `CreateInspectorGUI` returns a non-null VisualElement, this method is usually
        /// NOT called by Unity for the main inspector rendering. It might be called in other contexts
        /// or serve as a fallback if UIElements generation fails.
        /// The current implementation simply draws the default inspector and handles object updates.
        /// </summary>
        public override void OnInspectorGUI()
        {
            // Reads the latest state from the target object into the SerializedObject representation.
            // Important before making changes or drawing controls based on serialized properties.
            serializedObject.Update();

            // Draws the default inspector fields for the target object, as if this custom editor didn't exist.
            // If using CreateInspectorGUI, this line might be redundant or have no visible effect in the main inspector.
            DrawDefaultInspector();

            // Applies any modified properties from the SerializedObject back to the actual target object.
            // Handles marking the object dirty and saving changes.
            serializedObject.ApplyModifiedProperties();
        }
    }
}