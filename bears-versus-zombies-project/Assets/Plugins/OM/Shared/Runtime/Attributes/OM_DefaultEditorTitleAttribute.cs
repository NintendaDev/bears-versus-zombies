namespace OM
{
    /// <summary>
    /// Specifies a default title and an optional icon to be associated with a class,
    /// primarily intended for use in custom Unity editors (e.g., in header bars).
    /// This attribute should be placed on the class definition itself.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class)] // Specifies that this attribute can only be applied to classes.
    public class OM_DefaultEditorTitleAttribute : System.Attribute
    {
        /// <summary>
        /// Gets the default title string defined for the class.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the name/path of the optional icon associated with the title.
        /// Can be null if no icon is specified.
        /// </summary>
        /// <remarks>
        /// This name would typically be used by custom editor code (like OM_DefaultEditorHeaderBar)
        /// to load the actual icon texture using methods like `EditorGUIUtility.IconContent`.
        /// </remarks>
        public string Icon { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OM_DefaultEditorTitleAttribute"/> class
        /// with the specified title and optional icon name.
        /// </summary>
        /// <param name="title">The desired title string to display in the editor.</param>
        /// <param name="icon">
        /// (Optional) The name of the icon to display alongside the title.
        /// This usually corresponds to a built-in Unity editor icon name
        /// (e.g., "d_Prefab Icon", "cs Script Icon") or potentially a resource path.
        /// Defaults to null if no icon is needed.
        /// </param>
        public OM_DefaultEditorTitleAttribute(string title, string icon = null)
        {
            Title = title;
            Icon = icon;
        }
    }
}