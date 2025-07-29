using System.Collections.Generic;
using UnityEngine;

namespace OM
{
    /// <summary>
    /// Represents a collection of named colors (OM_ColorItem).
    /// This is a ScriptableObject, allowing color palettes to be created and managed as assets in the Unity project.
    /// Provides methods to efficiently retrieve colors by name or index, or get a random color.
    /// </summary>
    [CreateAssetMenu(fileName = "OM_ColorPalette", menuName = "OM/ColorPalette")] // Enables creation via Assets/Create menu.
    public class OM_ColorPalette : ScriptableObject
    {
        /// <summary>
        /// The name of this specific color palette (e.g., "UI", "Characters", "default").
        /// Used by OM_ColorManager to identify palettes.
        /// </summary>
        [SerializeField] private string paletteName;

        /// <summary>
        /// The list of color items contained within this palette.
        /// This is serialized and visible in the Inspector.
        /// </summary>
        [SerializeField] private List<OM_ColorItem> colors = new List<OM_ColorItem>(); // Initialize to avoid null refs

        /// <summary>
        /// Gets or sets the name of this color palette.
        /// </summary>
        public string PaletteName { get => paletteName; set => paletteName = value; }

        /// <summary>
        /// Gets or sets the list of OM_ColorItem objects in this palette.
        /// Setting this will require calling Refresh() or TrySetup() afterwards
        /// to update the internal lookup dictionary.
        /// </summary>
        public List<OM_ColorItem> Colors { get => colors; set => colors = value; }

        /// <summary>
        /// Internal dictionary for fast color lookup by key (color name).
        /// This is built on-demand by TrySetup() from the `colors` list.
        /// It is not serialized.
        /// </summary>
        private Dictionary<string, OM_ColorItem> _colorDictionary;

        /// <summary>
        /// Ensures the internal lookup dictionary (`_colorDictionary`) is initialized.
        /// Creates the dictionary from the `colors` list if it's null or if the list count has changed.
        /// Logs an error if duplicate color keys are found in the `colors` list.
        /// </summary>
        public void TrySetup()
        {
            // If the dictionary exists and has the same number of items as the list, assume it's up-to-date.
            if (_colorDictionary != null && colors != null && _colorDictionary.Count == colors.Count) return;

            // Ensure the colors list itself is not null before proceeding.
            if (colors == null) colors = new List<OM_ColorItem>();

            // Create or recreate the dictionary.
            _colorDictionary = new Dictionary<string, OM_ColorItem>();
            foreach (var colorItem in colors)
            {
                // Attempt to add the item. TryAdd prevents exceptions on duplicates but returns false.
                if (!_colorDictionary.TryAdd(colorItem.Key, colorItem))
                {
                    // Log an error if a duplicate key is detected. The first item with the key will be used.
                    Debug.LogError($"Duplicate color key found: '{colorItem.Key}' in palette '{paletteName}'. Please ensure all color keys are unique within a palette.");
                }
            }
        }

        /// <summary>
        /// Initializes a newly created palette instance with a name and a list of colors.
        /// Typically called by OM_ColorManager when creating the default palette.
        /// Also implicitly calls Refresh() to build the dictionary.
        /// </summary>
        /// <param name="newPaletteName">The name for the new palette.</param>
        /// <param name="newColors">The list of colors for the new palette.</param>
        public void OnCreate(string newPaletteName, List<OM_ColorItem> newColors)
        {
            this.paletteName = newPaletteName;
            this.colors = newColors ?? new List<OM_ColorItem>(); // Ensure colors isn't null
            Refresh(); // Build the dictionary after setting colors.
        }

        /// <summary>
        /// Retrieves a color from the palette by its index in the `colors` list.
        /// </summary>
        /// <param name="index">The zero-based index of the color.</param>
        /// <param name="defaultColor">The color to return if the index is out of bounds.</param>
        /// <returns>The Color at the specified index, or `defaultColor` if the index is invalid.</returns>
        public Color GetColor(int index, Color defaultColor)
        {
             // Ensure the colors list is not null and the index is valid.
            if (colors == null || index < 0 || index >= colors.Count)
            {
                return defaultColor;
            }
            return colors[index].GetColor(); // Return the color from the item at the index.
        }

        /// <summary>
        /// Retrieves a color from the palette by its string key (name).
        /// Uses the internal dictionary for efficient lookup.
        /// </summary>
        /// <param name="colorName">The key (name) of the color to retrieve.</param>
        /// <param name="defaultColor">The color to return if the key is not found or is null/empty.</param>
        /// <returns>The Color associated with the key, or `defaultColor` if not found.</returns>
        public Color GetColor(string colorName, Color defaultColor)
        {
            TrySetup(); // Ensure the dictionary is initialized.

            // Return default if the name is invalid or the dictionary is somehow still null.
            if (string.IsNullOrEmpty(colorName) || _colorDictionary == null)
            {
                return defaultColor;
            }

            // Try to get the color item from the dictionary.
            if (_colorDictionary.TryGetValue(colorName, out var colorItem))
            {
                return colorItem.GetColor(); // Return the color from the found item.
            }

            // Key not found, return the default color.
            return defaultColor;
        }

        /// <summary>
        /// Retrieves a random color from the `colors` list in this palette.
        /// </summary>
        /// <returns>A random Color from the palette, or Color.white if the palette is empty or null.</returns>
        public Color GetRandomColor()
        {
            TrySetup(); // Ensures 'colors' list isn't null internally if dictionary setup runs.

            // If the list is null or empty, return white.
            if (colors == null || colors.Count == 0) return Color.white;

            // Select a random index within the bounds of the list.
            var randomIndex = UnityEngine.Random.Range(0, colors.Count);
            return colors[randomIndex].GetColor(); // Return the color at the random index.
        }

        /// <summary>
        /// Retrieves the entire OM_ColorItem struct by its string key.
        /// Useful if you need both the key and the color value.
        /// </summary>
        /// <param name="colorName">The key (name) of the color item to retrieve.</param>
        /// <returns>The OM_ColorItem associated with the key, or a default OM_ColorItem if not found.</returns>
        public OM_ColorItem GetColorItem(string colorName)
        {
            TrySetup(); // Ensure the dictionary is initialized.

            // Return default struct if name invalid or dictionary null.
            if (string.IsNullOrEmpty(colorName) || _colorDictionary == null)
            {
                return default; // Returns default(OM_ColorItem)
            }

            // Use GetValueOrDefault which returns the default value (struct default) if the key isn't found.
            return _colorDictionary.GetValueOrDefault(colorName);
        }

        /// <summary>
        /// Reinitializes the internal color dictionary. Call this if the `colors` list
        /// is modified externally (e.g., through a custom editor script) after initial setup.
        /// Also ensures the `colors` list is not null.
        /// </summary>
        public void Refresh()
        {
             // Ensure colors list exists
            if (colors == null) colors = new List<OM_ColorItem>();
            // Force rebuild dictionary by setting it to null before calling TrySetup
             _colorDictionary = null;
            TrySetup();
        }
    }
}