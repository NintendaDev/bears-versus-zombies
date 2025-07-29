using OM.Editor;
using OM.TimelineCreator.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.TimelineCreator.Editor
{
    /// <summary>
    /// Represents the main body area of the Timeline Editor UI.
    /// This area contains the actual track rows, a background grid, and overlay elements like the snapping line.
    /// It handles adding/removing tracks visually and managing the overall height based on the number of tracks.
    /// It also handles clicks on the background area to deselect tracks or show context menus.
    /// </summary>
    /// <typeparam name="T">The type of the data clip, derived from <see cref="OM_ClipBase"/>.</typeparam>
    /// <typeparam name="TTrack">The concrete type of the track, derived from <see cref="OM_Track{T, TTrack}"/>.</typeparam>
    public class OM_TimelineBody<T,TTrack> :
        VisualElement, IOM_DragControlClickable // Implements clickable interface for background clicks
        where T : OM_ClipBase
        where TTrack : OM_Track<T,TTrack>
    {
        /// <summary>
        /// Reference to the parent Timeline UI element.
        /// </summary>
        private readonly OM_Timeline<T,TTrack> _timeline;
        /// <summary>
        /// The minimum calculated height for the body area, ensuring it doesn't collapse completely when empty.
        /// </summary>
        private readonly float _minHeight;
        /// <summary>
        /// The visual element responsible for drawing the background grid lines.
        /// </summary>
        private readonly OM_TimelineBackground _background;

        /// <summary>
        /// Gets the container VisualElement that holds all the track (<see cref="OM_Track{T, TTrack}"/>) elements.
        /// </summary>
        public VisualElement TracksContainer { get; }
        /// <summary>
        /// Gets the container VisualElement used for overlay elements like the snapping line or selection rectangle.
        /// This container sits on top of the tracks container.
        /// </summary>
        public VisualElement OverlayContainer { get; }
        /// <summary>
        /// Gets the <see cref="OM_SnappingLine"/> visual element displayed during drag operations.
        /// </summary>
        public OM_SnappingLine SnappingLine { get; }

        // Optional: Selection rectangle element (commented out)
        // public OM_SelectionElement SelectionElement { get; }

        /// <summary>
        /// Stores the mouse position when a drag operation (like selection) starts (commented out).
        /// </summary>
        private Vector2 _startMousePosition;

        /// <summary>
        /// Initializes a new instance of the <see cref="OM_TimelineBody{T, TTrack}"/> class.
        /// Sets up the structure with background, track container, and overlay container.
        /// </summary>
        /// <param name="timeline">The parent timeline instance.</param>
        public OM_TimelineBody(OM_Timeline<T,TTrack> timeline)
        {
            _timeline = timeline;
            // Assign name and USS class for identification and styling
            name = "OM_TimelineBody";
            AddToClassList("timeline-body");

            // Calculate minimum height based on showing a few empty track slots
            _minHeight = 5 * (OM_TimelineUtil.ClipHeight + OM_TimelineUtil.ClipSpaceBetween);

            // Create and add the background grid element
            _background = new OM_TimelineBackground();
            Add(_background);

            // Create the container for track elements
            TracksContainer = new VisualElement()
                // Ignore mouse events directly on the container itself; events are handled by tracks or background
               .SetPickingMode(PickingMode.Ignore);
            TracksContainer.AddToClassList("tracks-container"); // USS class for styling
            TracksContainer.style.height = _minHeight; // Set initial minimum height
            Add(TracksContainer); // Add track container on top of the background

            // Create the overlay container
            OverlayContainer = new VisualElement()
                .SetPickingMode(PickingMode.Ignore) // Ignore mouse events on the overlay container itself
                .SetName("overlay-container"); // Name for identification
            OverlayContainer.style.position = Position.Absolute; // Position absolutely to cover the body
            OverlayContainer.StretchToParentSize(); // Make it fill the entire body area
            Add(OverlayContainer); // Add overlay on top of track container

            // Create the snapping line and add it to the overlay, initially hidden
            SnappingLine = new OM_SnappingLine().SetDisplay(false);
            OverlayContainer.Add(SnappingLine);

        }

        /// <summary>
        /// Adds a track's visual element to the <see cref="TracksContainer"/>.
        /// </summary>
        /// <param name="track">The track element to add.</param>
        public void AddTrack(OM_Track<T,TTrack> track)
        {
            TracksContainer.Add(track); // Add track to the container
            UpdateHeight(); // Recalculate body height
            _background.MarkDirtyRepaint(); // Mark background for redraw (grid lines)
        }

        /// <summary>
        /// Removes a track's visual element from the <see cref="TracksContainer"/>.
        /// </summary>
        /// <param name="track">The track element to remove.</param>
        public void RemoveTrack(OM_Track<T,TTrack> track)
        {
            TracksContainer.Remove(track); // Remove track from container
            UpdateHeight(); // Recalculate body height
            _background.MarkDirtyRepaint(); // Mark background for redraw
        }

        /// <summary>
        /// Removes all track visual elements from the <see cref="TracksContainer"/>.
        /// </summary>
        public void ClearTracks()
        {
            TracksContainer.Clear(); // Remove all children tracks
            UpdateHeight(); // Recalculate body height
            _background.MarkDirtyRepaint(); // Mark background for redraw
        }

        /// <summary>
        /// Updates the height of the <see cref="TracksContainer"/> based on the number of tracks currently in the timeline.
        /// Ensures the height doesn't fall below the calculated minimum height (<see cref="_minHeight"/>).
        /// </summary>
        public void UpdateHeight()
        {
            // Calculate required height based on number of tracks and standard spacing
            var height = _timeline.TracksList.Count * (OM_TimelineUtil.ClipHeight + OM_TimelineUtil.ClipSpaceBetween);
            // Ensure height is at least the minimum calculated height
            height = Mathf.Max(height, _minHeight);
            // Apply the calculated height to the tracks container
            TracksContainer.style.height = height;
            // The body VisualElement itself should probably auto-size or be managed by its parent.
            // Setting the TracksContainer height implicitly affects the body if layout is correct.
        }

        /// <summary>
        /// Handles mouse clicks directly on the timeline body's background area.
        /// Implements the <see cref="IOM_DragControlClickable"/> interface.
        /// </summary>
        /// <param name="mouseButton">The mouse button that was clicked.</param>
        /// <param name="e">The original <see cref="MouseUpEvent"/>.</param>
        /// <param name="mousePosition">The position of the click within the body element's coordinates.</param>
        public void Click(MouseButton mouseButton, MouseUpEvent e, Vector2 mousePosition)
        {
            switch (mouseButton)
            {
                case MouseButton.LeftMouse:
                    // Left-clicking the background deselects any currently selected track
                    _timeline.SelectTrack(null);
                    break;
                case MouseButton.RightMouse:
                    // Right-clicking the background shows the timeline's main context menu
                    _timeline.ShowContextMenu(e.mousePosition); // Use event's mousePosition for context menu
                    break;
            }
        }
    }
}