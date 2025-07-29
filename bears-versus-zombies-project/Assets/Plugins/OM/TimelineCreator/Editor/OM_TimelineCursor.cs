using OM.Editor;
using OM.TimelineCreator.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.TimelineCreator.Editor
{
    /// <summary>
    /// Represents the vertical cursor line in the Timeline Editor UI.
    /// Displays the current playback time or the preview scrub time.
    /// </summary>
    /// <typeparam name="T">The type of the data clip, derived from <see cref="OM_ClipBase"/>.</typeparam>
    /// <typeparam name="TTrack">The concrete type of the track, derived from <see cref="OM_Track{T, TTrack}"/>.</typeparam>
    public class OM_TimelineCursor<T,TTrack> : VisualElement
        where T : OM_ClipBase
        where TTrack : OM_Track<T,TTrack>
    {
        /// <summary>
        /// Reference to the parent Timeline UI element.
        /// </summary>
        private readonly OM_Timeline<T,TTrack> _timeline;
        /// <summary>
        /// Reference to the container where the cursor's visual elements should be positioned relative to (usually the time ruler area in the header).
        /// </summary>
        private readonly VisualElement ParentContainer;
        /// <summary>
        /// The main visual element representing the vertical cursor line.
        /// </summary>
        private readonly VisualElement _line;
        // Optional: Icon element for the top of the cursor (commented out)
        // private readonly VisualElement _icon;

        /// <summary>
        /// Initializes a new instance of the <see cref="OM_TimelineCursor{T, TTrack}"/> class.
        /// Creates the visual elements for the cursor line (and optionally an icon).
        /// </summary>
        /// <param name="timeline">The parent timeline instance.</param>
        public OM_TimelineCursor(OM_Timeline<T,TTrack> timeline)
        {
            // Store reference to the parent timeline
            _timeline = timeline;
            // Store reference to the container used for horizontal positioning (time ruler)
            ParentContainer = _timeline.Header.Container2;

            // --- Basic Cursor Setup ---
            // Set initial size to zero, positioning is absolute and controlled manually
            style.width = 0;
            style.height = 0;
            style.position = Position.Absolute; // Position controlled via UpdatePosition

            // --- Line Element ---
            // Create the visual line element
            _line = new VisualElement()
                // Prevent the line itself from blocking mouse events intended for elements behind it
               .SetPickingMode(PickingMode.Ignore);
            // Set line color (a medium gray)
            _line.style.backgroundColor = new StyleColor(new Color(0.51f, 0.51f, 0.51f));
            // Set line position relative to the cursor's root element (which follows the mouse/time)
            _line.style.position = Position.Absolute;
            _line.style.left = 0; // Align line to the cursor's horizontal position
            _line.style.width = 1; // Line width of 1 pixel
            _line.style.height = 90; // Initial height (will be updated)
            // Add the line element to the cursor's visual tree
            Add(_line);

            // --- Icon Element (Commented Out) ---
            // // Create an icon element (optional, currently disabled)
            // _icon = new VisualElement().SetPickingMode(PickingMode.Ignore);
            // // Position the icon relative to the cursor's root
            // _icon.style.position = Position.Absolute;
            // // Set icon size
            // _icon.style.width = 16;
            // _icon.style.height = 16;
            // // Load icon from Unity Editor resources
            // _icon.style.backgroundImage =
            //     EditorGUIUtility.IconContent("Animation.EventMarker").image as Texture2D;
            // // Offset the icon horizontally to center it on the line
            // _icon.style.left = -8; // Half of the icon width (16 / 2)
            // // Add the icon to the cursor's visual tree
            // Add(_icon);
        }

        /// <summary>
        /// Updates the absolute position of the cursor's root element.
        /// </summary>
        /// <param name="x">The horizontal position (left offset).</param>
        /// <param name="y">The vertical position (top offset).</param>
        public void UpdatePosition(float x, float y)
        {
            // Set the 'left' style property for horizontal positioning
            style.left = x;
            // Set the 'top' style property for vertical positioning
            style.top = y;
        }

        /// <summary>
        /// Updates the height of the cursor's vertical line element.
        /// Typically called when the timeline body height changes.
        /// </summary>
        /// <param name="height">The new height for the cursor line.</param>
        public void UpdateHeight(float height)
        {
            // Set the height of the line's style
            _line.style.height = height;
        }

        /// <summary>
        /// Calculates and returns the current time represented by the cursor's horizontal position.
        /// </summary>
        /// <returns>The time in seconds.</returns>
        public float GetTime()
        {
            // Calculate the cursor's offset relative to its positioning container (ParentContainer)
            // layout.x gives the position relative to this VisualElement's parent
            var left = layout.x - ParentContainer.layout.x;
            // Convert the pixel offset to time using the timeline's pixels-per-second scale
            float pixelPerSecond = _timeline.GetPixelPerSecond();
             // Guard against division by zero if the scale is invalid (e.g., timeline width is zero or duration is zero)
             if (Mathf.Approximately(pixelPerSecond, 0f))
                 return 0f;
            return left / pixelPerSecond;

             // Original code was: return layout.x / _timeline.GetPixelPerSecond();
             // This might be correct if the cursor's direct parent *is* the ParentContainer,
             // but using the relative offset is generally safer if the hierarchy changes.
        }
    }
}