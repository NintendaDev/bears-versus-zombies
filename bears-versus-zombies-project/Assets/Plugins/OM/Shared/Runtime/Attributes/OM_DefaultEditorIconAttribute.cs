using UnityEngine;

namespace OM
{
    /// <summary>
    /// Specifies a default icon to be associated with a class, often used by
    /// custom editors or visual tooling within the Unity editor.
    /// This attribute should be placed on the class definition itself.
    /// </summary>
    /// <remarks>
    /// The GetIcon method prioritizes loading from a 'Resources' folder
    /// before falling back to built-in Unity editor icons.
    /// </remarks>
    [System.AttributeUsage(System.AttributeTargets.Class)] // Specifies that this attribute can only be applied to classes.
    public class OM_DefaultEditorIconAttribute : System.Attribute
    {
        /// <summary>
        /// Gets the name/path of the icon specified for the class.
        /// </summary>
        /// <remarks>
        /// This name is used by GetIcon() to load the actual Texture2D.
        /// </remarks>
        public string IconName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OM_DefaultEditorIconAttribute"/> class
        /// with the specified icon name.
        /// </summary>
        /// <param name="iconName">
        /// The name of the icon. This can be a path within a 'Resources'
        /// folder (e.g., "MyIcons/CustomIcon") or the name of a built-in
        /// Unity editor icon (e.g., "d_Prefab Icon", "cs Script Icon").
        /// </param>
        public OM_DefaultEditorIconAttribute(string iconName)
        {
            IconName = iconName;
        }

        /// <summary>
        /// Attempts to load the Texture2D icon specified by <see cref="IconName"/>.
        /// </summary>
        /// <returns>
        /// The loaded <see cref="Texture2D"/> if found either in a 'Resources' folder
        /// or as a built-in Unity editor icon; otherwise, returns null.
        /// </returns>
        /// <remarks>
        /// This method first tries to load the icon using Resources.Load<Texture2D>().
        /// If that fails, it attempts to load it using EditorGUIUtility.IconContent().
        /// Ensure the icon asset exists in the specified location for this to work correctly.
        /// </remarks>
        public Texture2D GetIcon()
        {
            // Attempt to load from Resources first.
            var texture2D = Resources.Load<Texture2D>(IconName);
            if (texture2D != null)
            {
                return texture2D;
            }

            // Fallback: Attempt to load as a built-in Unity editor icon.
            // UnityEditor.EditorGUIUtility.IconContent returns a GUIContent, we need the image.
            // Note: This part uses UnityEditor namespace, so it will only work within the Unity Editor.
#if UNITY_EDITOR
            return UnityEditor.EditorGUIUtility.IconContent(IconName)?.image as Texture2D;
#else
            // Return null if not in the editor, as EditorGUIUtility is unavailable.
            return null;
#endif
        }
    }
}