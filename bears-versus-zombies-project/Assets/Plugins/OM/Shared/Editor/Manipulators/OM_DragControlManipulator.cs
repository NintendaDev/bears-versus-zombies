using UnityEngine;
using UnityEngine.UIElements;

namespace OM.Editor
{
    /// <summary>
    /// Interface defining the contract for a VisualElement that can respond to drag operations
    /// initiated and managed by the OM_DragControlManipulator.
    /// </summary>
    public interface IOM_DragControlDraggable
    {
        /// <summary>
        /// Called when a drag operation starts on this element.
        /// </summary>
        /// <param name="mousePosition">The local mouse position within the element where the drag started.</param>
        void StartDrag(Vector2 mousePosition);

        /// <summary>
        /// Called continuously during a drag operation as the mouse moves.
        /// </summary>
        /// <param name="delta">The change in mouse position since the last Drag call or StartDrag.</param>
        /// <param name="mousePosition">The current local mouse position within the element.</param>
        void Drag(Vector2 delta, Vector2 mousePosition);

        /// <summary>
        /// Called when the drag operation ends (mouse button is released).
        /// </summary>
        /// <param name="delta">The total change in mouse position from StartDrag to EndDrag.</param>
        /// <param name="mousePosition">The local mouse position within the element where the drag ended.</param>
        void EndDrag(Vector2 delta, Vector2 mousePosition);
    }

    /// <summary>
    /// Interface defining the contract for a VisualElement that can respond to click events
    /// detected and dispatched by the OM_DragControlManipulator.
    /// </summary>
    public interface IOM_DragControlClickable
    {
        /// <summary>
        /// Called when a click (a MouseDown followed by a MouseUp within a short time and distance)
        /// is detected on this element.
        /// </summary>
        /// <param name="mouseButton">The mouse button that was clicked (e.g., LeftMouse, RightMouse).</param>
        /// <param name="e">The original MouseUpEvent containing detailed event information.</param>
        /// <param name="mousePosition">The local mouse position within the element where the click occurred.</param>
        void Click(MouseButton mouseButton, MouseUpEvent e, Vector2 mousePosition);
    }

    /// <summary>
    /// A UIElements MouseManipulator that handles detecting both drag operations and clicks on a target VisualElement.
    /// It identifies if the target element implements IOM_DragControlDraggable or IOM_DragControlClickable
    /// and calls the appropriate interface methods based on mouse input.
    /// Differentiates clicks from drags based on mouse movement distance and time duration between MouseDown and MouseUp.
    /// </summary>
    public class OM_DragControlManipulator : MouseManipulator
    {
        // State Tracking
        private bool _isActive = false; // True if a potential drag operation is currently active (mouse button is down).
        private Vector2 _startMousePosition; // Local position where the mouse button was initially pressed.
        private float _startTime; // Time (using Time.realtimeSinceStartup) when the mouse button was pressed.

        // References to interfaces implemented by the target element (if any).
        private IOM_DragControlDraggable _draggable; // Stores the draggable interface if found on MouseDown.
        private IOM_DragControlClickable _clickable; // Stores the clickable interface if found on MouseDown.

        /// <summary>
        /// Registers the necessary mouse event callbacks on the target VisualElement.
        /// Called automatically when the manipulator is added to an element.
        /// </summary>
        protected override void RegisterCallbacksOnTarget()
        {
            // Listen for mouse button down, mouse movement, and mouse button up events on the element this manipulator is attached to.
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
            target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
        }

        /// <summary>
        /// Unregisters the mouse event callbacks from the target VisualElement.
        /// Called automatically when the manipulator is removed from an element or the element is removed from the hierarchy.
        /// </summary>
        protected override void UnregisterCallbacksFromTarget()
        {
            // Stop listening to the events to prevent memory leaks and unexpected behavior.
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
        }

        /// <summary>
        /// Handles the MouseDownEvent. Initiates a potential drag or click operation.
        /// </summary>
        /// <param name="e">The MouseDownEvent arguments.</param>
        private void OnMouseDown(MouseDownEvent e)
        {
            // Store initial state for drag/click detection.
            _startMousePosition = e.localMousePosition;
            _startTime = Time.realtimeSinceStartup; // Use unscaled time for reliable click duration check.

            // Prevent starting a new operation if one is somehow already active (shouldn't normally happen).
            if (_isActive)
            {
                e.StopImmediatePropagation(); // Stop event to prevent conflicts.
                return; // Added return for clarity.
            }
            // Check if the event target is a valid VisualElement.
            else if (e.target is VisualElement visualElement)
            {
                // --- Check for Draggable Interface ---
                // Only start dragging with the left mouse button.
                if (e.button == (int)MouseButton.LeftMouse && // Check for Left Mouse Button explicitly
                    visualElement is IOM_DragControlDraggable draggable) // Check if the element implements the draggable interface.
                {
                    // Found a draggable element.
                    _draggable = draggable; // Store the reference.

                    // Prepare for dragging.
                    target.CaptureMouse(); // Ensure this manipulator continues receiving mouse events even if the cursor leaves the element.
                    e.StopPropagation(); // Prevent other elements or default behaviors from handling this event.
                    _isActive = true;    // Mark that a drag operation has started.
                    _draggable.StartDrag(_startMousePosition); // Notify the draggable element that dragging has begun.
                }

                // --- Check for Clickable Interface ---
                // Check if the element also implements the clickable interface, regardless of the button pressed (click logic is in OnMouseUp).
                if (visualElement is IOM_DragControlClickable clickable)
                {
                    _clickable = clickable; // Store the reference for potential use in OnMouseUp.
                }
                else
                {
                    _clickable = null; // Ensure it's null if the interface isn't present.
                }
            }
        }

        /// <summary>
        /// Handles the MouseMoveEvent. Updates the drag operation if active.
        /// </summary>
        /// <param name="e">The MouseMoveEvent arguments.</param>
        private void OnMouseMove(MouseMoveEvent e)
        {
            // Only proceed if a drag is currently active and we have a valid draggable target.
            if (!_isActive || _draggable == null) return;

            // Calculate the mouse movement delta since the drag started.
            var mouseDelta = e.localMousePosition - _startMousePosition;

            // Notify the draggable element of the movement.
            _draggable.Drag(mouseDelta, e.localMousePosition);
        }

        /// <summary>
        /// Handles the MouseUpEvent. Determines if it was a click or the end of a drag,
        /// and calls the appropriate interface methods. Cleans up state.
        /// </summary>
        /// <param name="e">The MouseUpEvent arguments.</param>
        private void OnMouseUp(MouseUpEvent e)
        {
            // --- Click Detection ---
            // Check if enough time has passed and if the mouse moved significantly.
            // If time is short AND distance is small, consider it a click.
            const float clickMaxDuration = 0.35f;
            const float clickMaxDistance = 6.0f;
            if ((Time.realtimeSinceStartup - _startTime < clickMaxDuration) &&
                (e.localMousePosition - _startMousePosition).magnitude < clickMaxDistance)
            {
                // If a clickable interface was found on MouseDown, trigger its Click method.
                if (_clickable != null)
                {
                    // Provide the button, event details, and position to the clickable element.
                    _clickable.Click((MouseButton)e.button, e, e.mousePosition); // Pass the original event 'e'
                    e.StopPropagation(); // Prevent other handlers if the click was processed.
                }
            }

            // Reset clickable reference after checking, regardless of whether it was used.
            _clickable = null;

            // --- Drag End Handling ---
            // Only proceed if a drag operation was active and we have a valid draggable target.
            if (!_isActive || _draggable == null)
            {
                // If not dragging, ensure mouse capture is released if it was somehow captured without drag starting
                if (target.HasMouseCapture())
                    target.ReleaseMouse();
                return;
            }


            // A drag operation was in progress.
            // Calculate the final delta.
            var finalDelta = e.localMousePosition - _startMousePosition;

            // Notify the draggable element that the drag has ended.
            _draggable.EndDrag(finalDelta, e.localMousePosition);

            // --- Cleanup ---
            _draggable = null; // Clear the draggable reference.
            _isActive = false; // Mark the operation as no longer active.
            target.ReleaseMouse(); // Release mouse capture.
            e.StopPropagation(); // Prevent further handling of this MouseUp event.
        }
    }
}