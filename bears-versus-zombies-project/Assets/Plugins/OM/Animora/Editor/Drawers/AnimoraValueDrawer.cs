using System;
using System.Collections.Generic;
using OM.Animora.Runtime;
using OM.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.Animora.Editor
{
    /// <summary>
    /// Custom drawer for <see cref="AnimoraValue{T}"/>.  
    /// Displays either a single value or two random range fields depending on the `random` toggle.
    /// </summary>
    [CustomPropertyDrawer(typeof(AnimoraValue<>), true)]
    public class AnimoraValueDrawer : PropertyDrawer
    {
        SerializedProperty valueTypeProperty, targetTransformProperty, vector3TypeProperty, valueProperty, randomValue1Property, randomValue2Property;
        SerializedProperty currentProperty;
        
        /// <summary>
        /// Builds the inspector UI for <see cref="AnimoraValue{T}"/>.
        /// </summary>
        /// <param name="property">The serialized property to render.</param>
        /// <returns>The root <see cref="VisualElement"/> container.</returns>
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            currentProperty = property.Copy();
            var root = new VisualElement();

            root.style.paddingBottom = 1;
            root.style.paddingTop = 1;
            root.style.paddingLeft = 1;
            root.style.paddingRight = 1;
            
            root.style.borderBottomWidth = 1;
            root.style.borderTopWidth = 1;
            root.style.borderLeftWidth = 1;
            root.style.borderRightWidth = 1;
            
            root.style.borderBottomColor = new Color(0.5f, 0.5f, 0.5f, 1);
            root.style.borderTopColor = new Color(0.5f, 0.5f, 0.5f, 1);
            root.style.borderLeftColor = new Color(0.5f, 0.5f, 0.5f, 1);
            root.style.borderRightColor = new Color(0.5f, 0.5f, 0.5f, 1);
            
            root.style.marginBottom = 2;
            root.style.marginTop = 2;
            
            root.style.borderBottomLeftRadius = 4;
            root.style.borderBottomRightRadius = 4;
            root.style.borderTopLeftRadius = 4;
            root.style.borderTopRightRadius = 4;
            
            // Create container with two sections: value fields and random toggle
            var container = new OM_Split2().SetFlexDirection(FlexDirection.Row);
            root.Add(container);
            container.AddToClassList("jt-value");
            container.Container2.style.width = 30;
            container.Container1.style.flexGrow = 1;

            // Find sub-properties
            valueTypeProperty = property.FindPropertyRelative("valueType");
            targetTransformProperty = property.FindPropertyRelative("targetTransform");
            vector3TypeProperty = property.FindPropertyRelative("vector3Type");
            valueProperty = property.FindPropertyRelative("value");
            randomValue1Property = property.FindPropertyRelative("randomValue1");
            randomValue2Property = property.FindPropertyRelative("randomValue2");

            
            
            var types = new List<string>();
            foreach (var name in Enum.GetNames(typeof(AnimoraValueType)))
            {
                types.Add(name);
            }

            var animoraValue = property.GetValueCustom(true);
            if(animoraValue is not AnimoraValue<Vector3> valueBase)
            {
                types.Remove(nameof(AnimoraValueType.Target));
                if (valueTypeProperty.enumValueIndex == types.IndexOf(nameof(AnimoraValueType.Target)))
                {
                    valueTypeProperty.enumValueIndex = 0;
                    valueTypeProperty.serializedObject.ApplyModifiedProperties();
                }
            }

            var valueField = new PropertyField(valueProperty)
            {
                label = property.displayName
            };
            var randomValue1Field = new PropertyField(randomValue1Property)
            {
                label = property.displayName + " 1"
            };
            var randomValue2Field = new PropertyField(randomValue2Property)
            {
                label = property.displayName + " 2"
            };
            var targetTransformField = new PropertyField(targetTransformProperty);
            targetTransformField.Bind(property.serializedObject);

            var vector3TypeField = new PropertyField(vector3TypeProperty);
            vector3TypeField.Bind(property.serializedObject);
            
            // Add fields to layout
            container.AddToContainer1(valueField);
            container.AddToContainer1(randomValue1Field);
            container.AddToContainer1(randomValue2Field);
            container.AddToContainer1(targetTransformField);
            container.AddToContainer1(vector3TypeField);

            var dropdownField = new DropdownField(types, valueTypeProperty.enumValueIndex);
            dropdownField.label = property.displayName + " Type";
            dropdownField.RegisterValueChangedCallback(evt =>
            {
                valueTypeProperty.enumValueIndex = types.IndexOf(evt.newValue);
                valueTypeProperty.serializedObject.ApplyModifiedProperties();

                UpdateDisplay();
            });
            root.Insert(0,dropdownField);
            
            // Initial display setup
            UpdateDisplay();

            container.RegisterCallback<AttachToPanelEvent>(e =>
            {
                Undo.undoRedoPerformed += UpdateDisplay;
            });
            
            container.RegisterCallback<DetachFromPanelEvent>(e =>
            {
                Undo.undoRedoPerformed -= UpdateDisplay;
            });
            
            return root;

            // Local function to toggle visibility
            void UpdateDisplay()
            {
                if(currentProperty.IsSerializedPropertyValid() == false) return;
                valueTypeProperty = currentProperty.FindPropertyRelative("valueType");
                
                var valueType = (AnimoraValueType)valueTypeProperty.enumValueIndex;
                dropdownField.value = valueType.ToString();
                
                switch (valueType)
                {
                    case AnimoraValueType.Fixed:
                        valueField.SetDisplay(true);
                        randomValue1Field.SetDisplay(false);
                        randomValue2Field.SetDisplay(false);
                        targetTransformField.SetDisplay(false);
                        vector3TypeField.SetDisplay(false);
                        break;
                    case AnimoraValueType.Random:
                        valueField.SetDisplay(false);
                        randomValue1Field.SetDisplay(true);
                        randomValue2Field.SetDisplay(true);
                        targetTransformField.SetDisplay(false);
                        vector3TypeField.SetDisplay(false);
                        break;
                    case AnimoraValueType.Target:
                        valueField.SetDisplay(false);
                        randomValue1Field.SetDisplay(false);
                        randomValue2Field.SetDisplay(false);
                        targetTransformField.SetDisplay(true);
                        vector3TypeField.SetDisplay(true);
                        break;
                }
            }
        }

    }
}
