using System.Collections.Generic; // Required for List<>
using System.Globalization; // Required for CultureInfo.InvariantCulture when formatting numbers
using OM.Editor; // Assuming this contains editor helper extensions like SetFlexDirection, SetJustifyContent, SetBackgroundFromIconContent
using OM.TimelineCreator.Runtime; // Contains OM_ClipBase, OM_PlayState, IOM_TimelinePlayer
using UnityEngine; // Required for Mathf
using UnityEngine.UIElements; // Required for VisualElement, Button, FloatField, Label, Position, StyleLength, LengthUnit, etc.
using UnityEditor.UIElements; // Required for FloatField

namespace OM.TimelineCreator.Editor
{
    /// <summary>
    /// Represents the header section of the Timeline Editor UI.
    /// Contains controls like Play/Stop/Replay buttons, the timeline duration field,
    /// context menu button, preview time scrubber, and the time ruler/numbers.
    /// It handles user interaction with these controls and drag operations on the time ruler for preview scrubbing.
    /// </summary>
    /// <typeparam name="T">The type of the data clip, derived from <see cref="OM_ClipBase"/>.</typeparam>
    /// <typeparam name="TTrack">The concrete type of the track, derived from <see cref="OM_Track{T, TTrack}"/>.</typeparam>
    public class OM_TimelineHeader<T, TTrack> :
        OM_Split2, // Inherits from a split view container (likely for layout)
        IOM_DragControlDraggable, // Interface indicating it can be dragged (for scrubbing)
        IOM_UndoRedoListener, // Interface indicating it responds to Undo/Redo
        IOM_ValidateListener // Interface indicating it responds to validation events
        where T : OM_ClipBase
        where TTrack : OM_Track<T, TTrack>
    {
        /// <summary>
        /// Event triggered when the Play button is clicked.
        /// </summary>
        public event System.Action OnPlayButtonClicked;
        /// <summary>
        /// Event triggered when the Stop button is clicked.
        /// </summary>
        public event System.Action OnStopButtonClicked;
        /// <summary>
        /// Event triggered when the Replay button is clicked.
        /// </summary>
        public event System.Action OnReplayButtonClicked;
        /// <summary>
        /// Event triggered when the Context Menu (...) button is clicked.
        /// </summary>
        public event System.Action OnContextButtonClicked;

        /// <summary>
        /// Reference to the parent Timeline UI element.
        /// </summary>
        private readonly OM_Timeline<T, TTrack> _timeline;
        /// <summary>
        /// List storing the Label elements used to display time markers on the ruler.
        /// </summary>
        private readonly List<Label> _numbers = new List<Label>();

        /// <summary>
        /// Gets the Play button element.
        /// </summary>
        public OM_HeaderButton PlayButton { get; private set; }
        /// <summary>
        /// Gets the Replay button element.
        /// </summary>
        public OM_HeaderButton ReplayButton { get; private set; }
        /// <summary>
        /// Gets the Stop button element.
        /// </summary>
        public OM_HeaderButton StopButton { get; private set; }
        /// <summary>
        /// Gets the Context Menu (...) button element.
        /// </summary>
        public OM_HeaderButton ContextButton { get; private set; }
        /// <summary>
        /// Gets the FloatField element used to display and edit the total timeline duration.
        /// </summary>
        public FloatField DurationField { get; }
        /// <summary>
        /// Gets the current time being previewed (scrubbed to) in seconds.
        /// </summary>
        public float CurrentPreviewTime { get; private set; }
        /// <summary>
        /// Gets the split view container within the first main container (used for layout).
        /// </summary>
        public OM_Split2 Container1SplitView { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OM_TimelineHeader{T, TTrack}"/> class.
        /// Sets up the layout, creates buttons, duration field, and time ruler labels.
        /// </summary>
        /// <param name="timeline">The parent timeline instance.</param>
        public OM_TimelineHeader(OM_Timeline<T, TTrack> timeline) : base() // Call base constructor (likely OM_Split2)
        {
            _timeline = timeline;
            // Register this header with the timeline owner's visual element manager
            timeline.TimelineEditorOwner.VisualElementsManager.AddElement(this);
            name = "OM_TimelineHeader"; // Assign name for identification/styling
            AddToClassList("timeline-header"); // Add USS class for styling

            // --- Layout Setup ---
            this.SetFlexDirection(FlexDirection.Column); // Arrange children vertically
            style.width = new StyleLength(new Length(100, LengthUnit.Percent)); // Span full width
            SetContainer1SizePercent(60); // Allocate percentage height to the top container (controls)
            Container1.style.height = 30; // Set fixed height for the top part (buttons, duration)
            Container2.style.height = 16; // Set fixed height for the bottom part (time ruler)

            // Setup Container 1 (Top Row: Buttons, Duration)
            Container1SplitView = new OM_Split2(); // Use another split view for internal layout
            Container1SplitView.style.height = 30; // Match parent height
            Container1SplitView.SetFlexDirection(FlexDirection.Row); // Arrange children horizontally
            Container1SplitView.SetJustifyContent(Justify.SpaceBetween); // Space out left and right groups
            Container1SplitView.Container1.style.flexGrow = 1; // Allow left side to grow
            Container1.Add(Container1SplitView); // Add the split view to the main Container1

            // Setup Container 2 (Bottom Row: Time Ruler Numbers)
            Container2.SetPickingMode(PickingMode.Ignore); // Ignore mouse events directly on the number container
            Container2.style.flexDirection = FlexDirection.Row; // Arrange numbers horizontally
            Container2.style.justifyContent = Justify.SpaceBetween; // Space out number blocks
            Container2.style.overflow = Overflow.Hidden; // Hide overflowing numbers
            Container2.AddToClassList("timeline-header-numbers-container"); // Add USS class

            // --- Controls Setup (in Container1SplitView.Container1 - Left Side) ---
            Container1SplitView.Container1.SetFlexDirection(FlexDirection.Row); // Arrange controls horizontally
            Container1SplitView.Container1.SetJustifyContent(Justify.FlexStart); // Align controls to the left

            // Add Preview Button (if timeline supports preview)
            if (_timeline.CanBePreviewed)
            {
                var previewButton = _timeline.CreatePreviewButton(); // Create the specific preview button
                Container1SplitView.Container1.Add(previewButton);
            }

            // Duration Field Container and Field
            var timelineDurationContainer = new VisualElement()
                .SetName("DurationContainer")
                .AddClassNames("timeline-duration-field-container"); // Container for styling/layout
            Container1SplitView.Container1.Add(timelineDurationContainer);

            DurationField = new FloatField();
            DurationField.style.paddingLeft = 10; // Add some spacing
            DurationField.AddToClassList("timeline-duration-field"); // USS class
            DurationField.value = timeline.TimelinePlayer.GetTimelineDuration(); // Initialize with current duration
            DurationField.label = "Duration"; // Set label
            DurationField.RegisterValueChangedCallback(evt => // Handle duration changes
            {
                // Ensure duration doesn't go below zero
                if (evt.newValue < 0)
                {
                    DurationField.SetValueWithoutNotify(0); // Reset to 0 visually if invalid
                    // Note: Does not automatically set player duration to 0, relies on the next block
                }

                // Update the actual timeline duration in the player
                // Record Undo step before changing the value
                timeline.TimelinePlayer.RecordUndo("Set Timeline Duration");
                timeline.TimelinePlayer.SetTimelineDuration(DurationField.value); // Use clamped value

                // Notify the player and refresh UI elements that depend on duration
                timeline.TimelinePlayer.OnValidate(); // Trigger validation/updates in the player
                RefreshNumbers(); // Update the time ruler numbers
            });
            timelineDurationContainer.Add(DurationField); // Add field to its container

            // --- Controls Setup (in Container1SplitView.Container2 - Right Side) ---
            // Context Menu Button (...)
            ContextButton = new OM_HeaderButton(() => { OnContextButtonClicked?.Invoke(); }); // Trigger event on click
            ContextButton.Icon.SetBackgroundFromIconContent("_Menu@2x"); // Set icon (using internal Unity icon name)
            ContextButton.AddToClassList("context-menu-button"); // USS class
            ContextButton.style.height = new StyleLength(new Length(100, LengthUnit.Percent)); // Fill height
            Container1SplitView.Container2.Add(ContextButton); // Add to the right side of the split view

            // --- Time Ruler Setup (in Container2) ---
            // Create 11 number containers (for 0s to 10s equivalent steps)
            for (int i = 0; i < 11; i++)
            {
                var numberContainer = new VisualElement();
                numberContainer.AddClassNames("timeline-header-number"); // Base class for styling

                var number = new Label(i.ToString()) // Create label for the number
                    .SetPickingMode(PickingMode.Ignore); // Ignore mouse events
                number.SetName("Number"); // Name for identification
                _numbers.Add(number); // Store label reference
                numberContainer.Add(number); // Add label to its container

                var numberIndicator = new VisualElement(); // Small tick mark below number
                numberIndicator.SetName("NumberIndicator");
                numberIndicator.AddToClassList("timeline-header-number-indicator"); // USS class
                numberContainer.Add(numberIndicator); // Add indicator to container

                // Add specific classes for first and last number containers for styling edges
                if (i == 0) numberContainer.AddClassNames("timeline-header-number-first");
                if (i == 10) numberContainer.AddClassNames("timeline-header-number-last");

                Container2.Add(numberContainer); // Add the number container to the ruler row
            }

            // Initial update of the ruler numbers based on current duration
            RefreshNumbers();

            // --- Playback Control Buttons (in Container1SplitView.Container1 - Left Side) ---
            // Replay Button
            ReplayButton = new OM_HeaderButton(() => { OnReplayButtonClicked?.Invoke(); });
            ReplayButton.Icon.SetBackgroundFromIconContent("preAudioAutoPlayOff@2x"); // Set icon
            Container1SplitView.Container1.Add(ReplayButton);

            // Play Button
            PlayButton = new OM_HeaderButton(() => { OnPlayButtonClicked?.Invoke(); });
            PlayButton.Icon.SetBackgroundFromIconContent("PlayButton@2x"); // Set icon
            Container1SplitView.Container1.Add(PlayButton);

            // Stop Button
            StopButton = new OM_HeaderButton(() => { OnStopButtonClicked?.Invoke(); });
            StopButton.Icon.SetBackgroundFromIconContent("d_Animation.Record"); // Set icon (Record icon often used for Stop)
            Container1SplitView.Container1.Add(StopButton);

             // --- Register for Dragging on Container2 (Time Ruler) ---
             // This allows scrubbing the timeline preview
             //this.AddManipulator(new OM_DragControlManipulator(Container2, this)); // Assumes OM_DragControlManipulator exists
        }

        /// <summary>
        /// Refreshes the text content of the time ruler labels based on the current timeline duration.
        /// Divides the duration into 10 steps.
        /// </summary>
        public void RefreshNumbers()
        {
            var timelineDuration = _timeline.TimelinePlayer.GetTimelineDuration();
            // Calculate the time interval represented by each number label section
            var step = timelineDuration > 0 ? timelineDuration / 10 : 0; // Avoid division by zero

            for (int i = 0; i < _numbers.Count; i++)
            {
                var number = _numbers[i];
                // Format the time value for the label
                // Display total duration for the last label, otherwise display step * i
                // Using ".0" format specifier for one decimal place (adjust if needed)
                // Using InvariantCulture to ensure consistent decimal separator (.)
                number.text = i == _numbers.Count - 1
                    ? timelineDuration.ToString("F1", CultureInfo.InvariantCulture) + "s" // Show total duration at the end
                    : (i * step).ToString("F1", CultureInfo.InvariantCulture) + "s"; // Show intermediate time steps
            }
        }

        /// <summary>
        /// Called when a drag operation starts on the time ruler (Container2).
        /// Initiates the timeline preview mode.
        /// </summary>
        /// <param name="mousePosition">The starting mouse position relative to the draggable element (Container2).</param>
        public void StartDrag(Vector2 mousePosition)
        {
            // Only allow scrubbing if preview is supported and enabled
            if (!_timeline.CanEnterPreview() || !_timeline.CanBePreviewed) return;

            // Clamp the horizontal position to the bounds of the ruler container
            mousePosition.x = Mathf.Clamp(mousePosition.x, 0, Container2.layout.width);

            // Start the preview mode in the parent timeline
            _timeline.StartPreview();

            // Calculate the initial preview time based on the click position
            CurrentPreviewTime = mousePosition.x / _timeline.GetPixelPerSecond();

            // Update the timeline's preview state and cursor position
            _timeline.UpdatePreviewElapsedTime(CurrentPreviewTime);
            _timeline.SetCursorLeft(mousePosition.x); // Set visual cursor position
        }

        /// <summary>
        /// Called continuously during a drag operation on the time ruler.
        /// Updates the preview time and cursor position based on mouse movement.
        /// </summary>
        /// <param name="delta">The change in mouse position since the last update.</param>
        /// <param name="mousePosition">The current mouse position relative to the draggable element (Container2).</param>
        public void Drag(Vector2 delta, Vector2 mousePosition)
        {
            // Only allow scrubbing if preview is supported and enabled
            if (!_timeline.CanEnterPreview() || !_timeline.CanBePreviewed) return;

            // Clamp the horizontal position to the bounds of the ruler container
            mousePosition.x = Mathf.Clamp(mousePosition.x, 0, Container2.layout.width);

            // Calculate the new preview time based on the current mouse position
            CurrentPreviewTime = mousePosition.x / _timeline.GetPixelPerSecond();

            // Update the timeline's preview state and cursor position
            _timeline.UpdatePreviewElapsedTime(CurrentPreviewTime);
            _timeline.SetCursorLeft(mousePosition.x);
        }

        /// <summary>
        /// Called when a drag operation ends on the time ruler.
        /// (Currently no specific logic needed here, preview remains at the last scrubbed time).
        /// </summary>
        /// <param name="delta">The total change in mouse position during the drag.</param>
        /// <param name="mousePosition">The final mouse position.</param>
        public void EndDrag(Vector2 delta, Vector2 mousePosition)
        {
            // Potential future logic: Maybe stop preview automatically? Or snap to nearest frame?
        }

        /// <summary>
        /// Called when an Undo or Redo operation is performed that might affect the timeline state.
        /// Refreshes the duration field and the time ruler numbers.
        /// </summary>
        public void OnUndoRedoPerformed()
        {
            // Update the duration field visually without triggering its own change callback
            DurationField.SetValueWithoutNotify(_timeline.TimelinePlayer.GetTimelineDuration());
            // Refresh the time ruler labels
            RefreshNumbers();
        }

        /// <summary>
        /// Called when the underlying timeline player data is validated (e.g., after loading or significant changes).
        /// Refreshes the duration field and the time ruler numbers.
        /// </summary>
        public void OnPlayerValidate()
        {
            // Update the duration field visually without triggering its own change callback
            DurationField.SetValueWithoutNotify(_timeline.TimelinePlayer.GetTimelineDuration());
            // Refresh the time ruler labels
            RefreshNumbers();
        }
    }
}