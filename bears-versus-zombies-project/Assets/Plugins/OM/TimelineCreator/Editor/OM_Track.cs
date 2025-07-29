using System.Collections.Generic;
using OM.Editor; 
using OM.TimelineCreator.Runtime;
using UnityEditor; 
using UnityEngine;
using UnityEngine.UIElements; 

namespace OM.TimelineCreator.Editor
{
    /// <summary>
    /// Abstract base class representing a single track row within the Timeline Editor UI.
    /// It manages the visual representation (<see cref="OM_TrackClip{T, TTrack}"/>) of a data clip (<see cref="OM_ClipBase"/>).
    /// Handles selection, dragging, resizing (via handles), context menus, and updates based on clip data.
    /// </summary>
    /// <typeparam name="T">The type of the data clip, derived from <see cref="OM_ClipBase"/>.</typeparam>
    /// <typeparam name="TTrack">The concrete type of the track itself, derived from <see cref="OM_Track{T, TTrack}"/>.</typeparam>
    public abstract class OM_Track<T, TTrack> : VisualElement
        where T : OM_ClipBase
        where TTrack : OM_Track<T, TTrack>
    {
        /// <summary>
        /// Reference to the parent Timeline UI element.
        /// </summary>
        public OM_Timeline<T, TTrack> Timeline { get; }

        /// <summary>
        /// The visual element representing the clip itself within this track row.
        /// </summary>
        public OM_TrackClip<T, TTrack> TrackClip { get; private set; }

        /// <summary>
        /// The underlying data clip associated with this track.
        /// </summary>
        public T Clip { get; }

        /// <summary>
        /// A visual element potentially used to display a fill/progress bar within the track.
        /// </summary>
        public VisualElement FillContainer { get; private set; }

        //--------------- Properties ---------------//

        /// <summary>
        /// Gets a value indicating whether this track (or its clip/handles) is currently being dragged.
        /// </summary>
        public bool IsDragging { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether this track is currently selected.
        /// </summary>
        public bool IsSelected { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether this track's associated clip is active/enabled.
        /// </summary>
        public bool IsActive { get; protected set; }

        //--------------- Fields ---------------//

        /// <summary>
        /// Stores the starting position of the mouse when a drag operation begins on the track clip.
        /// </summary>
        private Vector2 _dragStartPosition;

        /// <summary>
        /// Stores the start time of the clip when a drag operation begins.
        /// </summary>
        private float _dragStartTime;

        /// <summary>
        /// Reference to the visual snapping line displayed during drag operations.
        /// </summary>
        private OM_SnappingLine _snappingLine;

        /// <summary>
        /// Gets the icon associated with the clip type for display on the track.
        /// Derived classes must implement this.
        /// </summary>
        /// <returns>A Texture2D icon.</returns>
        public abstract Texture2D GetClipIcon();

        /// <summary>
        /// Gets the shared snapping line visual element from the timeline body.
        /// </summary>
        /// <returns>The <see cref="OM_SnappingLine"/> instance.</returns>
        public OM_SnappingLine GetSnappingLine()
        {
            // Lazily initialize the snapping line reference if needed
            return _snappingLine ??= Timeline.Body.SnappingLine;
        }

        /// <summary>
        /// Toggles the active state of the clip associated with this track.
        /// </summary>
        public void ToggleActive()
        {
            SetActive(!IsActive);
        }

        /// <summary>
        /// Sets the active state of the clip and updates the UI and underlying data.
        /// </summary>
        /// <param name="value">The desired active state.</param>
        public void SetActive(bool value)
        {
            if (IsActive == value) return; // Avoid redundant operations

            IsActive = value;
            TrackClip.OnIsActiveChanged(value); // Update visual clip state

            // Record the change for Undo/Redo functionality
            Timeline.TimelinePlayer.RecordUndo("Change Clip Active State");
            Clip.IsActive = value; // Update the underlying data clip's state
            // Potentially trigger a validation or refresh if needed
            // Timeline.TimelinePlayer.OnValidate();
        }

        /// <summary>
        /// Gets the <see cref="TrackClip"/> cast to a specific derived type <typeparamref name="TNew"/>.
        /// </summary>
        /// <typeparam name="TNew">The target type to cast the TrackClip to.</typeparam>
        /// <returns>The casted TrackClip, or null if the cast fails.</returns>
        public TNew GetTrackClip<TNew>() where TNew : OM_TrackClip<T, TTrack>
        {
            return TrackClip as TNew;
        }

        /// <summary>
        /// Gets the <see cref="Timeline"/> cast to a specific derived type <typeparamref name="TNew"/>.
        /// </summary>
        /// <typeparam name="TNew">The target type to cast the Timeline to.</typeparam>
        /// <returns>The casted Timeline, or null if the cast fails.</returns>
        public TNew GetTimeline<TNew>() where TNew : OM_Timeline<T, TTrack>
        {
            return Timeline as TNew;
        }

        /// <summary>
        /// Casts this <see cref="OM_Track{T, TTrack}"/> instance to a specific derived type <typeparamref name="TNew"/>.
        /// </summary>
        /// <typeparam name="TNew">The target type to cast this instance to.</typeparam>
        /// <returns>The casted instance, or null if the cast fails.</returns>
        public TNew As<TNew>() where TNew : OM_Track<T, TTrack>
        {
            return this as TNew;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OM_Track{T, TTrack}"/> class.
        /// </summary>
        /// <param name="timeline">The parent timeline UI element.</param>
        /// <param name="clip">The data clip associated with this track.</param>
        public OM_Track(OM_Timeline<T, TTrack> timeline, T clip)
        {
            Timeline = timeline;
            Clip = clip;
            // Call Init in the constructor or expect it to be called externally?
            // Assuming it might be called after adding to the hierarchy.
        }

        /// <summary>
        /// Initializes the visual elements and behaviour of this track.
        /// Should be called after the track is added to the timeline hierarchy.
        /// </summary>
        public virtual void Init()
        {
            name = $"OM_Track {Timeline.TracksList.Count}"; // Assign a default name
            this.SetPickingMode(PickingMode.Ignore); // Track itself doesn't receive mouse events directly
            AddToClassList("track");
            AddToClassList("no-animation"); // Initially disable animations for setup

            // Apply styling based on clip height and spacing defined in OM_TimelineUtil
            style.height = OM_TimelineUtil.ClipHeight + OM_TimelineUtil.ClipSpaceBetween;
            style.position = Position.Absolute; // Position is controlled explicitly
            style.left = 0; // Start at the left edge
            style.width = new StyleLength(new Length(100, LengthUnit.Percent)); // Span the full width

            // Create and add the fill container (used for progress or effects)
            FillContainer = new VisualElement().SetPickingMode(PickingMode.Ignore);
            FillContainer.style.position = Position.Absolute;
            FillContainer.style.width = new StyleLength(new Length(0, LengthUnit.Percent)); // Start with 0 width
            FillContainer.style.height = new StyleLength(new Length(20, LengthUnit.Percent)); // Example height
            FillContainer.style.backgroundColor = new StyleColor(new Color(0.41f, 0.17f, 0.47f, 0.68f)); // Example color
            FillContainer.style.bottom = 0; // Align to bottom
            FillContainer.SetDisplay(false); // Initially hidden
            Add(FillContainer);

            // Create the visual representation of the clip
            TrackClip = CreateAndGetClip();
            Add(TrackClip);
            TrackClip.OnIsActiveChanged(Clip.IsActive); // Update visual clip state

            // Schedule UI updates after the initial layout pass
            this.schedule.Execute(() =>
            {
                UpdateTrack(); // Perform initial update
                RemoveFromClassList("no-animation"); // Enable animations now
            });

            // Register callbacks for drag and drop operations on the TrackClip
            TrackClip.RegisterCallback<DragEnterEvent>(OnDragEnter);
            TrackClip.RegisterCallback<DragLeaveEvent>(OnDragLeave);
            TrackClip.RegisterCallback<DragUpdatedEvent>(OnDragUpdated);
            TrackClip.RegisterCallback<DragPerformEvent>(OnDragPerform);
        }

        /// <summary>
        /// Handles the DragEnter event, setting the visual mode for drag-and-drop.
        /// </summary>
        protected virtual void OnDragEnter(DragEnterEvent e)
        {
            // Check if the drag contains valid object references
            if (DragAndDrop.objectReferences == null || DragAndDrop.objectReferences.Length == 0 || DragAndDrop.objectReferences[0] == null)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Rejected; // Reject if no valid data
                return;
            }
            // Indicate that a link operation (e.g., dropping a compatible asset) is possible
            DragAndDrop.visualMode = DragAndDropVisualMode.Link;
        }

        /// <summary>
        /// Handles the DragLeave event. (Currently no specific logic)
        /// </summary>
        protected virtual void OnDragLeave(DragLeaveEvent e)
        {
            // Potential cleanup logic if needed when drag leaves the element
        }

        /// <summary>
        /// Handles the DragUpdated event, continuously updating the visual mode.
        /// </summary>
        protected virtual void OnDragUpdated(DragUpdatedEvent e)
        {
            // Keep checking if the drag contains valid object references
            if (DragAndDrop.objectReferences.Length > 0 && DragAndDrop.objectReferences[0] != null)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Link; // Allow drop
            }
            else
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Rejected; // Reject otherwise
            }
        }

        /// <summary>
        /// Handles the DragPerform event when a drop occurs.
        /// Derived classes must implement the logic to handle the dropped data.
        /// </summary>
        protected abstract void OnDragPerform(DragPerformEvent e);

        /// <summary>
        /// Sets the width of the <see cref="FillContainer"/> as a percentage.
        /// </summary>
        /// <param name="value">The percentage value (0 to 100).</param>
        public void SetFillPercent(float value)
        {
            FillContainer.style.width = new StyleLength(new Length(value, LengthUnit.Percent));
        }

        /// <summary>
        /// Updates the visual state of the entire track row, including position and width.
        /// </summary>
        public void UpdateTrack()
        {
            // Update visual details derived from the clip data
            TrackClip.UpdateDetails();
            TrackClip.UpdateTrackClip(); // Allow TrackClip to update itself

            // Only update layout if not currently being dragged (avoids conflicts)
            if (IsDragging == false)
            {
                UpdateTop();
                UpdateLeft();
                UpdateWidth();
            }
            // Hook for derived classes to perform additional updates
            OnUpdateTrack();
        }

        #region Drag Implementation

        /// <summary>
        /// Gets the tracks immediately above and below this track in the timeline.
        /// Used primarily for snapping calculations during drag operations.
        /// </summary>
        /// <returns>An enumerable collection of neighboring tracks.</returns>
        public IEnumerable<OM_Track<T, TTrack>> GetNeighbourTracks()
        {
            int currentIndex = GetTrackIndex();
            foreach (var track in Timeline.TracksList)
            {
                if (track == this) continue; // Skip self

                int trackIndex = track.GetTrackIndex();
                // Return tracks directly above or below
                if (trackIndex == currentIndex - 1 || trackIndex == currentIndex + 1)
                {
                    yield return track;
                }
            }
        }

        /// <summary>
        /// Initiates a drag operation for the track clip.
        /// Called when the user starts dragging the main body of the <see cref="TrackClip"/>.
        /// </summary>
        /// <param name="mousePosition">The current mouse position in the track's local coordinates.</param>
        public void StartDrag(Vector2 mousePosition)
        {
            // Store initial state for delta calculations
            _dragStartPosition = new Vector2(TrackClip.layout.x, layout.y); // Use layout.y for vertical position
            _dragStartTime = GetStartTime();

            BringToFront(); // Ensure the dragged track is rendered on top
            this.AddClassNames("no-animation"); // Disable animations during drag
            SetIsDragging(true); // Set the dragging flag

            TrackClip.Details.SetDisplay(true); // Show details label during drag
        }

        /// <summary>
        /// Handles the continuous dragging movement of the track clip.
        /// Updates the clip's position (both horizontally/time and vertically/order) based on mouse delta.
        /// Includes snapping logic.
        /// </summary>
        /// <param name="delta">The change in mouse position since the last update.</param>
        /// <param name="mousePosition">The current mouse position.</param>
        public void Drag(Vector2 delta, Vector2 mousePosition)
        {
            // Calculate potential new horizontal position in seconds
            var newXInSeconds = (_dragStartPosition.x + delta.x) / Timeline.GetPixelPerSecond();
            // Calculate potential new vertical position
            var newY = _dragStartPosition.y + delta.y; // Use stored Y

            // Clamp horizontal position within timeline bounds, considering clip duration
            newXInSeconds = Mathf.Clamp(newXInSeconds, 0, Timeline.TimelinePlayer.GetTimelineDuration() - GetDuration());
            // Clamp vertical position within the parent container bounds
            newY = Mathf.Clamp(newY, 0, parent.layout.height - layout.height);

            // Apply snapping logic to the horizontal position (time)
            HandleSnapping(ref newXInSeconds);

            // Determine the new track order index based on the vertical position
            var currentOrderIndex = GetTrackIndex();
            var newIndex = GetIndexOfClipBasedOnPosition(Timeline, newY);

            // If the order index has changed, move the track
            if (newIndex != currentOrderIndex)
            {
                MoveTrack(newIndex);
            }

            // Apply the calculated visual position
            style.top = newY;
            // Set the logical start time and update clip details
            SetStartTime(newXInSeconds); // This updates the underlying Clip data and visuals
            TrackClip.UpdateDetails(); // Refresh the details label
        }

        /// <summary>
        /// Applies snapping logic to the proposed horizontal position (time) during dragging.
        /// Modifies the <paramref name="newXInSeconds"/> value if it's close to snapping points
        /// (start/end times of the clip itself or neighboring clips).
        /// </summary>
        /// <param name="newXInSeconds">The proposed new start time in seconds (passed by reference).</param>
        private void HandleSnapping(ref float newXInSeconds)
        {
            // Only snap if enabled in settings
            if (!OM_TimelineSettings.Instance.UseSnapping) return;

            // Calculate the snapping range in seconds based on pixel setting
            var snapRange = OM_TimelineSettings.Instance.SnappingValue / Timeline.GetPixelPerSecond();
            float duration = GetDuration(); // Cache duration

            // --- Snap to Timeline Boundaries ---
            // Snap start time to timeline start (0)
            if (Mathf.Abs(newXInSeconds) < snapRange)
            {
                 newXInSeconds = 0;
                 GetSnappingLine().SetPosition(true, 0, layout.center.y); // Show snap line at timeline start
                 return; // Snapped, no need to check neighbors
            }
             // Snap end time to timeline end
            float timelineDuration = Timeline.TimelinePlayer.GetTimelineDuration();
            if (Mathf.Abs(newXInSeconds + duration - timelineDuration) < snapRange)
            {
                 newXInSeconds = timelineDuration - duration;
                 GetSnappingLine().SetPosition(true, timelineDuration * Timeline.GetPixelPerSecond(), layout.center.y); // Show snap line at timeline end
                 return; // Snapped
            }


            // --- Snap to Neighboring Clips ---
            foreach (var neighbourTrack in GetNeighbourTracks())
            {
                float neighbourStart = neighbourTrack.GetStartTime();
                float neighbourEnd = neighbourTrack.GetEndTime();
                float neighbourLayoutX = neighbourTrack.TrackClip.layout.x;
                float neighbourLayoutXMax = neighbourLayoutX + neighbourTrack.TrackClip.layout.width;

                // Snap Start -> Neighbour Start
                if (OM_Utility.IsWithinRange(newXInSeconds, neighbourStart, snapRange))
                {
                    newXInSeconds = neighbourStart;
                    GetSnappingLine().SetPosition(true, neighbourLayoutX, layout.center.y);
                    GetSnappingLine().SetFromTo(layout.center, neighbourTrack.layout.center);
                    return; // Snapped
                }

                // Snap End -> Neighbour End
                if (OM_Utility.IsWithinRange(newXInSeconds + duration, neighbourEnd, snapRange))
                {
                    newXInSeconds = neighbourEnd - duration;
                    GetSnappingLine().SetPosition(true, neighbourLayoutXMax, layout.center.y);
                    GetSnappingLine().SetFromTo(layout.center, neighbourTrack.layout.center);
                    return; // Snapped
                }

                 // Snap Start -> Neighbour End
                if (OM_Utility.IsWithinRange(newXInSeconds, neighbourEnd, snapRange))
                {
                    newXInSeconds = neighbourEnd;
                    GetSnappingLine().SetPosition(true, neighbourLayoutXMax, layout.center.y);
                     GetSnappingLine().SetFromTo(layout.center, neighbourTrack.layout.center);
                    return; // Snapped
                 }

                // Snap End -> Neighbour Start
                if (OM_Utility.IsWithinRange(newXInSeconds + duration, neighbourStart, snapRange))
                {
                    newXInSeconds = neighbourStart - duration;
                    GetSnappingLine().SetPosition(true, neighbourLayoutX, layout.center.y); // Use neighbour's start X
                    GetSnappingLine().SetFromTo(layout.center, neighbourTrack.layout.center);
                    return; // Snapped
                 }
            }

            // If no snapping occurred, hide the snapping line
            GetSnappingLine().SetPosition(false, Vector2.zero);
        }


        /// <summary>
        /// Finalizes a drag operation for the track clip.
        /// </summary>
        /// <param name="delta">The total change in mouse position during the drag.</param>
        /// <param name="mousePosition">The final mouse position.</param>
        public void EndDrag(Vector2 delta, Vector2 mousePosition)
        {
            SetIsDragging(false); // Clear the dragging flag
            this.RemoveFromClassList("no-animation"); // Re-enable animations
            UpdateTrack(); // Ensure final visual state is correct
            GetSnappingLine().SetPosition(false, Vector2.zero); // Hide snapping line
            TrackClip.Details.SetDisplay(false); // Hide details label
             // Potentially trigger timeline validation after drag
             // Timeline.TimelinePlayer.OnValidate();
        }

        /// <summary>
        /// Handles mouse clicks on the track clip body.
        /// </summary>
        /// <param name="mouseButton">The mouse button that was clicked.</param>
        /// <param name="mouseUpEvent">The original mouse up event.</param>
        /// <param name="mousePosition">The mouse position within the element.</param>
        public void Click(MouseButton mouseButton, MouseUpEvent mouseUpEvent, Vector2 mousePosition)
        {
            switch (mouseButton)
            {
                case MouseButton.LeftMouse:
                    // If already selected, deselect (select null). Otherwise, select this track.
                    if (Timeline.SelectedTrack == this)
                    {
                        Timeline.SelectTrack(null);
                    }
                    else
                    {
                        Timeline.SelectTrack(this);
                    }
                    break;
                case MouseButton.RightMouse:
                    // Show the context menu for this track
                    DrawContextMenu();
                    break;
            }
        }

        #endregion

        /// <summary>
        /// Calculates the target track order index based on a vertical position.
        /// </summary>
        /// <param name="timeline">The parent timeline.</param>
        /// <param name="newY">The vertical position (typically mouse Y).</param>
        /// <returns>The calculated target index.</returns>
        private static int GetIndexOfClipBasedOnPosition(OM_Timeline<T, TTrack> timeline, float newY)
        {
            // Adjust Y position to be relative to the center of the potential track slot
            newY += OM_TimelineUtil.ClipHeight / 2;
            // Calculate index based on total height of a track slot
            var index = Mathf.FloorToInt(newY / (OM_TimelineUtil.ClipHeight + OM_TimelineUtil.ClipSpaceBetween));
            // Clamp index within the valid range of existing tracks
            index = Mathf.Clamp(index, 0, timeline.TracksList.Count - 1);
            return index;
        }

        /// <summary>
        /// Sets the dragging state visually on the track clip.
        /// </summary>
        /// <param name="value">True if dragging, false otherwise.</param>
        public void SetIsDragging(bool value)
        {
            if (IsDragging == value) return; // Avoid redundant updates
            IsDragging = value;

            // Add/remove class for potential CSS styling during drag
            if (value) this.AddToClassList("no-animation");
            else this.RemoveFromClassList("no-animation");

            // Notify the visual clip representation
            TrackClip.OnIsDraggingChanged(value);
        }

        /// <summary>
        /// Sets the selected state visually on the track clip.
        /// </summary>
        /// <param name="value">True if selected, false otherwise.</param>
        public void SetIsSelected(bool value)
        {
            if (IsSelected == value) return; // Avoid redundant updates
            IsSelected = value;
            // Notify the visual clip representation
            TrackClip.OnIsSelectedChanged(value);
        }

        /// <summary>
        /// Moves this track to a new order index within the timeline.
        /// </summary>
        /// <param name="newIndex">The target order index.</param>
        public virtual void MoveTrack(int newIndex)
        {
            // Delegate the actual move operation to the timeline
            Timeline.MoveTrack(this, newIndex);
            // Hook for derived classes
            OnMoveTrack(newIndex);
        }

        /// <summary>
        /// Virtual hook called after the track has been moved to a new index.
        /// </summary>
        /// <param name="newIndex">The new index of the track.</param>
        protected virtual void OnMoveTrack(int newIndex)
        {
            // Base implementation does nothing. Derived classes can override.
        }

        /// <summary>
        /// Gets the start time of the associated clip.
        /// </summary>
        public virtual float GetStartTime() => Clip.GetStartTime();

        /// <summary>
        /// Gets the duration of the associated clip.
        /// </summary>
        public virtual float GetDuration() => Clip.GetDuration();

        /// <summary>
        /// Gets the end time of the associated clip.
        /// </summary>
        public virtual float GetEndTime() => Clip.GetEndTime();

        /// <summary>
        /// Gets the order index of the associated clip.
        /// </summary>
        public virtual int GetTrackIndex() => Clip.OrderIndex;

        /// <summary>
        /// Sets the duration of the associated clip and updates visuals.
        /// </summary>
        /// <param name="newDuration">The new duration in seconds.</param>
        public virtual void SetDuration(float newDuration)
        {
            // Update visual width based on new duration
            TrackClip.style.width = newDuration * Timeline.GetPixelPerSecond();
            // Record Undo
            Timeline.TimelinePlayer.RecordUndo("Set Duration");
            // Update data clip
            Clip.SetDuration(newDuration);
            // Notify timeline that a transform property changed (affects layout/snapping)
            Timeline.OnTrackTransformChanged(this);
             // Potentially trigger validation
             // Timeline.TimelinePlayer.OnValidate();
        }

        /// <summary>
        /// Sets the start time of the associated clip and updates visuals.
        /// </summary>
        /// <param name="newStartAt">The new start time in seconds.</param>
        public virtual void SetStartTime(float newStartAt)
        {
            // Update visual left position based on new start time
            TrackClip.style.left = newStartAt * Timeline.GetPixelPerSecond();
            // Record Undo
            Timeline.TimelinePlayer.RecordUndo("Set Start Time");
            // Update data clip
            Clip.SetStartTime(newStartAt);
            // Notify timeline that a transform property changed
            Timeline.OnTrackTransformChanged(this);
            // Potentially trigger validation
            // Timeline.TimelinePlayer.OnValidate();
        }

        /// <summary>
        /// Updates the visual width of the <see cref="TrackClip"/> based on its current duration.
        /// </summary>
        public virtual void UpdateWidth()
        {
            TrackClip.style.width = GetDuration() * Timeline.GetPixelPerSecond();
        }

        /// <summary>
        /// Updates the visual top position of the track based on its order index.
        /// </summary>
        public virtual void UpdateTop()
        {
            style.top = GetTrackIndex() * (OM_TimelineUtil.ClipHeight + OM_TimelineUtil.ClipSpaceBetween);
        }

        /// <summary>
        /// Updates the visual left position of the <see cref="TrackClip"/> based on its start time.
        /// </summary>
        public virtual void UpdateLeft()
        {
            TrackClip.style.left = GetStartTime() * Timeline.GetPixelPerSecond();
        }

        /// <summary>
        /// Draws the context menu for this track.
        /// </summary>
        protected virtual void DrawContextMenu()
        {
            var menu = new GenericMenu();
            // Add standard menu items
            menu.AddItem(new GUIContent("Delete"), false, () => { Timeline.DeleteTrack(this); });
            menu.AddItem(new GUIContent("Duplicate"), false, () => { Timeline.DuplicateTrack(this); });
            menu.AddItem(new GUIContent("Copy"), false, () => { Timeline.CopyTrack(this); });
            menu.AddItem(new GUIContent("Toggle Active"), false, ToggleActive); // Use the ToggleActive method
            menu.AddItem(new GUIContent("Solo"), false, () => { Timeline.SoloTrack(this); });

            // Allow derived classes or specific timeline implementations to add more items
            OnContextMenu(ref menu);

            // Show the menu
            menu.ShowAsContext();
        }

        /// <summary>
        /// Virtual hook to allow adding custom items to the track's context menu.
        /// </summary>
        /// <param name="menu">The <see cref="GenericMenu"/> instance (passed by reference).</param>
        protected virtual void OnContextMenu(ref GenericMenu menu) { /* Base implementation does nothing */ }

        /// <summary>
        /// Called when this track is about to be deleted.
        /// Removes the associated clip from the timeline player data.
        /// </summary>
        public virtual void OnTrackDeleted()
        {
            // Ensure the corresponding clip data is removed from the runtime player
            Timeline.TimelinePlayer.RemoveClip(Clip);
             // Potentially trigger validation
             // Timeline.TimelinePlayer.OnValidate();
        }

        /// <summary>
        /// Called when this track is duplicated.
        /// Duplicates the associated clip in the timeline player data.
        /// </summary>
        public virtual void OnDuplicated()
        {
            // Ensure the corresponding clip data is duplicated in the runtime player
            Timeline.TimelinePlayer.DuplicateClip(Clip);
             // Potentially trigger validation
             // Timeline.TimelinePlayer.OnValidate();
        }

        /// <summary>
        /// Called frequently to update the visual representation based on the <see cref="Clip"/> data.
        /// Updates tooltip, title, color line, active state, and error icon.
        /// </summary>
        protected virtual void OnUpdateTrack()
        {
            TrackClip.tooltip = Clip.ClipDescription; // Set tooltip from clip description
            TrackClip.UpdateTitle(Clip.ClipName); // Update title label
            TrackClip.ColoredLine.SetColor(Clip.HighlightColor); // Update color line
            this.SetActive(Clip.IsActive); // Update active state visuals

            // Check for errors in the clip data and update the error icon accordingly
            if (Clip.IsActive && Clip.HasError(out var error))
            {
                TrackClip.UpdateErrorIcon(true, error); // Show error icon with tooltip
            }
            else
            {
                TrackClip.UpdateErrorIcon(false); // Hide error icon
            }

            // Allow the visual TrackClip element to perform its own updates
            TrackClip.OnTrackUpdate();
        }

        /// <summary>
        /// Creates and initializes the visual <see cref="OM_TrackClip{T, TTrack}"/> element for this track.
        /// </summary>
        /// <returns>The created and initialized <see cref="OM_TrackClip{T, TTrack}"/> instance.</returns>
        protected virtual OM_TrackClip<T, TTrack> CreateAndGetClip()
        {
             // Default implementation creates a standard OM_TrackClip
            var trackClipInstance = new OM_TrackClip<T, TTrack>(this);
            trackClipInstance.Init(); // Initialize the created clip element
            return trackClipInstance;
            // Derived classes might override this to create custom TrackClip visuals
        }

        /// <summary>
        /// Determines if this track's clip duration (width) can be controlled by handles.
        /// </summary>
        /// <returns>True if width control is allowed, false otherwise. Default is true.</returns>
        public virtual bool CanControlWidth()
        {
            return true; // By default, allow duration control via handles
        }

        /// <summary>
        /// Virtual hook called when the timeline's preview state changes.
        /// </summary>
        /// <param name="isPreviewing">True if preview mode is active, false otherwise.</param>
        public virtual void OnPreviewStateChanged(bool isPreviewing) { /* Base implementation does nothing */ }

        /// <summary>
        /// Virtual hook called when the timeline player's play state changes.
        /// </summary>
        /// <param name="newState">The new <see cref="OM_PlayState"/>.</param>
        public virtual void OnPlayStateChanged(OM_PlayState newState) { /* Base implementation does nothing */ }

        /// <summary>
        /// Virtual hook called when the icon within the <see cref="TrackClip"/> is clicked.
        /// </summary>
        public virtual void OnIconClicked() { /* Base implementation does nothing */ }
    }
}