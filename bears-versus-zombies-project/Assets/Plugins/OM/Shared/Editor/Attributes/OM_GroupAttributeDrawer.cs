using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace OM.Editor
{
    /// <summary>
    /// Custom property drawer for fields marked with <see cref="OM_GroupAttribute"/>.
    /// Organizes serialized properties into a foldout group with optional toggle control.
    /// </summary>
    [CustomPropertyDrawer(typeof(OM_GroupAttribute), true)]
    public class OM_GroupAttributeDrawer : PropertyDrawer
    {
        /// <summary>
        /// Creates the custom UI for properties marked with <see cref="OM_GroupAttribute"/>.
        /// </summary>
        /// <param name="property">The serialized property to create a UI for.</param>
        /// <returns>A <see cref="VisualElement"/> representing the custom property drawer.</returns>
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var groupAttribute = this.attribute as OM_GroupAttribute;
            if (groupAttribute == null) return new Label("No Group Attribute");

            // Create foldout group UI container
            var foldout = OM_FoldoutGroup.CreateSettingsGroup(
                property.Copy(),
                groupAttribute.GroupName,
                groupAttribute.GroupColor
            );

            // If a toggle name is specified, add a switcher that controls foldout content's enabled state
            if (groupAttribute.ToggleName != null)
            {
                var toggleProperty = property.FindPropertyRelative(groupAttribute.ToggleName);

                var switcher = OM_Switcher.CreateSwitcher("", toggleProperty.Copy(), e =>
                {
                    foldout.Content.SetEnabled(e);
                });

                switcher.SendToBack();
                switcher.RegisterCallback<ClickEvent>(e => { e.StopImmediatePropagation(); });

                foldout.HeaderRightContainer.Add(switcher);
            }

            // Add child properties to the foldout, excluding the toggle property itself
            foreach (var childProperty in property.GetAllProperties())
            {
                if (childProperty.name == groupAttribute.ToggleName) continue;

                var propertyField = new PropertyField(childProperty);
                propertyField.Bind(property.serializedObject);
                foldout.AddToContent(propertyField);
            }

            return foldout;
        }
    }
}
