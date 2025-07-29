using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.Editor
{
    /// <summary>
    /// A UIElements Manipulator that allows users to draw a selection rectangle
    /// by clicking and dragging within a designated parent VisualElement.
    /// It manages the creation and display of an OM_RectangleElement to visualize the selection
    /// and provides a callback during the drag operation with the current state of the rectangle.
    /// </summary>
    public class OM_RectangleSelector : Manipulator
    {
        /// <summary>
        /// The VisualElement that acts as the parent container for the selection rectangle.
        /// The rectangle's coordinates and size are relative to this element's layout.
        /// This element's display style is toggled to show/hide the rectangle.
        /// </summary>
        public VisualElement SelectionParent { get; }

        /// <summary>
        /// The visual representation of the rectangle being drawn.
        /// This is an instance of OM_RectangleElement, created and managed by this manipulator.
        /// </summary>
        public OM_RectangleElement RectangleElement { get; }

        /// <summary>
        /// Gets a value indicating whether the user is currently dragging to draw the rectangle.
        /// True from MouseDown until MouseUp.
        /// </summary>
        public bool IsDragging { get; private set; }

        /// <summary>
        /// Callback action invoked continuously during the mouse drag operation (on MouseMove).
        /// It receives the current OM_RectangleElement, allowing external logic to react
        /// to the changing selection rectangle (e.g., highlighting items within the bounds).
        /// </summary>
        private readonly System.Action<OM_RectangleElement> _onSelection;

        /// <summary>
        /// Stores the local mouse position within the SelectionParent where the drag started.
        /// </summary>
        private Vector2 _startPosition;

        /// <summary>
        /// Initializes a new instance of the OM_RectangleSelector manipulator.
        /// </summary>
        /// <param name="selectionParent">The VisualElement within which the rectangle will be drawn and positioned.</param>
        /// <param name="onSelection">The callback action invoked during the drag with the current rectangle state.</param>
        public OM_RectangleSelector(VisualElement selectionParent, Action<OM_RectangleElement> onSelection)
        {
            this.SelectionParent = selectionParent;
            this._onSelection = onSelection;

            // Create the visual rectangle element.
            RectangleElement = new OM_RectangleElement();
            // Add it to the specified parent container.
            SelectionParent.Add(RectangleElement);
            // Hide the parent (and thus the rectangle) initially.
            SelectionParent.style.display = DisplayStyle.None; // Set initial state to hidden
            // Alternative using extension: SelectionParent.SetDisplay(false);
        }

        /// <summary>
        /// Registers the necessary mouse event callbacks (Down, Move, Up) on the target VisualElement.
        /// Called automatically when the manipulator is added to an element.
        /// </summary>
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
            target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
        }

        /// <summary>
        /// Unregisters the mouse event callbacks from the target VisualElement.
        /// Called automatically when the manipulator is removed from an element.
        /// </summary>
        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
        }

        /// <summary>
        /// Handles the MouseDownEvent. Starts the dragging process.
        /// </summary>
        /// <param name="evt">The mouse down event arguments.</param>
        private void OnMouseDown(MouseDownEvent evt)
        {
            // Only start if the primary button (usually left) is pressed. Consider evt.button if other buttons needed.
             if (evt.button != 0) return;

            IsDragging = true;
            // Calculate start position relative to the SelectionParent's local coordinate system.
            // We subtract the parent's layout position because evt.localMousePosition is relative to the *target*
            // of the manipulator, which might not be the same as the SelectionParent.
            _startPosition = target.ChangeCoordinatesTo(SelectionParent, evt.localMousePosition);
            //_startPosition = evt.localMousePosition - new Vector2(SelectionParent.layout.x, SelectionParent.layout.y); // Alternative if target == SelectionParent

            // Initialize the rectangle at the start position with zero size.
            RectangleElement.SetPosition(_startPosition);
            RectangleElement.SetSize(Vector2.zero);
            // Make the parent (and the rectangle) visible.
            SelectionParent.style.display = DisplayStyle.Flex;
            // Alternative using extension: SelectionParent.SetDisplay(true);

            // Capture the mouse so we continue receiving MouseMove/MouseUp events even if the cursor leaves the target element.
            target.CaptureMouse();
            evt.StopPropagation(); // Prevent other elements from reacting to this event.
        }

        /// <summary>
        /// Handles the MouseMoveEvent. Updates the rectangle size and invokes the callback during dragging.
        /// </summary>
        /// <param name="evt">The mouse move event arguments.</param>
        private void OnMouseMove(MouseMoveEvent evt)
        {
            // Only process if currently dragging.
            if (!IsDragging) return;

            // Calculate current position relative to the SelectionParent.
            var localMousePosition = target.ChangeCoordinatesTo(SelectionParent, evt.localMousePosition);
            //var localMousePosition = evt.localMousePosition - new Vector2(SelectionParent.layout.x, SelectionParent.layout.y); // Alternative

            // Clamp the position within the bounds of the SelectionParent to prevent drawing outside.
            // Note: layout.x/y are relative to the parent's *parent*, layout.width/height are the actual size.
            // xMin/yMin are usually 0 in local coordinates unless padding/margins affect layout.x/y.
            float xMin = 0; // Assuming local origin is top-left of layout rect
            float yMin = 0;
            float xMax = SelectionParent.layout.width;
            float yMax = SelectionParent.layout.height;

            localMousePosition.x = Mathf.Clamp(localMousePosition.x, xMin, xMax);
            localMousePosition.y = Mathf.Clamp(localMousePosition.y, yMin, yMax);

            // Update the rectangle's size based on the difference between the current position and the start position.
            // OM_RectangleElement's SetSize handles negative width/height correctly.
            RectangleElement.SetSize(localMousePosition - _startPosition);

            // Invoke the selection callback, passing the updated rectangle element.
            _onSelection?.Invoke(RectangleElement);
            evt.StopPropagation(); // Prevent other elements from reacting.
        }

        /// <summary>
        /// Handles the MouseUpEvent. Finalizes the drag operation.
        /// </summary>
        /// <param name="evt">The mouse up event arguments.</param>
        private void OnMouseUp(MouseUpEvent evt)
        {
             // Only process if dragging with the primary button. Consider evt.button if other buttons needed.
             if (!IsDragging || evt.button != 0) return;

            // Hide the parent element (which hides the rectangle).
            SelectionParent.style.display = DisplayStyle.None;
             // Alternative using extension: SelectionParent.SetDisplay(false);
            // Mark dragging as finished.
            IsDragging = false;
            // Release mouse capture.
            target.ReleaseMouse();
            evt.StopPropagation(); // Prevent other elements from reacting.

             // Optional: Could invoke a final _onSelection or a separate _onSelectionEnd callback here if needed.
        }
    }

    /// <summary>
    /// A custom VisualElement that draws a simple rectangular outline.
    /// Used by OM_RectangleSelector to visualize the area being selected.
    /// Handles drawing itself via the generateVisualContent callback and provides methods
    /// to set its position and size dynamically, supporting dragging in any direction.
    /// </summary>
    public class OM_RectangleElement : VisualElement
    {
        /// <summary>
        /// Cached top-left position set by SetPosition. Used by SetSize
        /// to correctly calculate the element's final top/left style when
        /// dragging upwards or leftwards (resulting in negative size delta).
        /// </summary>
        private Vector2 _mainPosition;

        /// <summary>
        /// Initializes a new instance of the OM_RectangleElement.
        /// Sets absolute positioning and registers the custom drawing method.
        /// </summary>
        public OM_RectangleElement()
        {
            // Use absolute positioning so 'left', 'top', 'width', 'height' styles work
            // relative to the parent's content area.
            style.position = Position.Absolute;
            // Ensure this element doesn't capture mouse events itself. Interaction is handled by the OM_RectangleSelector.
            pickingMode = PickingMode.Ignore;
            // Register the method that will draw the rectangle's visuals.
            generateVisualContent += GenerateVisualContent;
        }

        /// <summary>
        /// Custom drawing callback invoked by the UIElements rendering system.
        /// Draws the rectangular outline using Painter2D.
        /// </summary>
        /// <param name="mgc">The mesh generation context, providing access to the Painter2D.</param>
        private void GenerateVisualContent(MeshGenerationContext mgc)
        {
            // Check if width or height is zero or negative, no need to draw then.
             if (layout.width <= 0 || layout.height <= 0) return;

            var painter = mgc.painter2D;

            // Configure the drawing style (red stroke, 2 pixels wide).
            painter.strokeColor = Color.red;
            painter.lineWidth = 2;

            // Draw the rectangle path.
            painter.BeginPath();
            painter.MoveTo(Vector2.zero);                       // Top-left corner (local coordinates)
            painter.LineTo(new Vector2(0, layout.height));      // Bottom-left corner
            painter.LineTo(new Vector2(layout.width, layout.height)); // Bottom-right corner
            painter.LineTo(new Vector2(layout.width, 0));       // Top-right corner
            painter.LineTo(Vector2.zero);                       // Close the path back to top-left
            painter.Stroke(); // Render the path outline.
        }

        /// <summary>
        /// Sets the top-left position of the rectangle element within its parent.
        /// </summary>
        /// <param name="position">The desired top-left local position.</param>
        public void SetPosition(Vector2 position)
        {
            // Cache the position for use in SetSize calculations.
            _mainPosition = position;
            // Apply the position using UIElements style properties.
            style.left = position.x;
            style.top = position.y;
        }

        /// <summary>
        /// Sets the size of the rectangle element based on a size delta vector.
        /// This method handles cases where the user drags left or up from the start point,
        /// resulting in negative components in the size delta. It adjusts the element's
        /// top/left position accordingly and uses the absolute width/height.
        /// </summary>
        /// <param name="size">A Vector2 representing the difference between the current mouse position and the start position (current - start).</param>
        public void SetSize(Vector2 size)
        {
            // Handle horizontal dragging direction.
            if (size.x < 0)
            {
                // Dragged left: Adjust 'left' style based on the negative size and the cached start position.
                style.left = _mainPosition.x + size.x; // Adding negative size shifts left
                // Use the absolute value for the actual width.
                style.width = Mathf.Abs(size.x);
            }
            else
            {
                // Dragged right: 'left' style remains at the cached start position.
                style.left = _mainPosition.x;
                // Use the positive size directly as width.
                style.width = size.x;
            }

            // Handle vertical dragging direction.
            if (size.y < 0)
            {
                // Dragged up: Adjust 'top' style based on the negative size and the cached start position.
                style.top = _mainPosition.y + size.y; // Adding negative size shifts up
                // Use the absolute value for the actual height.
                style.height = Mathf.Abs(size.y);
            }
            else
            {
                // Dragged down: 'top' style remains at the cached start position.
                style.top = _mainPosition.y;
                // Use the positive size directly as height.
                style.height = size.y;
            }

            // Mark the visual content as dirty to trigger GenerateVisualContent redraw.
             MarkDirtyRepaint();
        }

        /// <summary>
        /// Checks if this rectangle element's world bounds overlap with another VisualElement's world bounds.
        /// Useful for checking which elements are contained within the selection rectangle.
        /// </summary>
        /// <param name="element">The other VisualElement to check for overlap.</param>
        /// <returns>True if the world bounds overlap, false otherwise.</returns>
        public bool ContainsInRectangle(VisualElement element)
        {
            // Use the built-in worldBound property (which represents the element's bounding box in world space)
            // and its Overlaps method for efficient intersection testing.
            return this.worldBound.Overlaps(element.worldBound);
        }
    }
}
