using OM.TimelineCreator.Runtime; 
using UnityEngine.UIElements;

namespace OM.TimelineCreator.Editor
{
    /// <summary>
    /// Represents the "Preview" button within the Timeline Header.
    /// Toggles the timeline's preview state and updates its visual appearance
    /// (e.g., text, USS class) based on whether preview mode is active.
    /// </summary>
    /// <typeparam name="T">The type of the data clip, derived from <see cref="OM_ClipBase"/>.</typeparam>
    /// <typeparam name="TTrack">The concrete type of the track, derived from <see cref="OM_Track{T, TTrack}"/>.</typeparam>
    public class OM_TimelinePreviewButton<T, TTrack> : Button
        where T : OM_ClipBase
        where TTrack : OM_Track<T, TTrack>
    {
        /// <summary>
        /// Flag potentially used for a flashing effect (currently unused).
        /// </summary>
        private bool _flash;
        /// <summary>
        /// Scheduler item potentially used for timing a flashing effect (currently unused).
        /// </summary>
        private IVisualElementScheduledItem _visualElementScheduledItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="OM_TimelinePreviewButton{T, TTrack}"/> class.
        /// </summary>
        /// <param name="timeline">The parent timeline instance this button controls.</param>
        public OM_TimelinePreviewButton(OM_Timeline<T, TTrack> timeline)
        {
            // Register the button's click event to toggle the timeline's preview mode.
            clicked += timeline.TogglePreview;

            // Subscribe to the timeline's preview state change event to update the button's appearance.
            timeline.OnPreviewStateChangedCallback += (isPreviewing) =>
            {
                Refresh(timeline.IsPreviewing); // Update visuals based on the new state
            };

            // Add base USS class for styling.
            this.AddToClassList("previewButton");
            // Set the initial appearance based on the current timeline preview state.
            Refresh(timeline.IsPreviewing);
        }

        /// <summary>
        /// Refreshes the visual state of the button (text and USS class) based on the preview mode.
        /// </summary>
        /// <param name="previewOn">True if preview mode is currently active, false otherwise.</param>
        private void Refresh(bool previewOn)
        {
            if (previewOn)
            {
                // Add the "on" class for specific styling when preview is active.
                this.AddToClassList("on");
                // Set text to indicate preview is active (could be changed to "Stop Preview", etc.)
                this.text = "Preview"; // Or "Stop Previewing"
            }
            else
            {
                // Remove the "on" class when preview is inactive.
                this.RemoveFromClassList("on");
                // Set text to indicate preview can be started.
                this.text = "Preview";
            }
            // Note: The flashing logic using _flash and _visualElementScheduledItem seems incomplete or removed.
        }
    }
}