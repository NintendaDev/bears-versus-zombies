using System.Reflection;
using OM.Animora.Runtime;
using OM.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace OM.Animora.Editor
{
    /// <summary>
    /// Custom drawer for <see cref="AnimoraInterpolation{T}"/>, used to configure interpolation behavior in the inspector.
    /// Includes support for enabling/disabling, zero-to-one interpolation, and randomization toggles.
    /// </summary>
    [CustomPropertyDrawer(typeof(AnimoraInterpolation<>), true)]
    public class AnimoraInterpolationDrawer : PropertyDrawer
    {
        private PropertyField _fieldZero;
        private PropertyField _fieldRandomizeForEach;
        private OM_Switcher _randomizeForEachSwitcher;

        /// <summary>
        /// Builds the custom UI for displaying <see cref="AnimoraInterpolation{T}"/> fields in the inspector.
        /// </summary>
        /// <param name="property">The serialized interpolation property.</param>
        /// <returns>A <see cref="VisualElement"/> representing the full UI.</returns>
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var info = property.GetFieldInfo(true);
            if (info == null)
                return new Label("No Field Info");

            var interpolationAttribute = info.GetCustomAttribute<AnimoraInterpolationAttribute>();
            var enabledProperty = property.FindPropertyRelative("enabled");

            var foldout = OM_FoldoutGroup.CreateSettingsGroup(property.Copy(), property.displayName, "Interpolation");

            // Optional "enabled" switcher
            if (interpolationAttribute != null && interpolationAttribute.UseOptional)
            {
                var enablePropertyField = OM_Switcher.CreateSwitcher(null, enabledProperty, newValue =>
                {
                    foreach (var element in foldout.Content.Children())
                        element.SetEnabled(newValue);
                }).AddTo(foldout.HeaderRightContainer);

                enablePropertyField.SendToBack();
                enablePropertyField.RegisterCallback<ClickEvent>(e => e.StopImmediatePropagation());
            }

            var randomizeForEachProperty = property.FindPropertyRelative("randomizeForEach");

            // Interpolation type field and visibility control
            var interpolationTypeField = new PropertyField(property.FindPropertyRelative("interpolationType"));
            interpolationTypeField.RegisterValueChangeCallback(e =>
            {
                var type = (AnimoraInterpolationType)e.changedProperty.enumValueIndex;
                _fieldZero?.SetDisplay(type == AnimoraInterpolationType.FromZeroToOne);
            });
            foldout.AddToContent(interpolationTypeField);

            // Draw remaining fields dynamically
            foreach (var child in property.GetAllProperties())
            {
                switch (child.name)
                {
                    case "relative":
                    case "enabled":
                    case "interpolationType":
                        continue;

                    case "randomizeForEach":
                        _randomizeForEachSwitcher = OM_Switcher.CreateSwitcher("Randomize For Each", randomizeForEachProperty.Copy(), _ => { })
                            .AddTo(foldout.Content);
                        continue;

                    default:
                        var field = new PropertyField(child);
                        foldout.AddToContent(field);

                        if (child.name == "zero")
                            _fieldZero = field;
                        break;
                }
            }

            // Visibility control logic for "Randomize For Each" switcher
            foldout.Add(new IMGUIContainer(() =>
            {
                //var zeroRandom = property.FindPropertyRelative("zero").FindPropertyRelative("random").boolValue;
                //var oneRandom = property.FindPropertyRelative("one").FindPropertyRelative("random").boolValue;
                
                var zeroRandom = (AnimoraValueType)property.FindPropertyRelative("zero").FindPropertyRelative("valueType").enumValueIndex == AnimoraValueType.Random;
                var oneRandom = (AnimoraValueType)property.FindPropertyRelative("one").FindPropertyRelative("valueType").enumValueIndex == AnimoraValueType.Random;
                _randomizeForEachSwitcher?.SetDisplay(zeroRandom || oneRandom);
            }));

            return foldout;
        }
    }
}
