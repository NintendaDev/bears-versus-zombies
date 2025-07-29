

using System.Collections.Generic;
using OM.Editor;
using OM.TimelineCreator.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace OM.TimelineCreator.Editor.Drawers
{
    /// <summary>
    /// A custom property drawer for OM_ClipBase and its derived classes.
    /// It automatically generates an inspector UI based on the serialized properties of the clip,
    /// respecting attributes like [OM_StartGroup], [OM_EndGroup], and [OM_HideInInspector]
    /// to create foldout groups and hide specific fields.
    /// </summary>
    [CustomPropertyDrawer(typeof(OM_ClipBase), true)] // Apply this drawer to OM_ClipBase and all derived classes (useForChildren = true)
    public class OM_ClipBaseDrawer : PropertyDrawer
    {
        /// <summary>
        /// Gets the root VisualElement for the drawer's UI.
        /// </summary>
        public VisualElement Root { get; private set; }

        /// <summary>
        /// Gets the SerializedProperty representing the OM_ClipBase object being drawn.
        /// A copy is made to avoid modifying the iterator state of the parent drawer.
        /// </summary>
        protected SerializedProperty Property { get; private set; }

        /// <summary>
        /// A list storing references to all generated OM_FoldoutGroup elements.
        /// </summary>
        protected readonly List<OM_FoldoutGroup> FoldoutGroups = new List<OM_FoldoutGroup>();

        /// <summary>
        /// A list storing references to all generated PropertyField elements.
        /// </summary>
        protected readonly List<PropertyField> PropertyFields = new List<PropertyField>();

        /// <summary>
        /// A dictionary mapping property names (string) to their corresponding PropertyField elements.
        /// Allows easy access to specific fields by name.
        /// </summary>
        protected readonly Dictionary<string, PropertyField> PropertyFieldDictionary = new Dictionary<string, PropertyField>();

        /// <summary>
        /// Creates the visual element hierarchy for the custom inspector UI.
        /// This method is called by the Unity Editor when the drawer needs to be rendered.
        /// </summary>
        /// <param name="property">The SerializedProperty representing the OM_ClipBase object.</param>
        /// <returns>The root VisualElement containing the generated UI.</returns>
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            // Make a copy of the property to avoid interfering with Unity's internal property iteration.
            Property = property.Copy();
            // Clear internal lists to ensure a fresh start if the drawer is reused.
            FoldoutGroups.Clear();
            PropertyFields.Clear();
            PropertyFieldDictionary.Clear();

            // Create the root container for this drawer's UI.
            Root = new VisualElement();

            // Keep track of the current foldout group being populated. Starts as null (fields go into Root).
            OM_FoldoutGroup foldoutGroup = null;

            // Iterate through all direct child properties of the main SerializedProperty.
            // GetAllProperties() is likely an extension method to handle this iteration correctly.
            foreach (var childProperty in property.GetAllProperties())
            {
                // --- Attribute Handling ---

                // Check for the [OM_HideInInspector] attribute.
                var hideInInspector = childProperty.GetAttribute<OM_HideInInspector>(true); // Check inherited attributes too
                // If the attribute exists and specifies not to handle other attributes, skip this field entirely.
                if (hideInInspector is { HandleOtherAttributes: false }) continue;

                // Check for the [OM_EndGroup] attribute.
                var endGroup = childProperty.GetAttribute<OM_EndGroup>(true);
                // If found, reset the current foldout group so subsequent fields go back to the Root or a new group.
                if (endGroup != null)
                {
                    foldoutGroup = null;
                }

                // Check for the [OM_StartGroup] attribute.
                var startGroup = childProperty.GetAttribute<OM_StartGroup>(true);
                // If found, create a new foldout group.
                if (startGroup != null)
                {
                    // Create the foldout group using a static factory method, passing property copy and attribute details.
                    foldoutGroup = OM_FoldoutGroup.CreateSettingsGroup(childProperty.Copy(), startGroup.GroupName, startGroup.GroupColor);
                    // Add the new foldout group to the UI Root.
                    Root.Add(foldoutGroup);
                    // Store the reference to the created group.
                    FoldoutGroups.Add(foldoutGroup);
                }

                // If the [OM_HideInInspector] attribute was found (and didn't have HandleOtherAttributes=false),
                // skip creating a PropertyField for this property.
                if (hideInInspector != null) continue;

                // --- Property Field Creation ---

                // Create a standard PropertyField for the current child property.
                var propertyField = new PropertyField(childProperty);
                // Register a callback to be notified when the value of this property changes in the UI.
                propertyField.RegisterValueChangeCallback(e =>
                {
                    // Call the virtual method to allow derived drawers to react to changes.
                    OnPropertyFieldChanged(e.changedProperty.name);
                    // Mark the owning SerializedObject as dirty to ensure changes are saved.
                    // Note: This might be redundant if the PropertyField binding handles it, but often necessary for custom logic.
                    e.changedProperty.serializedObject.ApplyModifiedProperties();
                });

                // Store references to the created PropertyField.
                PropertyFields.Add(propertyField);
                PropertyFieldDictionary[childProperty.name] = propertyField; // Map name to field

                // Ensure the PropertyField is bound to the SerializedObject for automatic updates.
                propertyField.Bind(childProperty.serializedObject);

                // Add the PropertyField to the UI: either directly to the Root or inside the current foldout group.
                GetParent(foldoutGroup).Add(propertyField);
            }

            // Return the fully constructed root VisualElement.
            return Root;
        }

        /// <summary>
        /// Helper method to determine the correct parent VisualElement for adding a PropertyField.
        /// If a foldout group is currently active, returns its content container; otherwise, returns the Root.
        /// </summary>
        /// <param name="currentFoldoutGroup">The currently active foldout group (can be null).</param>
        /// <returns>The VisualElement to which the next PropertyField should be added.</returns>
        private VisualElement GetParent(OM_FoldoutGroup currentFoldoutGroup)
        {
            // If currentFoldoutGroup is not null, return its Content area, otherwise return the main Root.
            return currentFoldoutGroup != null ? currentFoldoutGroup.Content : Root;
        }

        /// <summary>
        /// Gets the PropertyField element associated with the specified property name.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The corresponding PropertyField, or null if not found.</returns>
        protected PropertyField GetPropertyField(string propertyName)
        {
            // Try to retrieve the PropertyField from the dictionary.
            PropertyFieldDictionary.TryGetValue(propertyName, out var field);
            return field; // Returns null if key not found
        }

        /// <summary>
        /// Gets the SerializedProperty for a child property with the specified name,
        /// relative to the main Property being drawn.
        /// </summary>
        /// <param name="propertyName">The name of the child property.</param>
        /// <returns>The SerializedProperty, or null if not found.</returns>
        protected SerializedProperty GetProperty(string propertyName)
        {
            // Find the child property relative to the main property associated with this drawer.
            return Property.FindPropertyRelative(propertyName);
        }

        /// <summary>
        /// Virtual method called when the value of a property field changes in the UI.
        /// Derived classes can override this to implement custom logic that reacts to specific property changes.
        /// </summary>
        /// <param name="propertyName">The name of the property whose value changed.</param>
        protected virtual void OnPropertyFieldChanged(string propertyName)
        {
            // Base implementation does nothing.
        }
    }
}