namespace OM
{
    /// <summary>
    /// Specifies the source or method for retrieving an editor icon.
    /// This enum is used by attributes (like OM_DefaultEditorIconAttribute or OM_SearchPopupItemData)
    /// to determine how the associated icon name should be interpreted and loaded.
    /// </summary>
    public enum OM_IconType
    {
        /// <summary>
        /// Indicates that the icon name corresponds to a built-in Unity Editor icon identifier.
        /// These icons are typically retrieved using `UnityEditor.EditorGUIUtility.IconContent(iconName)`.
        /// Examples: "d_GameObject Icon", "console.infoicon", "d_Prefab Icon".
        /// The 'd_' prefix often denotes dark-theme versions, but IconContent usually handles theme switching.
        /// </summary>
        DefaultUnityIcon = 0, // Value 0, often the default choice.

        /// <summary>
        /// Indicates that the icon name corresponds to the filename (without extension)
        /// of a Texture2D asset located within any "Resources" folder in the Unity project.
        /// These icons are loaded using `UnityEngine.Resources.Load<Texture2D>(iconName)`.
        /// Example: If you have "Assets/MyStuff/Resources/MyCustomIcon.png", the iconName would be "MyCustomIcon".
        /// </summary>
        ResourcesFolder = 1, // Value 1.
    }
}