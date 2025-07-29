using System;
using System.Collections.Generic;
using UnityEngine;

namespace OM.Animora.Runtime
{
    /// <summary>
    /// Defines a set of standard color types that can be easily referenced,
    /// often used by attributes like <see cref="AnimoraColorAttribute"/>
    /// to assign predefined colors to elements in the editor UI.
    /// </summary>
    public enum AnimoraColorType
    {
        Default,
        Red,
        Green,
        Blue,
        Yellow,
        Cyan,
        Magenta,
        Black,
        White
    }

    /// <summary>
    /// Attribute used to associate a specific color with an Animora class (like a clip or action).
    /// This color might be used for visual distinction in editor UIs (e.g., timeline track color).
    /// Can be defined using RGB(A) floats, a hex string, or a predefined <see cref="AnimoraColorType"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)] // Can be applied to classes
    public class AnimoraColorAttribute : Attribute
    {
        /// <summary>
        /// A static dictionary mapping predefined <see cref="AnimoraColorType"/> enums to their corresponding Unity <see cref="Color"/> values.
        /// </summary>
        private static Dictionary<AnimoraColorType, Color> ColorsByName { get; } = new Dictionary<AnimoraColorType, Color>()
        {
            {AnimoraColorType.Default, new Color(1, 1, 1)}, // White
            {AnimoraColorType.Red, new Color(1, 0, 0)},
            {AnimoraColorType.Green, new Color(0, 1, 0)},
            {AnimoraColorType.Blue, new Color(0, 0, 1)},
            {AnimoraColorType.Yellow, new Color(1, 1, 0)},
            {AnimoraColorType.Cyan, new Color(0, 1, 1)},
            {AnimoraColorType.Magenta, new Color(1, 0, 1)},
            {AnimoraColorType.Black, new Color(0, 0, 0)},
            {AnimoraColorType.White, new Color(1, 1, 1)}
        };

        /// <summary>
        /// Gets the <see cref="Color"/> value defined by this attribute instance.
        /// </summary>
        public Color Color { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimoraColorAttribute"/> class using RGB float values.
        /// </summary>
        /// <param name="r">Red component (0-1).</param>
        /// <param name="g">Green component (0-1).</param>
        /// <param name="b">Blue component (0-1).</param>
        /// <param name="a">Alpha component (0-1, defaults to 1 for opaque).</param>
        public AnimoraColorAttribute(float r, float g, float b, float a = 1)
        {
            Color = new Color(r, g, b, a);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimoraColorAttribute"/> class using a hexadecimal color string (e.g., "#FF0000").
        /// Defaults to white if the string is invalid.
        /// </summary>
        /// <param name="hexColor">The hexadecimal color string (e.g., "#RRGGBB" or "#RRGGBBAA").</param>
        public AnimoraColorAttribute(string hexColor)
        {
            // Attempt to parse the hex string
            if (ColorUtility.TryParseHtmlString(hexColor, out var color))
            {
                Color = color;
            }
            else // Default to white on failure
            {
                Debug.LogWarning($"Invalid hex color string '{hexColor}' provided to AnimoraColorAttribute. Defaulting to white.");
                Color = Color.white;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimoraColorAttribute"/> class using a hexadecimal color string and an explicit alpha value.
        /// Defaults to white (with the specified alpha) if the hex string is invalid.
        /// </summary>
        /// <param name="hexColor">The hexadecimal color string (e.g., "#RRGGBB"). Alpha component will be overridden.</param>
        /// <param name="a">Alpha component (0-1).</param>
        public AnimoraColorAttribute(string hexColor, float a)
        {
            // Attempt to parse the hex string
            if (ColorUtility.TryParseHtmlString(hexColor, out var color))
            {
                // Use parsed RGB with the provided alpha
                Color = new Color(color.r, color.g, color.b, a);
            }
            else // Default to white (with specified alpha) on failure
            {
                 Debug.LogWarning($"Invalid hex color string '{hexColor}' provided to AnimoraColorAttribute. Defaulting to white with alpha {a}.");
                Color = new Color(1, 1, 1, a);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimoraColorAttribute"/> class using a predefined <see cref="AnimoraColorType"/>.
        /// </summary>
        /// <param name="color">The predefined color type.</param>
        public AnimoraColorAttribute(AnimoraColorType color)
        {
            Color = GetColorByType(color);
        }

        /// <summary>
        /// Retrieves the Unity <see cref="Color"/> value corresponding to a given <see cref="AnimoraColorType"/>.
        /// </summary>
        /// <param name="colorType">The predefined color type.</param>
        /// <returns>The corresponding Unity Color. Returns the default color if the type is not found (shouldn't normally happen).</returns>
        public static Color GetColorByType(AnimoraColorType colorType)
        {
            // Try to get the color from the dictionary
            if (ColorsByName.TryGetValue(colorType, out var color))
            {
                return color;
            }

            // Fallback logic if the enum value isn't in the dictionary (e.g., future enum additions)
            Debug.LogWarning($"Color '{colorType}' not found in AnimoraColorAttribute dictionary. Returning default color.");
            return ColorsByName[AnimoraColorType.Default]; // Return the defined default
        }
    }

    /// <summary>
    /// Attribute used to define how an Animora class (like a clip or action) should appear
    /// in editor creation menus (e.g., the "Add Track" or "Add Action" menus).
    /// Specifies the display name and the menu path.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)] // Applicable to classes, only one allowed, not inherited
    public class AnimoraCreateAttribute : Attribute
    {
        /// <summary>
        /// Gets the menu path where the item should appear (e.g., "Transform/Position").
        /// Uses '/' as a separator for submenus.
        /// </summary>
        public string Path { get; }
        /// <summary>
        /// Gets the display name of the item in the creation menu.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimoraCreateAttribute"/> class.
        /// </summary>
        /// <param name="name">The display name for the menu item.</param>
        /// <param name="path">The menu path (e.g., "Category/SubCategory").</param>
        public AnimoraCreateAttribute(string name, string path)
        {
            Name = name;
            Path = path;
        }
    }

    /// <summary>
    /// Attribute used to provide a descriptive text or tooltip for an Animora class.
    /// This description might be shown in the editor inspector or other UI elements.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)] // Applicable to classes, only one allowed, not inherited
    public class AnimoraDescriptionAttribute : Attribute
    {
        /// <summary>
        /// Gets the descriptive text associated with the class.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimoraDescriptionAttribute"/> class.
        /// </summary>
        /// <param name="description">The description text.</param>
        public AnimoraDescriptionAttribute(string description)
        {
            Description = description;
        }
    }

     /// <summary>
    /// Attribute used to associate an icon with an Animora class.
    /// The icon can be loaded from Unity's built-in icons or from a Resources folder.
    /// Used for visual representation in editor UIs (e.g., timeline tracks, inspector headers).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)] // Applicable to classes, only one allowed, not inherited
    public class AnimoraIconAttribute : Attribute
    {
        // Note: OM_IconType seems to be defined elsewhere, assuming it has DefaultUnityIcon and ResourcesFolder members.
        // If OM_IconType is not accessible, you might need to define a similar enum here or adjust the logic.

        /// <summary>
        /// Gets the type indicating the source of the icon (Unity built-in or Resources).
        /// </summary>
        public OM_IconType IconType { get; } // Assuming OM_IconType enum exists
        /// <summary>
        /// Gets the name of the icon.
        /// For DefaultUnityIcon, this is the string identifier used by EditorGUIUtility.IconContent (e.g., "d_Transform Icon").
        /// For ResourcesFolder, this is the path relative to a Resources folder (without extension).
        /// </summary>
        public string IconName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimoraIconAttribute"/> class.
        /// </summary>
        /// <param name="iconName">The name/path of the icon.</param>
        /// <param name="iconType">The source type of the icon (defaults to Unity built-in icons).</param>
        public AnimoraIconAttribute(string iconName, OM_IconType iconType = OM_IconType.DefaultUnityIcon) // Assuming OM_IconType enum exists
        {
            IconType = iconType;
            IconName = iconName;
        }

        /// <summary>
        /// Attempts to load and retrieve the <see cref="Texture2D"/> for the icon defined by this attribute.
        /// Handles loading from either Unity's internal icons or the Resources folder based on <see cref="IconType"/>.
        /// Logs a warning if the icon cannot be found.
        /// </summary>
        /// <returns>The loaded Texture2D icon, or null if not found.</returns>
        public Texture2D GetIcon()
        {
            // Attempt to load from Resources if specified
            if (IconType == OM_IconType.ResourcesFolder) // Assuming OM_IconType enum exists
            {
                var icon = Resources.Load<Texture2D>(IconName);
                if (icon != null) return icon;
                // Log warning if not found in Resources
                Debug.LogWarning($"[AnimoraIconAttribute] Icon '{IconName}' not found in Resources folder.");
            }
            // Otherwise, attempt to load from Unity's built-in icons
            else // DefaultUnityIcon
            {
                // Use EditorGUIUtility to find built-in icons (works in Editor context)
                #if UNITY_EDITOR
                var iconContent = UnityEditor.EditorGUIUtility.IconContent(IconName);
                if (iconContent != null)
                {
                     var icon = iconContent.image as Texture2D;
                     if (icon != null) return icon;
                }
                // Log warning if not found via EditorGUIUtility
                Debug.LogWarning($"[AnimoraIconAttribute] Icon '{IconName}' not found in Unity built-in icons or project.");
                #else
                 // Loading EditorGUIUtility icons is not possible outside the editor.
                 Debug.LogWarning($"[AnimoraIconAttribute] Cannot load editor icon '{IconName}' in a build.");
                #endif
            }
            // Return null if the icon couldn't be loaded from either source
            return null;
        }
    }

    /// <summary>
    /// Attribute used to associate search keywords with an Animora class.
    /// These keywords can be used by editor tools to filter or search for specific clips or actions.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)] // Applicable to classes, only one allowed, not inherited
    public class AnimoraKeywordsAttribute : Attribute
    {
        /// <summary>
        /// Gets the array of keywords associated with the class.
        /// </summary>
        public string[] Keywords { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimoraKeywordsAttribute"/> class.
        /// </summary>
        /// <param name="keywords">A variable number of string keywords.</param>
        public AnimoraKeywordsAttribute(params string[] keywords)
        {
            Keywords = keywords;
        }
    }

    /// <summary>
    /// Attribute applied to <see cref="AnimoraAction"/> classes to specify the required type(s)
    /// of target Components the action is designed to operate on.
    /// Used for validation, potentially in the editor to filter applicable actions.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)] // Applicable to action classes, only one, not inherited
    public class AnimoraActionAttribute : Attribute // Renamed from original context to avoid conflict if used elsewhere
    {
        /// <summary>
        /// Gets the array of <see cref="Type"/> objects representing the required target component types.
        /// An action might require a Transform, a Rigidbody, etc.
        /// </summary>
        public Type[] RequireTypes { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimoraActionAttribute"/> class.
        /// </summary>
        /// <param name="requireTypes">A variable number of Types representing the required target components.</param>
        public AnimoraActionAttribute(params Type[] requireTypes)
        {
            RequireTypes = requireTypes;
        }

        /// <summary>
        /// Checks if a given component type is compatible with the required types specified by this attribute.
        /// Compatibility means the given type is either one of the required types or inherits from one of them.
        /// </summary>
        /// <param name="type">The component type to check.</param>
        /// <returns>True if the type is compatible or if no types are required, false otherwise.</returns>
        public bool IsTargetType(Type type)
        {
            // If no specific types are required, any type is considered valid.
            if (RequireTypes == null || RequireTypes.Length == 0)
                return true;

            // Check against each required type
            foreach (var targetType in RequireTypes)
            {
                // Check for direct type match or if the provided type is assignable (inherits/implements)
                if (type == targetType || targetType.IsAssignableFrom(type))
                {
                    return true; // Found a compatible type
                }
            }
            // No compatible type found among the requirements
            return false;
        }
    }
}