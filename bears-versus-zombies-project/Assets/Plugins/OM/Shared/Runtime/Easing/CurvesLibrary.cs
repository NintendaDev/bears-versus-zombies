using System.Collections.Generic;
using UnityEngine;

namespace OM
{
    /// <summary>
    /// Represents a single named animation curve.
    /// Used within the CurvesLibrary to store and categorize predefined curves.
    /// Marked as Serializable so it can be viewed and edited in the Unity Inspector
    /// when part of a list within a ScriptableObject like CurvesLibrary.
    /// </summary>
    [System.Serializable]
    public class CurveData
    {
        /// <summary>
        /// The user-defined name for this animation curve (e.g., "Fast Bounce", "Linear Fade").
        /// This field is serialized and editable in the Inspector.
        /// </summary>
        [SerializeField] private string name;

        /// <summary>
        /// The actual AnimationCurve data defining the curve's shape.
        /// This field is serialized and editable in the Inspector using Unity's curve editor.
        /// </summary>
        [SerializeField] private AnimationCurve curve;

        /// <summary>
        /// Gets or sets the name of this curve data entry.
        /// </summary>
        public string Name
        {
            get => name;
            set => name = value; // Allows renaming, e.g., in a custom editor.
        }

        /// <summary>
        /// Gets or sets the AnimationCurve associated with this entry.
        /// </summary>
        public AnimationCurve Curve
        {
            get => curve;
            set => curve = value; // Allows changing the curve data.
        }
    }

    /// <summary>
    /// A ScriptableObject acting as a central library for storing predefined AnimationCurves.
    /// It uses a Singleton pattern (`Instance`) for easy access from anywhere.
    /// Curves are categorized into Lerp, Bounce, and Custom lists.
    /// Provides methods to retrieve curves by name and add new custom curves (Editor-only).
    /// </summary>
    [CreateAssetMenu(fileName = "CurvesLibrary", menuName = "OM/CurvesLibrary")] // Allows creating instances via Assets/Create menu.
    public class CurvesLibrary : ScriptableObject
    {
        // --- Singleton Implementation ---
        private static CurvesLibrary _instance;
        /// <summary>
        /// Provides global access to the single CurvesLibrary instance.
        /// Loads the "CurvesLibrary" asset from the Resources folder if not already loaded.
        /// Returns null if the asset cannot be found in Resources.
        /// </summary>
        public static CurvesLibrary Instance
        {
            get
            {
                if (_instance == null)
                {
                    // Attempt to load the asset from any "Resources" folder.
                    _instance = Resources.Load<CurvesLibrary>("CurvesLibrary");
                    if (_instance == null)
                    {
                        Debug.LogError("CurvesLibrary asset not found in Resources folder. Please create one via Assets/Create/OM/CurvesLibrary and place it in a Resources folder.");
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// List storing curves typically used for standard interpolation or fading (e.g., linear, ease-in, ease-out).
        /// Editable in the Inspector.
        /// </summary>
        [SerializeField] private List<CurveData> lerpCurves = new List<CurveData>();

        /// <summary>
        /// List storing curves often used for bounce or overshoot effects.
        /// Editable in the Inspector.
        /// </summary>
        [SerializeField] private List<CurveData> bounceCurves = new List<CurveData>();

        /// <summary>
        /// List storing custom user-defined curves, often added programmatically via AddCurve or manually in the Inspector.
        /// Editable in the Inspector.
        /// </summary>
        [SerializeField] private List<CurveData> customCurves = new List<CurveData>();

        /// <summary>
        /// Gets the list of Lerp-type curves.
        /// </summary>
        public List<CurveData> LerpCurves => lerpCurves;

        /// <summary>
        /// Gets the list of Bounce-type curves.
        /// </summary>
        public List<CurveData> BounceCurves => bounceCurves;

        /// <summary>
        /// Gets the list of Custom-type curves.
        /// </summary>
        public List<CurveData> CustomCurves => customCurves;

        /// <summary>
        /// Retrieves an AnimationCurve by its exact name.
        /// Searches through LerpCurves, BounceCurves, and CustomCurves in that order.
        /// </summary>
        /// <param name="curveName">The case-sensitive name of the curve to find.</param>
        /// <returns>The found AnimationCurve, or a default linear EaseInOut curve if the name is not found in any list.</returns>
        public AnimationCurve GetCurveByName(string curveName)
        {
            // Search Lerp curves first
            foreach (var curveData in lerpCurves)
            {
                if (curveData.Name == curveName) return curveData.Curve;
            }
            // Search Bounce curves next
            foreach (var curveData in bounceCurves)
            {
                if (curveData.Name == curveName) return curveData.Curve;
            }
            // Search Custom curves last
            foreach (var curveData in customCurves)
            {
                if (curveData.Name == curveName) return curveData.Curve;
            }

            // Fallback if not found
            Debug.LogWarning($"Curve with name '{curveName}' not found in CurvesLibrary. Returning default EaseInOut curve.");
            return AnimationCurve.EaseInOut(0, 0, 1, 1); // Default fallback curve.
        }

        /// <summary>
        /// Adds a new curve to the `customCurves` list.
        /// This method is intended for use within the Unity Editor (e.g., from a custom property drawer).
        /// It handles Undo recording, marking the asset as dirty, saving assets, and selecting/pinging the library asset.
        /// </summary>
        /// <param name="curveName">The base name for the new curve (a count suffix will be added).</param>
        /// <param name="curve">The AnimationCurve data to add.</param>
        public void AddCurve(string curveName, AnimationCurve curve)
        {
#if UNITY_EDITOR
            // Record the change for Undo/Redo functionality.
            UnityEditor.Undo.RecordObject(this, "Add Curve to Library");
#endif
            // Add the new curve data to the custom list, appending the current count to the name for uniqueness.
            customCurves.Add(new CurveData { Name = curveName + $" ({customCurves.Count})", Curve = curve });

#if UNITY_EDITOR
            // Mark this ScriptableObject as dirty so Unity knows it needs to be saved.
            UnityEditor.EditorUtility.SetDirty(this);
            // Save the changes to the asset database.
            UnityEditor.AssetDatabase.SaveAssets();
            // Optionally select and ping the asset in the Project window for user feedback.
            UnityEditor.Selection.activeObject = this;
            UnityEditor.EditorGUIUtility.PingObject(this);
#endif
        }
    }
}