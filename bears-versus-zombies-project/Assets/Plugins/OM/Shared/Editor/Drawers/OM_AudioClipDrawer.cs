using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace OM.Editor.Drawers 
{
    /// <summary>
    /// Custom PropertyDrawer for the OM_AudioClip class using UIElements.
    /// This drawer renders all properties of OM_AudioClip and adds logic
    /// to hide/show the range fields (pitchRange, volumeRange) and id
    /// based on whether an AudioClip is assigned to the 'clip' field.
    /// </summary>
    [CustomPropertyDrawer(typeof(OM_AudioClip))] // Specifies that this drawer is for properties of type OM_AudioClip.
    public class OM_AudioClipDrawer : PropertyDrawer
    {
        /// <summary>
        /// A list to keep references to the PropertyField elements *other* than the 'clip' field.
        /// This allows easily toggling their visibility when the 'clip' field changes.
        /// </summary>
        private readonly List<PropertyField> _fields = new List<PropertyField>();

        /// <summary>
        /// Creates the visual element hierarchy for the property drawer.
        /// Called by Unity when the Inspector needs to draw the OM_AudioClip property.
        /// </summary>
        /// <param name="property">The SerializedProperty representing the OM_AudioClip instance being drawn.</param>
        /// <returns>The root VisualElement for this property drawer's UI.</returns>
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            // Clear the list of tracked fields for this specific instance of the drawer.
            _fields.Clear();

            // Create the main container element for this property.
            var root = new VisualElement();

            // Get the initial state: is an AudioClip currently assigned?
            bool hasClip = property.FindPropertyRelative("clip").objectReferenceValue != null;

            // Iterate through all direct child properties of the OM_AudioClip serialized property.
            // (e.g., "clip", "id", "pitchRange", "volumeRange").
            foreach (var childProperty in property.GetAllProperties()) // GetAllProperties is likely an extension method.
            {
                // Create a standard PropertyField for the current child property.
                var field = new PropertyField(childProperty);

                // Add the created field to the root container.
                root.Add(field);

                // --- Special Handling for the 'clip' field ---
                if (childProperty.name == "clip")
                {
                    // Register a callback that triggers whenever the value of the 'clip' property changes.
                    field.RegisterValueChangeCallback(evt =>
                    {
                        // Determine if a clip is now assigned after the change.
                        bool clipIsAssigned = evt.changedProperty.objectReferenceValue != null;

                        // Iterate through all the other tracked fields (id, ranges).
                        foreach (var propertyField in _fields)
                        {
                            // Set the display style based on whether a clip is assigned.
                            // If a clip is assigned, show the field (Flex); otherwise, hide it (None).
                            propertyField.style.display = clipIsAssigned ? DisplayStyle.Flex : DisplayStyle.None;
                            // Alternative using extension method: propertyField.SetDisplay(clipIsAssigned);
                        }
                    });
                }
                // --- Handling for other fields ---
                else
                {
                    // Add this field (id, pitchRange, volumeRange) to the list of fields to be tracked.
                    _fields.Add(field);
                    // Set the initial visibility based on whether a clip was assigned when the drawer was created.
                    field.style.display = hasClip ? DisplayStyle.Flex : DisplayStyle.None;
                     // Alternative using extension method: field.SetDisplay(hasClip);
                }
            }

            // Return the fully constructed root element to be displayed in the Inspector.
            return root;
        }
    }
}
