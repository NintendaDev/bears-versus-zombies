using OM.Editor;
using OM.TimelineCreator.Runtime;
using UnityEngine.UIElements;

namespace OM.TimelineCreator.Editor
{
    /// <summary>
    /// Represents the footer section of the Timeline Editor UI.
    /// Typically contains controls related to the selected track, such as Delete, Duplicate, Copy,
    /// and potentially global actions like Paste or Copy All.
    /// </summary>
    /// <typeparam name="T">The type of the data clip, derived from <see cref="OM_ClipBase"/>.</typeparam>
    /// <typeparam name="TTrack">The concrete type of the track, derived from <see cref="OM_Track{T, TTrack}"/>.</typeparam>
    public class OM_TimelineFooter<T, TTrack> : OM_Split2 // Inherits from a split view container for layout
        where T : OM_ClipBase
        where TTrack : OM_Track<T, TTrack>
    {
        /// <summary>
        /// Reference to the parent Timeline UI element.
        /// </summary>
        private readonly OM_Timeline<T, TTrack> _timeline;
        /// <summary>
        /// Container holding the buttons specific to the selected track (Delete, Duplicate, Copy).
        /// Visibility is toggled based on whether a track is selected.
        /// </summary>
        private readonly VisualElement _controlContainer;
        /// <summary>
        /// Button used to paste copied track(s). Its enabled state and text might change
        /// based on the content of the copy buffer.
        /// </summary>
        private readonly Button _pasteButton;

        /// <summary>
        /// Reference to the currently selected track in the timeline. Kept in sync via events.
        /// </summary>
        private OM_Track<T, TTrack> _selectedTrack;

        /// <summary>
        /// Initializes a new instance of the <see cref="OM_TimelineFooter{T, TTrack}"/> class.
        /// Sets up layout, creates control buttons (Add, Paste, Delete, Duplicate, Copy, Copy All),
        /// and subscribes to relevant timeline events (selection change, copy buffer change).
        /// </summary>
        /// <param name="timeline">The parent timeline instance.</param>
        public OM_TimelineFooter(OM_Timeline<T, TTrack> timeline)
        {
            // Store reference to the parent timeline
            _timeline = timeline;
            // Assign name for identification and USS styling
            name = "OM_TimelineFooter";
            // Add base USS class for styling
            AddToClassList("timeline-footer");

            // --- Layout Setup using OM_Split2 ---
            // Arrange the two main containers (Container1 for global, Container2 for selection-specific/copy-all) horizontally
            this.SetFlexDirection(FlexDirection.Row)
                // Distribute space between the two main containers
                .SetJustifyContent(Justify.SpaceBetween);

            // Configure Container 1 (Left side - Add/Paste buttons)
            Container1.SetFlexDirection(FlexDirection.Row); // Arrange buttons horizontally
            Container1.SetJustifyContent(Justify.Center);   // Center buttons horizontally within the container
            Container1.style.alignItems = Align.Center;     // Center buttons vertically

            // Configure Container 2 (Right side - Contextual buttons + Copy All)
            Container2.SetFlexDirection(FlexDirection.Row); // Arrange buttons horizontally
            Container2.SetJustifyContent(Justify.Center);   // Center buttons horizontally within the container
            Container2.style.alignItems = Align.Center;     // Center buttons vertically

            // Create the inner container for selection-specific controls (Delete, Duplicate, Copy)
            // These buttons will only be visible when a track is selected.
            _controlContainer = new VisualElement()
                .SetFlexDirection(FlexDirection.Row) // Arrange inner buttons horizontally
                .SetJustifyContent(Justify.Center);   // Center inner buttons horizontally
            
            _controlContainer.style.alignItems = Align.Center; // Center inner buttons vertically
            // Make the control container fill the height of its parent (Container2)
            _controlContainer.style.height = new StyleLength(new Length(100, LengthUnit.Percent));
            // Add this inner container to the right main container (Container2)
            Container2.Add(_controlContainer);

            // --- Button Creation ---

            // Add Track Button (in Container1 - always visible)
            var addTrackButton = new Button(timeline.OnAddTrackClicked) // Assign the action from the timeline
                .SetText("Add") // Set button label
                .AddClassNames("control-button"); // Add USS class for styling

            // Paste Button (in Container1 - always visible, but enabled/disabled based on copy buffer)
            _pasteButton = new Button(timeline.PasteTrack) // Assign the paste action
                .SetText("Paste") // Set button label
                .AddClassNames("control-button"); // Add USS class for styling

            // Add the global buttons to the left container
            Container1.Add(addTrackButton);
            Container1.Add(_pasteButton);

            // --- Selection-Specific Buttons (created but added to _controlContainer, which is initially hidden) ---

            // Delete Button
            var deleteButton = new Button(() =>
            {
                // Only perform action if a track is actually selected
                if (_selectedTrack != null)
                {
                    timeline.DeleteTrack(_selectedTrack); // Call timeline's delete method
                }
            })
                .SetText("X") // Use "X" as a common symbol for delete
                .AddClassNames("control-button"); // Add USS class

            // Duplicate Button
            var duplicateButton = new Button(() =>
            {
                if (_selectedTrack != null)
                {
                    timeline.DuplicateTrack(_selectedTrack); // Call timeline's duplicate method
                }
            })
                .SetText("Duplicate") // Set button label
                .AddClassNames("control-button"); // Add USS class

            // Copy Button
            var copyButton = new Button(() =>
            {
                if (_selectedTrack != null)
                {
                    timeline.CopyTrack(_selectedTrack); // Call timeline's copy method
                }
            })
                .SetText("Copy") // Set button label
                .AddClassNames("control-button"); // Add USS class

            // Add these selection-specific buttons to their dedicated container (_controlContainer)
            _controlContainer.Add(deleteButton);
            _controlContainer.Add(duplicateButton);
            _controlContainer.Add(copyButton);

            // Copy All Button (in Container2, outside _controlContainer - always visible)
            var copyAllButton = new Button(() =>
            {
                timeline.CopyAllTracks(); // Call timeline's copy all method
            })
                .SetText("Copy All") // Set button label
                .AddClassNames("control-button"); // Add USS class
            // Add the Copy All button directly to Container2 (it's not selection-dependent)
            Container2.Add(copyAllButton);


            // --- Event Handling Setup ---

            // Subscribe to track selection changes from the timeline
            timeline.OnSelectedTrackChanged += OnSelectedTrackChanged;

            // Get the copy-paste manager instance specific to the timeline's clip type
            var copyPasteInstance = timeline.GetCopyPasteInstance();
            if (copyPasteInstance != null)
            {
                // If copy/paste is supported, subscribe to its callback
                copyPasteInstance.OnCopyPasteCallback += OnHasCopiedTracksChanged;
                // Set the initial state of the paste button based on the current buffer content
                OnHasCopiedTracksChanged(copyPasteInstance.CanPaste(), copyPasteInstance.GetCopyType());
            }
            else
            {
                // If no copy/paste manager exists for this type, permanently disable the Paste button
                _pasteButton.SetEnabled(false);
            }

            // Set the initial visibility state of the selection-specific controls based on the initially selected track
            OnSelectedTrackChanged(timeline.SelectedTrack);
        }

        /// <summary>
        /// Callback method invoked when the state of the copy/paste buffer changes.
        /// Updates the enabled state and text of the Paste button.
        /// </summary>
        /// <param name="hasCopiedTracks">True if the buffer contains one or more compatible tracks, false otherwise.</param>
        /// <param name="copyType">Indicates if a single item (<see cref="OM_CopyType.Single"/>) or multiple items (<see cref="OM_CopyType.Multiple"/>) are in the buffer.</param>
        private void OnHasCopiedTracksChanged(bool hasCopiedTracks, OM_CopyType copyType)
        {
            // Enable the paste button only if there are tracks in the copy buffer
            _pasteButton.SetEnabled(hasCopiedTracks);

            if (hasCopiedTracks)
            {
                // Adjust the paste button text depending on whether single or multiple items were copied
                _pasteButton.text = copyType == OM_CopyType.Multiple ? "Paste All" : "Paste";
            }
            else
            {
                // Reset to default text when the buffer is empty
                _pasteButton.text = "Paste";
            }
        }

        /// <summary>
        /// Callback method invoked when the selected track in the parent timeline changes.
        /// Updates the internal reference <see cref="_selectedTrack"/> and toggles the visibility
        /// of the container (<see cref="_controlContainer"/>) holding selection-specific controls.
        /// </summary>
        /// <param name="obj">The newly selected track instance, or null if no track is selected.</param>
        private void OnSelectedTrackChanged(OM_Track<T, TTrack> obj)
        {
            // Update the local reference to the selected track
            _selectedTrack = obj;
            // Show the container with Delete, Duplicate, Copy buttons only if a track is actually selected
            _controlContainer.SetDisplay(obj != null);
        }
    }
}