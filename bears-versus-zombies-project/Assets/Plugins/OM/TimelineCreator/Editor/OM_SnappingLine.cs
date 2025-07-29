using OM.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.TimelineCreator.Editor
{
    /// <summary>
    /// Represents the visual vertical snapping line displayed in the timeline editor
    /// when dragging clips or handles near snap points (other clips or timeline boundaries).
    /// It consists of a thin vertical line positioned absolutely within its parent container.
    /// </summary>
    public class OM_SnappingLine : VisualElement
    {
        /// <summary>
        /// The actual VisualElement used to draw the visible vertical line.
        /// </summary>
        private readonly VisualElement _line;

        /// <summary>
        /// Initializes a new instance of the <see cref="OM_SnappingLine"/> class.
        /// Sets up initial styles for the container and creates the inner line element.
        /// </summary>
        public OM_SnappingLine()
        {
            // Assign a name for identification and USS styling
            name = "snapping-line";
            // Use absolute positioning, controlled by SetPosition methods
            style.position = Position.Absolute;
            // Initial position and size (will be updated by SetPosition)
            style.left = 0;
            style.width = 0; // Container itself has no width initially
            style.height = 0; // Container itself has no height initially
             // Prevent this element (and its children unless overridden) from blocking mouse events
            pickingMode = PickingMode.Ignore;

            // Create the inner visual element for the line itself
            _line = new VisualElement()
                .SetName("line") // Name for identification/styling
                .SetPickingMode(PickingMode.Ignore); // Line also ignores mouse events

            // Style the inner line element
            _line.style.backgroundColor = new StyleColor(Color.cyan); // Set line color
            _line.style.position = Position.Absolute; // Position absolutely within the container
            _line.style.left = 0; // Align to the left of the container
            _line.style.width = 2; // Set line thickness
            _line.style.height = 90; // Default height (can be changed by SetFromTo)
            // Default vertical position: center the line vertically around the y=0 point of the container.
            // This assumes the container's top is set to the target snapping point's Y coordinate.
            _line.style.top = -_line.style.height.value.value * 0.5f; // Center vertically around the container's top

            // Add the inner line element to this container
            Add(_line);
        }

        /// <summary>
        /// Sets the visibility and horizontal/vertical position of the snapping line container.
        /// </summary>
        /// <param name="enable">True to show the line, false to hide it.</param>
        /// <param name="x">The horizontal position (left offset) where the line should appear.</param>
        /// <param name="y">The vertical position (top offset) where the line container should be placed (line centers around this).</param>
        public void SetPosition(bool enable, float x, float y)
        {
            // Set the display style (visible/hidden)
            this.SetDisplay(enable);
            // Set the horizontal position of the container (where the line visually starts)
            style.left = x;
            // Set the vertical position of the container (the inner line centers around this)
            style.top = y;
        }

        /// <summary>
        /// Adjusts the height and vertical position of the inner line element to span
        /// between two vertical points, typically used to connect snapping points on different tracks.
        /// Note: The container's position should still be set using <see cref="SetPosition(bool, float, float)"/>.
        /// </summary>
        /// <param name="from">The starting point (Vector2, only Y is used significantly).</param>
        /// <param name="to">The ending point (Vector2, only Y is used significantly).</param>
        public void SetFromTo(Vector2 from, Vector2 to)
        {
            // Calculate the vertical distance between the points
            var formY = to.y - from.y; // Renamed 'formY' -> 'deltaY' for clarity would be better
            // Set the line's height to the absolute vertical distance
            _line.style.height = Mathf.Abs(formY);
            // Adjust the line's top position relative to its container.
            // If 'to' is below 'from' (formY > 0), the line starts at the container's top (0).
            // If 'to' is above 'from' (formY < 0), the line starts above the container's top
            // by its full height, effectively drawing upwards from the container's Y position.
            _line.style.top = (formY > 0) ? 0 : -_line.style.height.value.value;
        }

        /// <summary>
        /// Sets the visibility and position of the snapping line container using a Vector2.
        /// </summary>
        /// <param name="enable">True to show the line, false to hide it.</param>
        /// <param name="position">The position (Vector2) where the line should appear (X for horizontal, Y for vertical).</param>
        public void SetPosition(bool enable, Vector2 position)
        {
            // Delegate to the overload using float x and float y
            SetPosition(enable, position.x, position.y);
        }
    }
}