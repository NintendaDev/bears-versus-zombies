using OM.Editor;
using OM.TimelineCreator.Runtime;
using UnityEngine.UIElements;

namespace OM.TimelineCreator.Editor
{
    /// <summary>
    /// Represents the Inspector panel UI for the Timeline Editor.
    /// It displays details and editable properties of the currently selected track/clip.
    /// </summary>
    /// <typeparam name="T">The type of the data clip, derived from <see cref="OM_ClipBase"/>.</typeparam>
    /// <typeparam name="TTrack">The concrete type of the track, derived from <see cref="OM_Track{T, TTrack}"/>.</typeparam>
    public class OM_TimelineInspector<T, TTrack> : OM_Container // Inherits from a base container likely providing structure
        where T : OM_ClipBase
        where TTrack : OM_Track<T, TTrack>
    {
        /// <summary>
        /// Gets the reference to the main Timeline UI element this inspector belongs to.
        /// </summary>
        public OM_Timeline<T, TTrack> Timeline { get; }

        /// <summary>
        /// Gets the specific container within the inspector where the track's details (like property fields) are displayed.
        /// </summary>
        public OM_Container Container { get; } // A nested container for the actual inspector content

        /// <summary>
        /// Gets the currently selected track whose details are being displayed.
        /// Can be null if no track is selected.
        /// </summary>
        public OM_Track<T, TTrack> SelectedTrack { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OM_TimelineInspector{T, TTrack}"/> class.
        /// </summary>
        /// <param name="timeline">The parent timeline instance.</param>
        public OM_TimelineInspector(OM_Timeline<T, TTrack> timeline)
        {
            // Store the reference to the parent timeline.
            Timeline = timeline;

            // Create and add a header label for the inspector panel.
            var header = new Label("Timeline Inspector");
            header.style.fontSize = 20; // Set header font size
            // TODO: Add additional styling for the header if needed (e.g., margins, padding)
            Add(header);

            // Create the main container for the inspector's content.
            Container = new OM_Container();
            Add(Container); // Add the content container to the inspector's root

            // Subscribe to the timeline's selection change event.
            // When the selected track changes, OnSelectedTrackChanged will be called.
            Timeline.OnSelectedTrackChanged += OnSelectedTrackChanged;

            // Initialize with the currently selected track (if any)
            OnSelectedTrackChanged(Timeline.SelectedTrack);
        }

        /// <summary>
        /// Called when the selected track in the parent timeline changes.
        /// Updates the internal <see cref="SelectedTrack"/> reference and triggers a refresh of the inspector UI.
        /// </summary>
        /// <param name="track">The newly selected track (can be null).</param>
        private void OnSelectedTrackChanged(OM_Track<T, TTrack> track)
        {
            // Update the reference to the currently selected track.
            SelectedTrack = track;
            // Refresh the inspector panel to display the details of the new selection (or clear it if null).
            RefreshInspector();
        }

        /// <summary>
        /// Refreshes the content of the inspector panel.
        /// Clears the existing content and, if a track is selected, potentially re-populates it
        /// with the selected track's properties (using PropertyDrawer or custom UI).
        /// </summary>
        public virtual void RefreshInspector()
        {
            // Clear any previous content from the inspector container.
            Container.Clear();

            // If no track is selected, there's nothing more to display.
            if (SelectedTrack == null) return;

             // For now, just add a placeholder label:
             Container.Add(new Label($"Inspecting: {SelectedTrack.Clip.ClipName ?? "Unnamed Clip"}"));
        }
    }
}