using OM.Animora.Runtime;
using OM.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace OM.Animora.Editor
{
    /// <summary>
    /// Custom drawer for <see cref="AnimoraValueGroup{T}"/>.
    /// Displays a foldout group with a toggle to enable/disable the group and shows all contained values.
    /// </summary>
    [CustomPropertyDrawer(typeof(AnimoraValueGroup<>), true)]
    public class AnimoraValueGroupDrawer : PropertyDrawer
    {
        /// <summary>
        /// Builds the custom UI for <see cref="AnimoraValueGroup{T}"/>.
        /// </summary>
        /// <param name="property">The serialized property representing the group.</param>
        /// <returns>A <see cref="VisualElement"/> representing the value group layout.</returns>
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var settingsGroup = OM_FoldoutGroup.CreateSettingsGroup(property.Copy(), property.displayName, "ValueGroup");

            // Enable/disable switcher
            var enabledProp = property.FindPropertyRelative("enabled");
            var switcher = OM_Switcher.CreateSwitcher("", enabledProp.Copy(), isEnabled =>
            {
                settingsGroup.Content.SetEnabled(isEnabled);
            });

            switcher.SendToBack();
            switcher.RegisterCallback<ClickEvent>(e => e.StopImmediatePropagation());
            settingsGroup.HeaderRightContainer.Add(switcher);

            // Add all other properties as fields
            foreach (var childProperty in property.GetAllProperties())
            {
                if (childProperty.name == "enabled") continue;

                var propertyField = new PropertyField(childProperty);
                propertyField.Bind(property.serializedObject);
                settingsGroup.AddToContent(propertyField);
            }

            return settingsGroup;
        }
    }
}