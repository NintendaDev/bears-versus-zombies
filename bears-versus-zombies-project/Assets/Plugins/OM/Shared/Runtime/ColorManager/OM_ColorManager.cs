using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OM
{
    /// <summary>
    /// Represents a single named color item within a color palette.
    /// Allows identifying colors by a string key.
    /// Implements IEquatable for efficient comparisons.
    /// </summary>
    [System.Serializable] // Allows this struct to be serialized and shown in the Unity Inspector.
    public struct OM_ColorItem : IEquatable<OM_ColorItem>
    {
        [SerializeField] private string key; // The unique identifier for this color within its palette.
        [SerializeField] private Color color; // The actual color value.

        /// <summary>
        /// Gets or sets the unique key (name) of this color item.
        /// </summary>
        public string Key { get => key; set => key = value; }

        /// <summary>
        /// Initializes a new color item with a key and a Color object.
        /// </summary>
        /// <param name="key">The identifier key for the color.</param>
        /// <param name="color">The color value.</param>
        public OM_ColorItem(string key, Color color)
        {
            this.key = key;
            this.color = color;
        }

        /// <summary>
        /// Initializes a new color item with a key and a hexadecimal color string (e.g., "#FF0000").
        /// Defaults to white if parsing fails.
        /// </summary>
        /// <param name="key">The identifier key for the color.</param>
        /// <param name="colorHex">The hexadecimal string representation of the color.</param>
        public OM_ColorItem(string key, string colorHex)
        {
            this.key = key;
            this.color = Color.white; // Default color if parsing fails
            // Attempt to parse the hex string into a Color object.
            ColorUtility.TryParseHtmlString(colorHex, out this.color); // Note: out parameter modifies this.color directly
        }

        /// <summary>
        /// Initializes a new color item with a key and RGBA float components.
        /// </summary>
        /// <param name="key">The identifier key for the color.</param>
        /// <param name="r">Red component (0.0 to 1.0).</param>
        /// <param name="g">Green component (0.0 to 1.0).</param>
        /// <param name="b">Blue component (0.0 to 1.0).</param>
        /// <param name="a">Alpha component (0.0 to 1.0, defaults to 1).</param>
        public OM_ColorItem(string key, float r, float g, float b, float a = 1)
        {
            this.key = key;
            this.color = new Color(r, g, b, a);
        }

        /// <summary>
        /// Gets the Color value of this item.
        /// </summary>
        /// <returns>The Color object.</returns>
        public Color GetColor()
        {
            return color;
        }

        /// <summary>
        /// Determines whether this instance is equal to another OM_ColorItem instance.
        /// Compares both the key and the color value.
        /// </summary>
        /// <param name="other">The OM_ColorItem to compare with this instance.</param>
        /// <returns>True if the specified OM_ColorItem is equal to the current instance; otherwise, false.</returns>
        public bool Equals(OM_ColorItem other)
        {
            // Check if both the key strings match and the Color structs are equal.
            return key == other.key && color.Equals(other.color);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current OM_ColorItem instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>True if the specified object is an OM_ColorItem and is equal to the current instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            // Check if the object is of the correct type and then use the specific Equals method.
            return obj is OM_ColorItem other && Equals(other);
        }

        /// <summary>
        /// Returns the hash code for this OM_ColorItem instance.
        /// Based on the hash codes of the key and the color.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            // Combine the hash codes of the key and color for a unique hash per item.
            return HashCode.Combine(key, color);
        }
    }

    // --- Color Manager ---

    /// <summary>
    /// Manages collections of color palettes (OM_ColorPalette ScriptableObjects).
    /// Provides a Singleton access point (Instance) to retrieve colors by key and palette name.
    /// Handles automatic creation and saving of the manager asset and a default palette if they don't exist (Editor-only).
    /// </summary>
    [CreateAssetMenu(menuName = "OM/ColorManager", fileName = "ColorManager")] // Makes this ScriptableObject creatable via the Assets/Create menu.
    public class OM_ColorManager : ScriptableObject
    {
        // --- Constants for Asset Paths (Editor-specific) ---
        private const string ColorManagerPath = "ColorManager"; // Path within Resources folder to load the manager.
        private const string ColorManagerFileName = "ColorManager.asset"; // Default filename when creating the manager asset.
        private const string ColorManagerSavePath = "Assets/Plugins/OM/Shared/Resources/ColorManager.asset"; // Full path where the manager asset is saved (Editor).
        private const string ColorPaletteSavePath = "Assets/Plugins/OM/Shared/Resources/"; // Directory where palette assets are saved (Editor).

        // --- Singleton Implementation ---
        private static OM_ColorManager _instance;
        public static OM_ColorManager Instance
        {
            get
            {
                // 1. If instance is already cached, return it.
                if (_instance == null)
                {
                    // 2. Try loading from the Resources folder.
                    _instance = Resources.Load<OM_ColorManager>(ColorManagerPath);
                }

                // 3. If still not found (e.g., first run, asset deleted), create it dynamically.
                if (_instance == null)
                {
                    _instance = ScriptableObject.CreateInstance<OM_ColorManager>();
                    _instance.name = ColorManagerFileName; // Set the internal name.
                    _instance.Refresh(); // Initialize palettes list, check default.

                    #if UNITY_EDITOR
                    // 4. (Editor Only) Save the newly created instance as an asset.
                    // Ensure the target directory exists.
                    string directoryPath = System.IO.Path.GetDirectoryName(ColorManagerSavePath);
                    if (!System.IO.Directory.Exists(directoryPath))
                    {
                        System.IO.Directory.CreateDirectory(directoryPath);
                    }
                    // Create and save the asset.
                    UnityEditor.AssetDatabase.CreateAsset(_instance, ColorManagerSavePath);
                    UnityEditor.AssetDatabase.SaveAssets();
                    UnityEditor.AssetDatabase.Refresh(); // Ensure Unity recognizes the new asset.
                    // Notify the user.
                    UnityEditor.EditorUtility.DisplayDialog("ColorManager Missing", "The ColorManager asset was missing. A new one has been created at:\n" + ColorManagerSavePath, "OK");
                    #endif
                }
                // Always ensure the default palette exists after getting the instance.
                _instance.CheckDefault();
                return _instance;
            }
        }

        /// <summary>
        /// List holding all the color palettes managed by this instance.
        /// </summary>
        [SerializeField] private List<OM_ColorPalette> palettes = new List<OM_ColorPalette>();

        /// <summary>
        /// Refreshes the manager, ensuring the palettes list exists and refreshing individual palettes.
        /// Primarily used internally after dynamic creation.
        /// </summary>
        private void Refresh()
        {
            if (palettes == null) palettes = new List<OM_ColorPalette>();
            //CheckDefault(); // CheckDefault is now called reliably in the Instance getter.
            foreach (var palette in palettes)
            {
                 if (palette != null) palette.Refresh(); // Refresh each palette's internal dictionary.
            }
        }

        /// <summary>
        /// Gets a specific color palette by its key (name).
        /// If the requested palette doesn't exist, it returns the default palette.
        /// </summary>
        /// <param name="key">The name of the palette to retrieve.</param>
        /// <returns>The requested OM_ColorPalette, or the default palette if not found.</returns>
        public OM_ColorPalette GetPalette(string key)
        {
            // Find the palette with the matching name. Case-sensitive.
            var palette = palettes.FirstOrDefault(x => x != null && x.PaletteName == key);
            // If not found, fall back to the default palette.
            if (palette == null) return GetDefaultPalette();
            return palette;
        }

        /// <summary>
        /// Gets the default color palette (named "default").
        /// Ensures the default palette exists using CheckDefault().
        /// </summary>
        /// <returns>The default OM_ColorPalette.</returns>
        public OM_ColorPalette GetDefaultPalette()
        {
            CheckDefault(); // Make sure the default palette exists or is created.
            // Find the palette specifically named "default".
            var defaultPalette = palettes.FirstOrDefault(x => x != null && x.PaletteName == "default");
            // This should ideally not return null after CheckDefault, but handle defensively.
             if (defaultPalette == null)
             {
                 Debug.LogError("Default color palette could not be found or created!");
                 // Optionally return a new empty palette or throw an exception
                 return ScriptableObject.CreateInstance<OM_ColorPalette>(); // Return an empty temporary one
             }
            return defaultPalette;
        }

        /// <summary>
        /// Gets a specific color by its key from the specified palette (or the default palette).
        /// </summary>
        /// <param name="colorKey">The key of the color to retrieve.</param>
        /// <param name="paletteName">The name of the palette to search in (defaults to "default").</param>
        /// <returns>The requested Color, or Color.white if the color or palette is not found.</returns>
        public Color GetColor(string colorKey, string paletteName = "default")
        {
            CheckDefault(); // Ensure default palette exists if needed.
            var palette = GetPalette(paletteName); // Get the target palette (or default).
            if (palette == null) return Color.white; // Should not happen after GetPalette logic
            return palette.GetColor(colorKey, Color.white); // Get the color from the palette.
        }

        /// <summary>
        /// Gets a random color from the specified palette (or the default palette).
        /// </summary>
        /// <param name="paletteName">The name of the palette to get a random color from (defaults to "default").</param>
        /// <returns>A random Color from the palette, or Color.white if the palette is empty or not found.</returns>
        public Color GetRandomColor(string paletteName = "default")
        {
            CheckDefault(); // Ensure default palette exists if needed.
            var palette = GetPalette(paletteName); // Get the target palette (or default).
             if (palette == null) return Color.white; // Should not happen after GetPalette logic
            return palette.GetRandomColor(); // Get a random color from the palette.
        }

        /// <summary>
        /// Checks if the "default" palette exists in the `palettes` list.
        /// If not, it creates a new OM_ColorPalette named "default" with a standard set of colors,
        /// adds it to the list, and saves it as an asset (Editor-only).
        /// </summary>
        private void CheckDefault()
        {
            // If a palette named "default" already exists, do nothing.
            if (palettes.Any(x => x != null && x.PaletteName == "default")) return;

            // Create the default palette instance.
            var defaultPalette = ScriptableObject.CreateInstance<OM_ColorPalette>();
            // Initialize it with standard colors.
            defaultPalette.OnCreate("default", new List<OM_ColorItem>()
            {
                new OM_ColorItem("default", Color.white), // Note: Often good practice to have a "default" key within the "default" palette
                new OM_ColorItem("red", new Color(1f, 0.29f, 0.45f)),
                new OM_ColorItem("green", new Color(0.45f, 1f, 0.51f)),
                new OM_ColorItem("blue", new Color(0.33f, 0.65f, 1f)),
                new OM_ColorItem("yellow", new Color(1f, 0.99f, 0.51f)),
                new OM_ColorItem("cyan", new Color(0.41f, 1f, 0.98f)),
                new OM_ColorItem("magenta", new Color(0.98f, 0.46f, 1f)),
                new OM_ColorItem("black", Color.black),
                new OM_ColorItem("white", Color.white),
                new OM_ColorItem("gray", Color.gray), // Standard Unity gray
                new OM_ColorItem("grey", Color.grey), // Standard Unity grey (alias)
                new OM_ColorItem("clear", Color.clear), // Transparent
                new OM_ColorItem("pink", new Color(0.97f, 0.42f, 1f)), // Example custom color
                // Add more standard colors as needed...
            });

            #if UNITY_EDITOR
            // (Editor Only) Save the newly created default palette as an asset.
            string paletteAssetPath = ColorPaletteSavePath + "ColorPalette_Default.asset";
             // Ensure the target directory exists.
            string directoryPath = System.IO.Path.GetDirectoryName(paletteAssetPath);
            if (!System.IO.Directory.Exists(directoryPath))
            {
                System.IO.Directory.CreateDirectory(directoryPath);
            }
            UnityEditor.AssetDatabase.CreateAsset(defaultPalette, paletteAssetPath);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
             Debug.Log($"Created default color palette at: {paletteAssetPath}");
            #endif

            // Add the new default palette to the manager's list and save the manager asset.
            palettes.Add(defaultPalette);
            Save(); // Mark manager as dirty and save (Editor).
        }

        /// <summary>
        /// Saves the ColorManager ScriptableObject asset (Editor-only).
        /// Marks the object as dirty so Unity knows it needs saving.
        /// </summary>
        private void Save()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this); // Mark this asset as modified.
            UnityEditor.AssetDatabase.SaveAssets(); // Save all modified assets.
            #endif
        }
    }
}