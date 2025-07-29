using OM.Editor;
using OM.TimelineCreator.Runtime; 
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements; 

namespace OM.TimelineCreator.Editor 
{
    /// <summary>
    /// Represents the visual UI element for a single clip on a timeline track in the editor.
    /// It displays the clip's information (name, duration, state) and provides interactive elements
    /// like drag handles for adjusting timing and potentially buttons or icons for specific actions.
    /// Implements drag/click interfaces for manipulation within the timeline editor.
    /// </summary>
    /// <typeparam name="T">The specific type of OM_ClipBase represented by this UI element.</typeparam>
    /// <typeparam name="TTrack">The specific type of OM_Track containing this track clip.</typeparam>
    public class OM_TrackClip<T, TTrack> :
        VisualElement, IOM_DragControlDraggable, IOM_DragControlClickable
        where T : OM_ClipBase
        where TTrack : OM_Track<T, TTrack> // Constraint ensures Track is of the correct type
    {
        // --- References ---

        /// <summary>Gets the parent Track visual element that contains this clip UI.</summary>
        public OM_Track<T, TTrack> Track { get; }

        /// <summary>Gets the data clip associated with this UI element.</summary>
        protected T Clip => Track.Clip; // Convenience accessor

        // --- UI Elements ---

        /// <summary>Gets the draggable handle on the right side for adjusting duration/end time.</summary>
        public OM_ClipHandleRight<T, TTrack> HandleRight { get; private set; }

        /// <summary>Gets the draggable handle on the left side for adjusting start time.</summary>
        public OM_ClipHandleLeft<T, TTrack> HandleLeft { get; private set; }

        /// <summary>Gets the Label element displaying the clip's name.</summary>
        public Label Title { get; private set; }

        /// <summary>Gets the main container VisualElement for the clip's body content.</summary>
        public VisualElement Body { get; private set; }

        /// <summary>Gets the VisualElement used to display a selection outline.</summary>
        public VisualElement Outline { get; private set; }

        /// <summary>Gets the VisualElement displaying the clip's type icon.</summary>
        public VisualElement Icon { get; private set; }

        /// <summary>Gets the VisualElement displaying an error icon if the clip has errors.</summary>
        public VisualElement ErrorIcon { get; private set; }

        /// <summary>Gets the VisualElement used to display an outline on mouse hover.</summary>
        public VisualElement HoverOutline { get; private set; }

        /// <summary>Gets the colored line element often used to indicate clip status or category.</summary>
        public OM_ColoredLine ColoredLine { get; private set; }

        /// <summary>Gets the Label element displaying detailed timing information (start/duration/end).</summary>
        public Label Details { get; private set; }

        /// <summary>Gets the Button used to ping/highlight the associated asset or GameObject (optional).</summary>
        public Button PingButton { get; private set; } // May not be present in all versions
        
        public Label DescriptionLabel { get; private set; } // May not be present in all versions

        /// <summary>
        /// Constructor for the track clip UI.
        /// </summary>
        /// <param name="track">The parent OM_Track instance.</param>
        public OM_TrackClip(OM_Track<T, TTrack> track)
        {
            Track = track;
            name = "OM_Clip"; // Assign base name
            AddToClassList("clip"); // Base USS class
            // Add initial class for fade-in animation, removed after first layout/update
            AddToClassList("no-animation");
        }

        /// <summary>
        /// Initializes the visual elements within the TrackClip.
        /// Creates handles, labels, icons, and sets up initial styles and event callbacks.
        /// Should be called after the constructor and potentially after adding to the hierarchy.
        /// </summary>
        public void Init()
        {
            // Set base height based on utility constants
            style.height = OM_TimelineUtil.ClipHeight;
            // Position above the track base, accounting for spacing
            style.top = OM_TimelineUtil.ClipSpaceBetween * 0.5f;

            // --- Create Body Container ---
            Body = new VisualElement().SetPickingMode(PickingMode.Ignore); // Ignore picks on body itself
            Body.style.position = Position.Absolute; // Absolute positioning within TrackClip
            Body.style.width = new StyleLength(new Length(100, LengthUnit.Percent)); // Full width
            Body.style.height = new StyleLength(new Length(100, LengthUnit.Percent)); // Full height
            Body.style.overflow = Overflow.Hidden; // Clip content
            Body.AddToClassList("clip-body"); // USS class for styling
            Add(Body); // Add body to TrackClip

            // --- Create Handles (if track allows width control) ---
            if (Track.CanControlWidth()) // Check if resizing is permitted
            {
                HandleRight = new OM_ClipHandleRight<T, TTrack>(Track);
                Add(HandleRight); // Add handle to TrackClip (sibling of Body)

                HandleLeft = new OM_ClipHandleLeft<T, TTrack>(Track);
                Add(HandleLeft); // Add handle to TrackClip
            }

            // --- Create Colored Status Line ---
            // Use the HighlightColor from the clip data, fallback to default if needed
            ColoredLine = new OM_ColoredLine(Clip?.HighlightColor ?? Color.grey);
            Body.Add(ColoredLine); // Add line inside the Body

            DescriptionLabel = new Label(Clip?.ClipDescription);
            DescriptionLabel.SetPickingMode(PickingMode.Ignore); // Ignore picks on description
            DescriptionLabel.style.position = Position.Absolute; // Absolute positioning
            DescriptionLabel.style.width = new StyleLength(new Length(100, LengthUnit.Percent)); // Full width
            DescriptionLabel.style.height = new StyleLength(new Length(100, LengthUnit.Percent)); // Full height
            DescriptionLabel.style.unityTextAlign = TextAnchor.MiddleCenter; // Center text
            DescriptionLabel.style.fontSize = 12;
            DescriptionLabel.style.color = new Color(0.62f, 0.62f, 0.8f); // Dim text color
            DescriptionLabel.style.top = 5; // Position above the clip
            Body.Add(DescriptionLabel);
            
            // --- Create Title Label ---
            Title = new Label(Clip?.ClipName ?? "Clip").SetName("clip-name");
            Title.SetPickingMode(PickingMode.Ignore); // Ignore picks on title
            Title.style.position = Position.Absolute; // Absolute positioning
            Title.style.width = new StyleLength(new Length(100, LengthUnit.Percent)); // Full width
            Title.style.height = new StyleLength(new Length(100, LengthUnit.Percent)); // Full height
            Title.style.unityTextAlign = TextAnchor.MiddleCenter; // Center text
            Title.style.top = -8;
            Body.Add(Title); // Add title inside the Body

            // --- Create Selection Outline ---
            Outline = new VisualElement().SetPickingMode(PickingMode.Ignore);
            Outline.style.position = Position.Absolute;
            Outline.SetBorderRadius(5); // Rounded corners
            Outline.SetBorderSize(2); // Border thickness
            Outline.SetBorderColor(Color.white); // Default selection color
            Outline.SetTrbl(-3); // Position slightly outside the clip bounds
            Outline.SetDisplay(false); // Initially hidden
            Add(Outline); // Add outline as sibling of Body

            // --- Create Hover Outline ---
            HoverOutline = new VisualElement().SetPickingMode(PickingMode.Ignore);
            HoverOutline.style.position = Position.Absolute;
            HoverOutline.SetBorderRadius(5);
            HoverOutline.SetBorderSize(1); // Thinner than selection outline
            HoverOutline.SetBorderColor(Color.white * 0.7f); // Slightly dimmer color
            HoverOutline.SetTrbl(0); // Position exactly on clip bounds
            HoverOutline.SetDisplay(false); // Initially hidden
            Add(HoverOutline); // Add hover outline as sibling

            // --- Create Details Label (for timing info) ---
            Details = new Label("").SetPickingMode(PickingMode.Ignore);
            Details.style.position = Position.Absolute;
            Details.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            Details.style.top = -14; // Position above the clip
            Details.style.unityTextAlign = TextAnchor.LowerLeft; // Align text
            Details.style.fontSize = 10;
            Details.style.color = new Color(0.62f, 0.62f, 0.8f); // Dim text color
            Details.SetDisplay(false); // Initially hidden (shown on drag/selection)
            Add(Details); // Add details label as sibling

            // --- Create Icon ---
            var iconContainer = new VisualElement().SetPickingMode(PickingMode.Ignore);
            iconContainer.style.position = Position.Absolute;
            iconContainer.style.height = new StyleLength(new Length(100, LengthUnit.Percent));
            iconContainer.style.left = 10; // Indent from left edge
            iconContainer.SetJustifyContent(Justify.Center); // Center icon vertically
            Body.Add(iconContainer); // Add container to Body

            Icon = new VisualElement().SetName("Icon").AddClassNames("clip-icon");
            Icon.style.width = 16; // Standard icon size
            Icon.style.height = 16;
            Icon.style.backgroundImage = Track.GetClipIcon(); // Get icon from parent track
            // Add click event listener for the icon
            Icon.RegisterCallback<ClickEvent>(e =>
            {
                Track.OnIconClicked(); // Delegate click to parent track
                e.StopPropagation(); // Prevent click from reaching the track clip body
            });
            iconContainer.Add(Icon); // Add icon to its container

            // --- Create Error Icon ---
            var errorContainer = new VisualElement().SetPickingMode(PickingMode.Ignore);
            errorContainer.style.position = Position.Absolute;
            errorContainer.style.height = new StyleLength(new Length(100, LengthUnit.Percent));
            errorContainer.style.right = 5; // Position near the right edge
            errorContainer.SetJustifyContent(Justify.Center); // Center vertically
            Body.Add(errorContainer); // Add container to Body

            ErrorIcon = new VisualElement();
            ErrorIcon.style.width = 16;
            ErrorIcon.style.height = 16;
            // Use standard Unity error icon
            ErrorIcon.style.backgroundImage = EditorGUIUtility.IconContent("d_console.erroricon.sml").image as Texture2D;
            ErrorIcon.tooltip = "There is an error in this clip"; // Default tooltip
            ErrorIcon.AddClassNames("scale-on-hover", "cursor-link"); // USS classes for interaction hints
            ErrorIcon.SetDisplay(false); // Initially hidden
            // Potentially add click handler to ErrorIcon to show details?
            // ErrorIcon.RegisterCallback<ClickEvent>(e => { /* Show error details */ });
            errorContainer.Add(ErrorIcon); // Add icon to its container

            // --- Setup Initial State & Callbacks ---
            // Schedule removal of the 'no-animation' class after the first frame draw
            this.schedule.Execute(() =>
            {
                RemoveFromClassList("no-animation");
            });

            // Hover effects for the main clip body
            this.RegisterCallback<MouseEnterEvent>(e =>
            {
                if (Track.IsSelected) return; // Don't show hover if already selected
                HoverOutline.SetDisplay(true);
            });
            this.RegisterCallback<MouseOutEvent>(e =>
            {
                HoverOutline.SetDisplay(false);
            });

            // Initial update
            UpdateTrackClip();
            UpdateDetails();
        }

        // --- Update Methods ---

        /// <summary>
        /// Updates the timing details label (Start - Duration - End).
        /// </summary>
        public void UpdateDetails()
        {
            if (Track == null) return;
            Details.text = $"{Track.GetStartTime():0.00} - {Track.GetDuration():0.00} - {Track.GetEndTime():0.00}";
        }

        /// <summary>
        /// Updates the main title label of the clip.
        /// </summary>
        /// <param name="title">The new title text.</param>
        public void UpdateTitle(string title)
        {
            Title.text = title;
        }

        /// <summary>
        /// Updates the background image of the clip's icon.
        /// </summary>
        /// <param name="texture">The new icon texture.</param>
        public void UpdateIcon(Texture2D texture)
        {
            Icon.style.backgroundImage = texture;
        }

        /// <summary>
        /// Shows or hides the error icon and sets its tooltip.
        /// </summary>
        /// <param name="value">True to show the error icon, false to hide it.</param>
        /// <param name="tooltipText">The tooltip message to display when hovering over the error icon.</param>
        public void UpdateErrorIcon(bool value, string tooltipText = "")
        {
            ErrorIcon.SetDisplay(value);
            ErrorIcon.tooltip = tooltipText;
        }

        /// <summary>
        /// Updates the border color of the selection outline.
        /// </summary>
        /// <param name="color">The new border color.</param>
        public void UpdateOutlineColor(Color color)
        {
            Outline.SetBorderColor(color);
        }

        /// <summary>
        /// Called when the underlying clip data or track state might have changed.
        /// Updates various visual aspects of the clip UI.
        /// Can be overridden by subclasses for custom update logic.
        /// </summary>
        public virtual void OnTrackUpdate()
        {
            DescriptionLabel.text = Clip?.ClipDescription;
            // Default implementation might do nothing, or update common elements
            // E.g., UpdateTitle(Clip.ClipName);
            // E.g., ColoredLine.SetColor(Clip.HighlightColor);
        }

        /// <summary>
        /// Updates the visual representation based on the clip's IsActive state.
        /// Typically adds/removes a "disabled" USS class.
        /// </summary>
        /// <param name="value">The new active state.</param>
        public void OnIsActiveChanged(bool value)
        {
            if (value == false)
            {
                this.AddToClassList("disabled"); // Apply disabled styling
            }
            else
            {
                this.RemoveFromClassList("disabled"); // Remove disabled styling
            }
            ColoredLine?.SetActive(value); // Update colored line state
            Body?.SetEnabled(value); // Enable/disable body interactions (if any)
        }

        /// <summary>
        /// Updates the visual representation based on the clip's selection state.
        /// Shows/hides the selection outline.
        /// </summary>
        /// <param name="value">True if the clip is selected, false otherwise.</param>
        public void OnIsSelectedChanged(bool value)
        {
            if (value)
            {
                this.AddToClassList("selected"); // Apply selected styling
                Outline?.SetDisplay(true); // Show selection outline
                HoverOutline?.SetDisplay(false); // Hide hover outline when selected
            }
            else
            {
                this.RemoveFromClassList("selected"); // Remove selected styling
                Outline?.SetDisplay(false); // Hide selection outline
                // Hover outline visibility is handled by MouseEnter/MouseOut
            }
             Details?.SetDisplay(value || Track.IsDragging); // Show details when selected or dragged
        }

        /// <summary>
        /// Updates the visual representation based on whether the parent track is being dragged.
        /// Adds/removes a "no-animation" class to disable transitions during drag.
        /// </summary>
        /// <param name="value">True if the track is being dragged, false otherwise.</param>
        public void OnIsDraggingChanged(bool value)
        {
            if (value)
            {
                this.AddToClassList("no-animation"); // Disable smooth transitions
                 Details?.SetDisplay(true); // Show details while dragging
            }
            else
            {
                this.RemoveFromClassList("no-animation"); // Re-enable transitions
                 Details?.SetDisplay(Track.IsSelected); // Details visibility reverts to selection state
                 UpdateDetails(); // Refresh details text after drag ends
            }
        }

        /// <summary>
        /// Called after the clip's properties (StartTime, Duration) have been potentially modified.
        /// Updates the visual size and position of the clip UI element on the track.
        /// </summary>
        public virtual void UpdateTrackClip()
        {
            if (Track == null) return;
            // Update horizontal position based on start time and timeline scale
            style.left = Track.GetStartTime() * Track.Timeline.GetPixelPerSecond();
            // Update width based on duration and timeline scale
            style.width = Track.GetDuration() * Track.Timeline.GetPixelPerSecond();
        }


        // --- Drag Interface Implementation ---

        /// <summary>Initiates a drag operation on the parent track.</summary>
        public void StartDrag(Vector2 mousePosition)
        {
            Track.StartDrag(mousePosition); // Delegate to parent track
        }

        /// <summary>Handles dragging movement, delegating to the parent track.</summary>
        public void Drag(Vector2 delta, Vector2 mousePosition)
        {
            Track.Drag(delta, mousePosition); // Delegate to parent track
        }

        /// <summary>Ends the drag operation on the parent track.</summary>
        public void EndDrag(Vector2 delta, Vector2 mousePosition)
        {
            Track.EndDrag(delta, mousePosition); // Delegate to parent track
        }

        /// <summary>Handles click events, delegating to the parent track.</summary>
        public void Click(MouseButton mouseButton, MouseUpEvent mouseUpEvent, Vector2 mousePosition)
        {
            Track.Click(mouseButton, mouseUpEvent, mousePosition); // Delegate to parent track
        }
    }
}