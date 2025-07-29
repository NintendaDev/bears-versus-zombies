using UnityEditor;
using UnityEngine;

namespace OM.Editor
{
    /// <summary>
    /// Custom PropertyDrawer for the EaseData struct.
    /// Provides a specialized Inspector GUI that allows selecting between standard Easing Functions
    /// and custom Animation Curves via a dropdown menu, displaying the relevant field accordingly.
    /// Relies on EasingHelper and CurvesLibrary for populating the dropdown menu.
    /// </summary>
    [CustomPropertyDrawer(typeof(EaseData), true)] // Draw for EaseData and derived classes (if any).
    public class EasingDataPropertyDrawer : PropertyDrawer
    {
        /// <summary>
        /// Override to render the custom GUI for the EaseData property.
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the property GUI.</param>
        /// <param name="property">The SerializedProperty representing the EaseData instance.</param>
        /// <param name="label">The label of this property (usually the field name).</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Get references to the child properties within the EaseData struct.
            var easingDataTypeProperty = property.FindPropertyRelative("easeDataType"); // Enum (Ease or AnimationCurve)
            var easingTypeProperty = property.FindPropertyRelative("easeType");         // EasingFunction enum
            var animationCurveProperty = property.FindPropertyRelative("animationCurve"); // AnimationCurve data

            // --- Calculate Rects ---
            var propertyRect = new Rect(position);
            propertyRect.height = EditorGUIUtility.singleLineHeight; // Ensure single line height for the main field part.
            propertyRect.width -= 20; // Make space for the dropdown button.

            var buttonRect = new Rect(propertyRect)
            {
                x = propertyRect.xMax, // Position button immediately after the property field.
                width = 20,            // Standard width for small buttons/icons.
                height = EditorGUIUtility.singleLineHeight,
            };

            // --- Draw Button ---
            // Style for button hover effect (currently using grayTexture which might need definition or replacement).
            var buttonStyle = new GUIStyle(GUI.skin.button) // Start with default button style
            {
                // Define hover state visual appearance.
                 hover = new GUIStyleState { background = Texture2D.grayTexture } // Basic gray background on hover. Consider a more subtle/themed texture.
                ,
                 // Remove padding to make icon fill the button better
                // padding = new RectOffset(0, 0, 0, 0)
            };
            // Use a Unity built-in icon for the dropdown arrow.
            //GUIContent buttonIcon = new GUIContent(EditorGUIUtility.IconContent("d_Preset.Context@2x")); // Icon often used for dropdowns/pickers.

            // Draw the button and check if it was clicked.
            if (GUI.Button(buttonRect,new GUIContent("*"), buttonStyle))
            {
                // If clicked, open the generic menu for selecting easing types/curves.
                OpenGenericMenu(easingTypeProperty, easingDataTypeProperty, property, animationCurveProperty);
            }

            // --- Draw Main Property Field ---
            // Determine which property to display based on the current easeDataType.
            SerializedProperty propertyToDraw = easingDataTypeProperty.intValue == (int)EaseDataType.Ease
                ? easingTypeProperty        // Show EasingFunction enum if type is Ease.
                : animationCurveProperty;   // Show AnimationCurve field if type is AnimationCurve.

            // Draw the appropriate property field using the original label.
            EditorGUI.PropertyField(propertyRect, propertyToDraw, new GUIContent(label)); // Pass label to ensure field name is shown.
        }

        /// <summary>
        /// Creates and displays a GenericMenu (context menu) populated with options
        /// to select standard easing functions, predefined animation curves from CurvesLibrary,
        /// or switch to a custom animation curve.
        /// </summary>
        /// <param name="easingTypeProperty">SerializedProperty for the 'easeType' field (EasingFunction).</param>
        /// <param name="easingDataTypeProperty">SerializedProperty for the 'easeDataType' field (Ease/AnimationCurve enum).</param>
        /// <param name="property">The parent SerializedProperty representing the entire EaseData struct.</param>
        /// <param name="animationCurveProperty">SerializedProperty for the 'animationCurve' field.</param>
        private void OpenGenericMenu(SerializedProperty easingTypeProperty, SerializedProperty easingDataTypeProperty,
                                     SerializedProperty property, SerializedProperty animationCurveProperty)
        {
            // Record the current state of the target object for Undo functionality.
            Undo.RecordObject(property.serializedObject.targetObject, "Switch Ease Type");

            var genericMenu = new GenericMenu();

            // Determine the current selection state.
            var currentDataType = (EaseDataType)easingDataTypeProperty.enumValueIndex;
            var isEaseSelected = currentDataType == EaseDataType.Ease;
            var isCurveSelected = currentDataType == EaseDataType.AnimationCurve;
            var currentEaseFunction = (EasingFunction)easingTypeProperty.enumValueIndex; // Store current ease func for checkmark logic

            // --- Add Standard Easing Functions (Lerp/Idle Category) ---
            // Use EasingHelper to iterate through available 'Idle' (non-PingPong) easing functions.
            foreach (var functionIdle in EasingHelper.LoopThroughEasingFunctionsIdle())
            {
                var functionEnum = functionIdle.CastToEasingFunction(); // Cast Idle enum to main EasingFunction enum
                // Add item: "Lerp/FunctionName", checkmark if it's the currently selected Ease function.
                genericMenu.AddItem(new GUIContent("Lerp/" + functionIdle.ToString()),
                                    isEaseSelected && currentEaseFunction == functionEnum, // Checkmark logic
                                    () => // Action executed when this item is selected:
                                    {
                                        easingTypeProperty.enumValueIndex = (int)functionEnum; // Set the ease function.
                                        easingDataTypeProperty.intValue = (int)EaseDataType.Ease; // Set data type to Ease.
                                        property.serializedObject.ApplyModifiedProperties(); // Apply changes to the serialized object.
                                    });
            }

            // --- Add PingPong Easing Functions ---
             genericMenu.AddSeparator(""); // Visual separator in the menu.
            // Use EasingHelper to iterate through available PingPong easing functions.
            foreach (var functionPingPong in EasingHelper.LoopThroughEasingFunctionsPingPong())
            {
                var functionEnum = functionPingPong.CastToEasingFunction(); // Cast PingPong enum to main EasingFunction enum
                 // Add item: "PingPong/FunctionName", checkmark if it's the currently selected Ease function.
                genericMenu.AddItem(new GUIContent("PingPong/" + functionPingPong.ToString()),
                                    isEaseSelected && currentEaseFunction == functionEnum, // Checkmark logic
                                    () => // Action executed when this item is selected:
                                    {
                                        easingTypeProperty.enumValueIndex = (int)functionEnum; // Set the ease function.
                                        easingDataTypeProperty.intValue = (int)EaseDataType.Ease; // Set data type to Ease.
                                        property.serializedObject.ApplyModifiedProperties(); // Apply changes.
                                    });
            }

            // --- Add Predefined Animation Curves from Library ---
            genericMenu.AddSeparator(""); // Visual separator.

            // Access the singleton instance of the CurvesLibrary.
            var curvesLibrary = CurvesLibrary.Instance;
            if (curvesLibrary != null) // Check if the library asset exists.
            {
                // Add Lerp Curves from Library
                foreach (var curve in curvesLibrary.LerpCurves)
                {
                     // Checkmark logic: Is the type AnimationCurve AND is the current curve value equal to this library curve?
                    bool isCurrentCurve = isCurveSelected && AnimationCurveClamper.AreCurvesEqual(animationCurveProperty.animationCurveValue, curve.Curve);
                    genericMenu.AddItem(new GUIContent("Curve Lerp/" + curve.Name),
                                        isCurrentCurve, // Checkmark logic
                                        () => // Action executed when selected:
                                        {
                                            animationCurveProperty.animationCurveValue = curve.Curve; // Set the curve value.
                                            easingDataTypeProperty.intValue = (int)EaseDataType.AnimationCurve; // Set data type to AnimationCurve.
                                            property.serializedObject.ApplyModifiedProperties(); // Apply changes.
                                        });
                }

                // Add Bounce Curves from Library
                foreach (var curve in curvesLibrary.BounceCurves)
                {
                    bool isCurrentCurve = isCurveSelected && AnimationCurveClamper.AreCurvesEqual(animationCurveProperty.animationCurveValue, curve.Curve);
                    genericMenu.AddItem(new GUIContent("Curve Bounce/" + curve.Name),
                                        isCurrentCurve, // Checkmark logic
                                        () => // Action executed when selected:
                                        {
                                            animationCurveProperty.animationCurveValue = curve.Curve;
                                            easingDataTypeProperty.intValue = (int)EaseDataType.AnimationCurve;
                                            property.serializedObject.ApplyModifiedProperties();
                                        });
                }

                // Add Custom Curves from Library (if any)
                if (curvesLibrary.CustomCurves.Count > 0)
                {
                    genericMenu.AddSeparator("Curve Custom/"); // Separator specific to custom curves category
                    foreach (var curve in curvesLibrary.CustomCurves)
                    {
                        bool isCurrentCurve = isCurveSelected && AnimationCurveClamper.AreCurvesEqual(animationCurveProperty.animationCurveValue, curve.Curve);
                        genericMenu.AddItem(new GUIContent("Curve Custom/" + curve.Name),
                                            isCurrentCurve, // Checkmark logic
                                            () => // Action executed when selected:
                                            {
                                                animationCurveProperty.animationCurveValue = curve.Curve;
                                                easingDataTypeProperty.intValue = (int)EaseDataType.AnimationCurve;
                                                property.serializedObject.ApplyModifiedProperties();
                                            });
                    }
                }
            }
            else
            {
                 genericMenu.AddDisabledItem(new GUIContent("CurvesLibrary not found in Resources"));
            }


            // --- Add General Animation Curve Options ---
            genericMenu.AddSeparator("");

            // Option to switch to Custom Animation Curve mode.
            genericMenu.AddItem(new GUIContent("Custom Animation Curve"),
                                isCurveSelected, // Checkmark if currently in AnimationCurve mode.
                                () =>
                                {
                                    easingDataTypeProperty.intValue = (int)EaseDataType.AnimationCurve;
                                    // Optionally set a default curve if switching to custom.
                                    // animationCurveProperty.animationCurveValue = AnimationCurve.Linear(0, 0, 1, 1);
                                    property.serializedObject.ApplyModifiedProperties();
                                });

            genericMenu.AddSeparator("");

            // Option to save the currently edited Animation Curve to the library.
            if (isCurveSelected && curvesLibrary != null) // Only enable if in AnimationCurve mode and library exists.
            {
                genericMenu.AddItem(new GUIContent("Save Current Curve to Library"),
                                    false, // Never checkmarked by default.
                                    () =>
                                    {
                                        // Prompt library to add the current curve.
                                        curvesLibrary.AddCurve("New Custom Curve", animationCurveProperty.animationCurveValue);
                                        // No need to ApplyModifiedProperties here as AddCurve handles saving/refreshing.
                                    });
            }
            else // Disable the save option otherwise.
            {
                genericMenu.AddDisabledItem(new GUIContent("Save Current Curve to Library"));
            }

            // Option to open the CurvesLibrary asset in the Inspector.
             if (curvesLibrary != null)
             {
                genericMenu.AddItem(new GUIContent("Open CurveLibrary Asset"),
                                    false,
                                    () =>
                                    {
                                        Selection.activeObject = curvesLibrary; // Select the library asset.
                                        EditorGUIUtility.PingObject(curvesLibrary); // Highlight it in the Project window.
                                    });
            }
             else
             {
                 genericMenu.AddDisabledItem(new GUIContent("Open CurveLibrary Asset"));
             }


            // Display the menu at the current mouse position.
            genericMenu.ShowAsContext();
        }

        // Optional Helper for comparing AnimationCurves (Unity doesn't provide a built-in reliable equality check)
        // This is a basic implementation; more robust checking might be needed.
        private static class AnimationCurveClamper
        {
            public static bool AreCurvesEqual(AnimationCurve curve1, AnimationCurve curve2)
            {
                if (curve1 == null && curve2 == null) return true;
                if (curve1 == null || curve2 == null) return false;
                if (curve1.keys.Length != curve2.keys.Length) return false;
                if (curve1.preWrapMode != curve2.preWrapMode || curve1.postWrapMode != curve2.postWrapMode) return false;

                for (int i = 0; i < curve1.keys.Length; i++)
                {
                    Keyframe key1 = curve1.keys[i];
                    Keyframe key2 = curve2.keys[i];
                    // Compare keyframe properties with a small tolerance for floating-point inaccuracies.
                    if (Mathf.Abs(key1.time - key2.time) > 0.0001f ||
                        Mathf.Abs(key1.value - key2.value) > 0.0001f ||
                        Mathf.Abs(key1.inTangent - key2.inTangent) > 0.001f || // Tangents might need larger tolerance
                        Mathf.Abs(key1.outTangent - key2.outTangent) > 0.001f ||
                        key1.weightedMode != key2.weightedMode || // Weighted mode must match exactly
                        (key1.weightedMode != WeightedMode.None && // Only compare weights if weighted mode is active
                            (Mathf.Abs(key1.inWeight - key2.inWeight) > 0.001f ||
                             Mathf.Abs(key1.outWeight - key2.outWeight) > 0.001f)))
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}
