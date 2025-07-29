using System;
using OM.Animora.Runtime;
using OM.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace OM.Animora.Editor
{
    /// <summary>
    /// Custom drawer for <see cref="AnimoraTargets{T}"/>.  
    /// Dynamically shows either direct target list or reference ID depending on the selected <see cref="AnimoraTargetType"/>.
    /// Displays an error icon if the target configuration is invalid.
    /// </summary>
    [CustomPropertyDrawer(typeof(AnimoraTargets<>), true)]
    public class AnimoraTargetsDrawer : PropertyDrawer
    {
        private PropertyField _directTargetsField, _referenceIdField;

        /// <summary>
        /// Builds the custom UI for <see cref="AnimoraTargets{T}"/> in the Unity inspector.
        /// </summary>
        /// <param name="property">The serialized property being drawn.</param>
        /// <returns>A configured <see cref="VisualElement"/>.</returns>
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var foldoutGroup = OM_FoldoutGroup.CreateSettingsGroup(property.Copy(), property.displayName, "Target");

            // Error icon setup
            var errorIcon = new VisualElement();
            errorIcon.SetBackgroundFromIconContent("console.erroricon.sml");
            errorIcon.style.width = 20;
            errorIcon.style.height = 20;
            errorIcon.tooltip = "Error";
            errorIcon.SetDisplay(false);
            foldoutGroup.AddToRightHeaderContainer(errorIcon);

            var animoraTargetBase = property.GetValueCustom(true) as AnimoraTargetBase;

            // Iterate over all sub-properties
            foreach (var childProperty in property.GetAllProperties())
            {
                var propertyField = new PropertyField(childProperty);
                propertyField.Bind(property.serializedObject);

                switch (childProperty.name)
                {
                    case "directTargets":
                        _directTargetsField = propertyField;
                        foldoutGroup.AddToContent(propertyField);
                        break;

                    case "referenceId":
                        _referenceIdField = propertyField;
                        foldoutGroup.AddToContent(propertyField);
                        break;

                    case "targetType":
                        propertyField.label = "";
                        propertyField.RegisterValueChangeCallback(evt =>
                        {
                            var targetType = (AnimoraTargetType)evt.changedProperty.enumValueIndex;
                            switch (targetType)
                            {
                                case AnimoraTargetType.Id:
                                    _referenceIdField.SetDisplay(true);
                                    _directTargetsField.SetDisplay(false);
                                    break;
                                case AnimoraTargetType.Direct:
                                    _referenceIdField.SetDisplay(false);
                                    _directTargetsField.SetDisplay(true);
                                    break;
                            }
                        });

                        foldoutGroup.AddToRightHeaderContainer(propertyField);
                        break;
                }
            }

            // Display error icon if validation fails
            foldoutGroup.Add(new IMGUIContainer(() =>
            {
                if (animoraTargetBase == null) return;

                if (animoraTargetBase.HasErrors(out var error))
                {
                    errorIcon.SetDisplay(true);
                    errorIcon.tooltip = error;
                }
                else
                {
                    errorIcon.SetDisplay(false);
                    errorIcon.tooltip = "";
                }
            }));

            return foldoutGroup;
        }
    }
}
