using UnityEditor; 
using UnityEngine;

namespace OM.TimelineCreator.Editor
{
    /// <summary>
    /// Defines ScriptableObject-based settings for the OM Timeline Creator editor.
    /// Allows users to configure editor behavior like snapping.
    /// Uses a Singleton pattern to provide easy access to the settings instance.
    /// </summary>
    [CreateAssetMenu(fileName = "OM_TimelineSettings", menuName = "OM/Timeline Creator Settings")] // Make it creatable from the Assets menu
    public class OM_TimelineSettings : ScriptableObject
    {
        #region Singleton

        /// <summary>
        /// Backing field for the singleton instance.
        /// </summary>
        private static OM_TimelineSettings _instance;

        /// <summary>
        /// Gets the singleton instance of the OM_TimelineSettings.
        /// If the instance doesn't exist, it attempts to load it from Resources.
        /// If it's still not found, it creates a new instance and saves it as an asset.
        /// </summary>
        public static OM_TimelineSettings Instance
        {
            get
            {
                // If instance already exists, return it
                if (_instance != null) return _instance;

                // Try to load from Resources folder
                _instance = Resources.Load<OM_TimelineSettings>("OM_TimelineSettings");

                // If loaded successfully, return it
                if (_instance != null) return _instance;

                // --- Instance not found, create a new one ---
                Debug.LogWarning("OM_TimelineSettings asset not found in Resources. Creating a new one.");
                _instance = CreateInstance<OM_TimelineSettings>();

                // Define the path where the new asset will be saved
                const string assetPath = "Assets/Plugins/OM/TimelineCreator/Editor/Resources/OM_TimelineSettings.asset";

                // Ensure the directory exists (optional, CreateAsset handles it but good practice)
                string directoryPath = System.IO.Path.GetDirectoryName(assetPath);
                if (!System.IO.Directory.Exists(directoryPath))
                {
                    System.IO.Directory.CreateDirectory(directoryPath);
                }

                // Create the asset file in the project
                AssetDatabase.CreateAsset(_instance, assetPath);
                // Save the changes to the asset database
                AssetDatabase.SaveAssets();
                 // Refresh asset database to make sure it's visible
                AssetDatabase.Refresh();

                Debug.Log($"Created OM_TimelineSettings asset at: {assetPath}");

                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// Serialized field indicating whether snapping functionality is enabled in the timeline editor.
        /// </summary>
        [SerializeField]
        [Tooltip("Enable snapping of clips to each other and timeline boundaries during dragging.")]
        private bool useSnapping = true;

        /// <summary>
        /// Serialized field defining the snapping sensitivity distance in screen pixels.
        /// Clips will snap if their edges are within this distance of a snap point.
        /// </summary>
        [SerializeField, Range(0, 30f)]
        [Tooltip("How close (in pixels) edges need to be to snap during dragging.")]
        private float snappingValueInPixel = 5;

        /// <summary>
        /// Public property to get or set whether snapping is enabled.
        /// </summary>
        public bool UseSnapping
        {
            get => useSnapping;
            set {
                if (useSnapping != value) {
                     useSnapping = value;
                     EditorUtility.SetDirty(this); // Mark asset dirty when changed via code
                }
            }
        }

        /// <summary>
        /// Public property to get or set the snapping sensitivity value in pixels.
        /// </summary>
        public float SnappingValue
        {
            get => snappingValueInPixel;
             set {
                if (snappingValueInPixel != value) {
                     snappingValueInPixel = value;
                     EditorUtility.SetDirty(this); // Mark asset dirty when changed via code
                }
            }
        }
    }
}