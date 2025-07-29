namespace OM
{
    /// <summary>
    /// Represents standard anchor points, typically used for positioning UI elements
    /// relative to a container (like the screen or a parent RectTransform).
    /// Values correspond to common layout presets.
    /// </summary>
    public enum OM_Anchor
    {
        /// <summary>Top center anchor.</summary>
        Top = 0,
        /// <summary>Bottom center anchor.</summary>
        Bottom = 1,
        /// <summary>Middle right anchor.</summary>
        Right = 2,
        /// <summary>Middle left anchor.</summary>
        Left = 3,
        /// <summary>Top right corner anchor.</summary>
        TopRight = 4,
        /// <summary>Top left corner anchor.</summary>
        TopLeft = 5,
        /// <summary>Bottom right corner anchor.</summary>
        BottomRight = 6,
        /// <summary>Bottom left corner anchor.</summary>
        BottomLeft = 7,
        /// <summary>Center anchor (middle of the container).</summary>
        Center = 8
    }

    /// <summary>
    /// Represents the four cardinal directions (Up, Down, Left, Right).
    /// Commonly used for movement, orientation, or selection logic.
    /// </summary>
    public enum OM_Direction
    {
        /// <summary>Represents the upward direction.</summary>
        Top, // Implicitly 0
        /// <summary>Represents the downward direction.</summary>
        Down, // Implicitly 1
        /// <summary>Represents the rightward direction.</summary>
        Right, // Implicitly 2
        /// <summary>Represents the leftward direction.</summary>
        Left // Implicitly 3
    }

    /// <summary>
    /// Represents the primary axes in a 2D coordinate system or layout context.
    /// </summary>
    public enum OM_Axis
    {
        /// <summary>Represents the horizontal axis (typically X).</summary>
        Horizontal, // Implicitly 0
        /// <summary>Represents the vertical axis (typically Y).</summary>
        Vertical // Implicitly 1
    }

    /// <summary>
    /// Explicitly represents the axes in a 2D Cartesian coordinate system.
    /// </summary>
    public enum OM_Axis2D
    {
        /// <summary>The X-axis.</summary>
        X, // Implicitly 0
        /// <summary>The Y-axis.</summary>
        Y // Implicitly 1
    }

    /// <summary>
    /// Explicitly represents the axes in a 3D Cartesian coordinate system.
    /// </summary>
    public enum OM_Axis3D
    {
        /// <summary>The X-axis.</summary>
        X, // Implicitly 0
        /// <summary>The Y-axis.</summary>
        Y, // Implicitly 1
        /// <summary>The Z-axis.</summary>
        Z // Implicitly 2
    }
}