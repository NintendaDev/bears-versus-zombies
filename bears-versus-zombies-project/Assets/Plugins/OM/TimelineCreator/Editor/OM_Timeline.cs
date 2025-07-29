using System;
using System.Collections.Generic;
using System.Linq;
using OM.Editor;
using OM.TimelineCreator.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.TimelineCreator.Editor
{
    /// <summary>
    /// Abstract base class representing the main Timeline Editor UI control.
    /// It orchestrates the header, body (containing tracks), and footer sections.
    /// Manages track creation, deletion, selection, duplication, copy/paste,
    /// preview mode, time cursor updates, and communication with the underlying
    /// <see cref="IOM_TimelinePlayer{T}"/> data representation.
    /// </summary>
    /// <typeparam name="T">The type of the data clip, derived from <see cref="OM_ClipBase"/>.</typeparam>
    /// <typeparam name="TTrack">The concrete type of the track UI element, derived from <see cref="OM_Track{T, TTrack}"/>.</typeparam>
    public abstract class OM_Timeline<T, TTrack> : VisualElement,
        IOM_ValidateListener, // Responds to player validation events
        IOM_UndoRedoListener // Responds to Undo/Redo events
        where T : OM_ClipBase
        where TTrack : OM_Track<T, TTrack>
    {
        /// <summary>
        /// Event triggered when the currently selected track changes.
        /// The parameter is the newly selected track, or null if deselected.
        /// </summary>
        public event Action<OM_Track<T, TTrack>> OnSelectedTrackChanged;
        /// <summary>
        /// Event triggered when the preview state (active or inactive) changes.
        /// The boolean parameter indicates if preview mode is now active.
        /// </summary>
        public event Action<bool> OnPreviewStateChangedCallback;

        /// <summary>
        /// Gets the header UI element of the timeline.
        /// </summary>
        public OM_TimelineHeader<T, TTrack> Header { get; protected set; }
        /// <summary>
        /// Gets the body UI element of the timeline, which contains the tracks.
        /// </summary>
        public OM_TimelineBody<T, TTrack> Body { get; protected set; }
        /// <summary>
        /// Gets the footer UI element of the timeline.
        /// </summary>
        public OM_TimelineFooter<T, TTrack> Footer { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether this timeline instance supports preview mode scrubbing.
        /// Can be overridden by derived classes. Default implementation returns true.
        /// </summary>
        public virtual bool CanBePreviewed { get; } = true; // Default to true

        /// <summary>
        /// Gets the set of all track UI elements currently present in the timeline body.
        /// Using HashSet for efficient add/remove/contains operations.
        /// </summary>
        public HashSet<OM_Track<T, TTrack>> TracksList { get; }
        /// <summary>
        /// Gets the currently selected track UI element. Can be null if no track is selected.
        /// </summary>
        public OM_Track<T, TTrack> SelectedTrack { get; protected set; }

        /// <summary>
        /// Gets the timeline cursor UI element.
        /// </summary>
        public OM_TimelineCursor<T, TTrack> TimelineCursor { get; protected set; }
        /// <summary>
        /// Gets the owner object providing editor context (e.g., the EditorWindow or CustomEditor).
        /// </summary>
        public IOM_TimelineEditorOwner<T> TimelineEditorOwner { get; protected set; }
        /// <summary>
        /// Gets the underlying runtime player object that holds the actual timeline data.
        /// </summary>
        public IOM_TimelinePlayer<T> TimelinePlayer { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the timeline is currently in preview scrubbing mode.
        /// </summary>
        public bool IsPreviewing { get; private set; } = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="OM_Timeline{T, TTrack}"/> class.
        /// </summary>
        /// <param name="timelineEditorOwner">The owner providing editor context.</param>
        /// <param name="timelinePlayer">The runtime player holding the data.</param>
        /// <param name="canBePreviewed">Whether this timeline instance supports preview scrubbing.</param>
        public OM_Timeline(IOM_TimelineEditorOwner<T> timelineEditorOwner, IOM_TimelinePlayer<T> timelinePlayer, bool canBePreviewed = true)
        {
            TimelineEditorOwner = timelineEditorOwner;
            TimelinePlayer = timelinePlayer;
            CanBePreviewed = canBePreviewed; // Set preview capability

            // Add this timeline instance to the owner's manager for potential tracking/access
            TimelineEditorOwner.VisualElementsManager.AddElement(this);

            // Initialize the set to hold track UI elements
            TracksList = new HashSet<OM_Track<T, TTrack>>();

            // Load and add the main stylesheet for the timeline
            styleSheets.Add(Resources.Load<StyleSheet>("OM_Timeline"));
            // Assign a name for identification and potential USS targeting
            name = "OM_Timeline";

            // Add the manipulator responsible for handling drag operations (like scrubbing the header)
            // Assumes OM_DragControlManipulator exists and handles routing drag events
            this.AddManipulator(new OM_DragControlManipulator());

            // --- Subscribe to Player Events ---
            // When clips are added/removed in the player data, update the UI
            timelinePlayer.OnClipAddedOrRemoved += TriggerUpdateClipsCreateAndDelete;
            // When the player's elapsed time changes (during runtime playback), update the cursor
            timelinePlayer.OnElapsedTimeChangedCallback += OnElapsedTimeChanged;
            // When the player's state changes (Playing, Paused, Stopped), notify tracks
            timelinePlayer.OnPlayStateChanged += OnPlayStateChanged;
            // When the player requests an editor refresh (e.g., after loading data), update UI
            timelinePlayer.OnTriggerEditorRefresh += OnTriggerEditorRefresh;

            // --- Subscribe to Editor Events ---
            // When Unity's play mode state changes, potentially stop preview
            EditorApplication.playModeStateChanged += OnplayModeStateChanged;
            // When the editor is quitting, stop preview
            EditorApplication.quitting += OnQuitting;


            // Initialize the main UI components (Header, Body, Footer, Cursor)
            Init();
            // Perform an initial synchronization between player data and UI tracks
            TriggerUpdateClipsCreateAndDelete();
            // Select the track corresponding to the player's selected clip index (if any)
            SelectTrack(TracksList.FirstOrDefault(x => x.Clip.OrderIndex == TimelinePlayer.GetSelectedClipIndex()));
        }

        /// <summary>
        /// Handles the <see cref="IOM_TimelinePlayer{T}.OnTriggerEditorRefresh"/> event.
        /// Triggers a full UI update based on the player data.
        /// </summary>
        protected virtual void OnTriggerEditorRefresh()
        {
            TriggerUpdateClipsCreateAndDelete();
        }

        /// <summary>
        /// Handles the <see cref="IOM_TimelinePlayer{T}.OnPlayStateChanged"/> event.
        /// Propagates the state change to all track UI elements.
        /// </summary>
        /// <param name="newState">The new playback state.</param>
        protected virtual void OnPlayStateChanged(OM_PlayState newState)
        {
            // Notify each track about the state change
            foreach (var track in TracksList)
            {
                track.OnPlayStateChanged(newState);
            }
        }

        /// <summary>
        /// Handles the <see cref="IOM_TimelinePlayer{T}.OnElapsedTimeChangedCallback"/> event.
        /// Updates the timeline cursor's position based on the new elapsed time.
        /// </summary>
        /// <param name="time">The new elapsed time in seconds.</param>
        private void OnElapsedTimeChanged(float time)
        {
            // Update the visual cursor position based on the time
            SetCursorTime(time);
        }

        /// <summary>
        /// Initializes the core UI components of the timeline: Header, Body, Footer, and Cursor.
        /// Also sets up keyboard event handling and necessary callbacks.
        /// </summary>
        public void Init()
        {
            // Initialize the player for editor use (e.g., load clips if needed)
            TimelinePlayer.InitPlayerForEditor();

            // Create Header, Body, and Footer components
            Header = new OM_TimelineHeader<T, TTrack>(this);
            Body = new OM_TimelineBody<T, TTrack>(this);
            Footer = new OM_TimelineFooter<T, TTrack>(this);

            // Add components to the timeline's visual tree
            Add(Header);
            Add(Body);
            Add(Footer);

            // Create and add the timeline cursor
            TimelineCursor = new OM_TimelineCursor<T, TTrack>(this);
            Add(TimelineCursor);

            // Register for keyboard events on the timeline element
            this.RegisterCallback<KeyDownEvent>(OnKeyDownEvent);
            // Make the timeline focusable to receive keyboard events
            this.focusable = true;


            // Register callback for when the body's geometry changes (e.g., resizing)
            Body.RegisterCallback<GeometryChangedEvent>(e =>
            {
                // Recalculate cursor position and height after layout changes
                TimelineCursor.UpdatePosition(IsPreviewing ? TimelineCursor.layout.x : 0, Header.layout.height - Header.Container2.layout.height);
                TimelineCursor.UpdateHeight(Body.layout.height + Header.Container2.layout.height); // Adjust height based on body + ruler
                // Refresh visual state of all tracks
                RefreshAllTracks();
            });

            // Register callback for when the timeline element is detached from the panel (e.g., window closed)
            this.RegisterCallback<DetachFromPanelEvent>(e =>
            {
                // Ensure preview mode is stopped and editor event subscriptions are cleaned up
                StopPreview();
                EditorApplication.playModeStateChanged -= OnplayModeStateChanged;
                EditorApplication.quitting -= OnQuitting;
            });
        }

        /// <summary>
        /// Handles the <see cref="EditorApplication.quitting"/> event.
        /// Ensures preview mode is stopped when the editor is closing.
        /// </summary>
        protected virtual void OnQuitting()
        {
            StopPreview();
        }

        /// <summary>
        /// Handles the <see cref="EditorApplication.playModeStateChanged"/> event.
        /// Ensures preview mode is stopped when entering or exiting play mode.
        /// </summary>
        /// <param name="playModeStateChange">The type of state change.</param>
        protected virtual void OnplayModeStateChanged(PlayModeStateChange playModeStateChange)
        {
            // Stop preview regardless of the specific state change (entering/exiting play mode)
            StopPreview();
        }

        /// <summary>
        /// Sets the horizontal position (left offset) of the timeline cursor visually.
        /// Clamps the position within the bounds of the time ruler area.
        /// </summary>
        /// <param name="left">The desired left offset in pixels.</param>
        public virtual void SetCursorLeft(float left)
        {
            // Clamp the horizontal position within the visible width of the time ruler (Header.Container2)
            left = Mathf.Clamp(left, 0, Header.Container2.layout.width);
            // Update the cursor's visual position and height
            TimelineCursor.UpdatePosition(left, Header.layout.height - Header.Container2.layout.height); // Y position relative to header
            TimelineCursor.UpdateHeight(Body.layout.height + Header.Container2.layout.height); // Update height to span body + ruler
        }

        /// <summary>
        /// Sets the timeline cursor's position based on a time value.
        /// Converts time to pixels using <see cref="GetPixelPerSecond"/>.
        /// </summary>
        /// <param name="time">The time in seconds.</param>
        public virtual void SetCursorTime(float time)
        {
            // Convert time to horizontal pixel position and set the cursor
            SetCursorLeft(time * GetPixelPerSecond());
        }

        /// <summary>
        /// Adds a new track UI element to the timeline body.
        /// </summary>
        /// <param name="track">The track UI element to add.</param>
        public virtual void AddTrack(OM_Track<T, TTrack> track)
        {
            // Add to the internal list
            TracksList.Add(track);
            // Add visually to the body container
            Body.AddTrack(track);
            // Allow derived classes to react
            OnAddTrack(track);
            // Refresh visuals of all tracks (e.g., for potential layout changes)
            RefreshAllTracks();
        }

        /// <summary>
        /// Removes a track UI element from the timeline body.
        /// </summary>
        /// <param name="track">The track UI element to remove.</param>
        public virtual void RemoveTrack(OM_Track<T, TTrack> track)
        {
            // Remove from the internal list
            TracksList.Remove(track);
            // Remove visually from the body container
            Body.RemoveTrack(track);
            // Allow derived classes to react
            OnRemoveTrack(track);
             // Refresh visuals of all tracks
            RefreshAllTracks();
        }

        /// <summary>
        /// Removes all track UI elements from the timeline body.
        /// </summary>
        public virtual void ClearTracks()
        {
            Body.ClearTracks(); // Clear visually
            TracksList.Clear(); // Clear internal list
            OnClearTracks(); // Allow derived classes to react
        }

        /// <summary>
        /// Refreshes the visual state of all track UI elements currently in the timeline.
        /// Calls <see cref="OM_Track{T, TTrack}.UpdateTrack"/> on each track.
        /// </summary>
        public virtual void RefreshAllTracks()
        {
            // Update each track individually
            foreach (var track in TracksList)
            {
                track.UpdateTrack();
            }
            // Allow derived classes to perform additional refresh logic
            OnRefreshAllTracks();
        }

        /// <summary>
        /// Calculates the number of pixels that represent one second on the timeline.
        /// Based on the width of the track container and the total timeline duration.
        /// </summary>
        /// <returns>Pixels per second, or 0 if duration is invalid.</returns>
        public virtual float GetPixelPerSecond()
        {
            float duration = GetTimelineDuration();
            // Prevent division by zero if duration is invalid or container has no width yet
            if (duration <= 0 || Body.TracksContainer.layout.width <= 0)
                return 0; // Or a default value?
            return Body.TracksContainer.layout.width / duration;
        }

        /// <summary>
        /// Displays the main context menu for the timeline (usually triggered by right-clicking the body).
        /// Includes options like "Add Track", "Paste", "Clamp Timeline".
        /// </summary>
        /// <param name="mousePosition">The position where the right-click occurred (used for menu placement).</param>
        public void ShowContextMenu(Vector2 mousePosition)
        {
            var menu = new GenericMenu();
            // Add Track option
            menu.AddItem(new GUIContent("Add Track"), false, OnAddTrackClicked);

            // Paste Track option (enabled only if pasting is possible)
            if (CanPaste())
            {
                menu.AddItem(new GUIContent("Paste"), false, PasteTrack);
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Paste"));
            }

            // Separator
            menu.AddSeparator("");

            // Clamp Timeline option
            menu.AddItem(new GUIContent("Clamp Timeline"), false, () =>
            {
                // Find the maximum end time among all clips
                 if (!TracksList.Any()) return; // Do nothing if no tracks
                var maxDuration = TracksList.Max(x => x.GetEndTime());
                // Record Undo
                TimelinePlayer.RecordUndo("Clamp Timeline");
                // Set player duration to the calculated maximum
                TimelinePlayer.SetTimelineDuration(maxDuration);
                 // Trigger validation to update UI (e.g., duration field)
                TimelinePlayer.OnValidate();
                RefreshAllTracks(); // Refresh tracks as duration changed scale
            });

            // Allow derived classes or specific implementations to add more context items
            OnContextMenu(ref menu);

            // Show the menu at the cursor position
            menu.ShowAsContext();
        }

        /// <summary>
        /// Abstract method that must be implemented by derived classes to handle the "Add Track" action.
        /// This method should typically create a new clip data instance, add it to the player,
        /// and the <see cref="TriggerUpdateClipsCreateAndDelete"/> mechanism will handle creating the UI.
        /// </summary>
        public abstract void OnAddTrackClicked();

        /// <summary>
        /// Selects a specific track UI element. Handles deselection of the previous track.
        /// </summary>
        /// <param name="index">The order index of the clip associated with the track to select.</param>
        public virtual void SelectTrack(int index)
        {
            // Find the track corresponding to the clip index
            var track = TracksList.FirstOrDefault(x => x.Clip.OrderIndex == index);
            // Delegate to the main SelectTrack method
            SelectTrack(track);
        }

        /// <summary>
        /// Selects a specific track UI element. Handles deselection of the previous track.
        /// Updates the selected state visually and notifies listeners.
        /// </summary>
        /// <param name="track">The track UI element to select, or null to deselect all.</param>
        public virtual void SelectTrack(OM_Track<T, TTrack> track)
        {
            // Do nothing if the selection hasn't actually changed
            if (SelectedTrack == track) return;

            // Deselect the previously selected track, if any
            SelectedTrack?.SetIsSelected(false);

            // Set the new selected track
            SelectedTrack = track;

            // Select the new track, if it's not null
            SelectedTrack?.SetIsSelected(true);

            // Notify listeners about the selection change
            OnSelectedTrackChanged?.Invoke(SelectedTrack);

            // Record Undo for the selection change action
            TimelinePlayer.RecordUndo("Select Track");
            // Update the player's selected clip index
            TimelinePlayer.SetSelectedClipIndex(SelectedTrack?.Clip.OrderIndex ?? -1); // Use -1 if null
            // Trigger validation in the player (might update inspector or other UI)
            TimelinePlayer.OnValidate();

            // Optional: Notify the editor owner about the selection change
            //TimelineOwner.TriggerOnSelectedTrackChanged(SelectedTrack);
        }

        /// <summary>
        /// Deletes a specified track UI element and its associated clip data.
        /// </summary>
        /// <param name="trackToDelete">The track UI element to delete.</param>
        public void DeleteTrack(OM_Track<T, TTrack> trackToDelete)
        {
            if (trackToDelete == null) return; // Do nothing if null

            // Notify the track it's being deleted (allows cleanup)
            trackToDelete.OnTrackDeleted();

            // If the deleted track was the selected one, deselect it
            if (SelectedTrack == trackToDelete)
            {
                SelectTrack(null);
            }

            // Allow derived classes to react before removing visually
            OnTrackDeleted(trackToDelete); // Note: This calls the base virtual method first

            // Trigger the update mechanism which will remove the track visually
            // because its corresponding clip is no longer in the player data
            TriggerUpdateClipsCreateAndDelete(); // This indirectly calls RemoveTrack
        }

        /// <summary>
        /// Duplicates a specified track UI element and its associated clip data.
        /// </summary>
        /// <param name="trackToDuplicate">The track UI element to duplicate.</param>
        public virtual void DuplicateTrack(OM_Track<T, TTrack> trackToDuplicate)
        {
            if (trackToDuplicate == null) return; // Do nothing if null

            // Notify the track it's being duplicated (player handles actual data duplication)
            trackToDuplicate.OnDuplicated();

            // Select the newly created track (assuming duplication adds it after the original)
            if (SelectedTrack != null)
                SelectTrack(SelectedTrack.GetTrackIndex() + 1); // Select the expected new index

            // Trigger the update mechanism which will create the new track UI
            // based on the duplicated clip data now in the player
            TriggerUpdateClipsCreateAndDelete();
        }

        /// <summary>
        /// Selects the track UI element immediately preceding the currently selected one.
        /// </summary>
        public void SelectPreviousTrack()
        {
            // If nothing is selected, select the first track if available
            if (SelectedTrack == null)
            {
                if (TracksList.Count > 0) SelectTrack(TracksList.First());
                return;
            }

            // Get current index and check if it's already the first track
            var currentTrackIndex = SelectedTrack.GetTrackIndex();
            if (currentTrackIndex == 0) return; // Already at the top

            // Find the track with the previous index
            var previousTrack = TracksList.FirstOrDefault(x => x.GetTrackIndex() == currentTrackIndex - 1);
            if (previousTrack == null) return; // Should not happen if indices are contiguous

            // Select the found previous track
            SelectTrack(previousTrack);
        }

        /// <summary>
        /// Selects the track UI element immediately following the currently selected one.
        /// </summary>
        public void SelectNextTrack()
        {
            // If nothing is selected, select the first track if available
            if (SelectedTrack == null)
            {
                if (TracksList.Count > 0) SelectTrack(TracksList.First());
                return;
            }

            // Get current index and check if it's already the last track
            var currentTrackIndex = SelectedTrack.GetTrackIndex();
            if (currentTrackIndex == TracksList.Count - 1) return; // Already at the bottom

            // Find the track with the next index
            var nextTrack = TracksList.FirstOrDefault(x => x.GetTrackIndex() == currentTrackIndex + 1);
            if (nextTrack == null) return; // Should not happen if indices are contiguous

            // Select the found next track
            SelectTrack(nextTrack);
        }

        /// <summary>
        /// Handles keyboard events when the timeline element has focus.
        /// Implements shortcuts for selection, deletion, duplication, copy, paste, etc.
        /// </summary>
        /// <param name="e">The <see cref="KeyDownEvent"/> arguments.</param>
        protected virtual void OnKeyDownEvent(KeyDownEvent e)
        {
            // Process shortcuts based on key code and modifiers (like command/control)
            switch (e.keyCode)
            {
                case KeyCode.Escape:
                    SelectTrack(null); // Deselect current track
                    e.StopPropagation(); // Prevent event bubbling
                    break;

                case KeyCode.Space:
                     // Potentially trigger Add Track, or maybe toggle play/pause if implemented
                    // ShowContextMenu(e.originalMousePosition); // Show context menu at mouse? Risky.
                    OnAddTrackClicked(); // Default Space action: Add Track
                    e.StopPropagation();
                    break;

                // Delete/Backspace (with command/control on Mac/Win respectively, usually)
                // TODO: Check e.commandKey or e.ctrlKey based on platform if modifier is needed
                case KeyCode.Delete: // when e.commandKey: // Mac convention
                case KeyCode.Backspace: // when e.commandKey: // Mac convention
                    DeleteTrack(SelectedTrack);
                    e.StopPropagation();
                    break;

                case KeyCode.UpArrow:
                    SelectPreviousTrack();
                    e.StopPropagation();
                    break;

                case KeyCode.DownArrow:
                    SelectNextTrack();
                    e.StopPropagation();
                    break;

                 // Duplicate (Cmd/Ctrl + D)
                 case KeyCode.D when e.commandKey || e.ctrlKey: // Check for modifier
                    DuplicateTrack(SelectedTrack);
                    e.StopPropagation();
                    break;

                // Copy (Cmd/Ctrl + C)
                case KeyCode.C when e.commandKey || e.ctrlKey:
                    if(SelectedTrack != null) CopyTrack(SelectedTrack); // Copy only if track selected
                    e.StopPropagation();
                    break;

                // Paste (Cmd/Ctrl + V)
                case KeyCode.V when e.commandKey || e.ctrlKey:
                    PasteTrack();
                    e.StopPropagation();
                    break;

                 // Toggle Active (T)
                 case KeyCode.T:
                    SelectedTrack?.ToggleActive(); // Toggle active state of selected track
                    // Trigger validation after changing active state
                    //TimelineEditorOwner.OnPlayerEditorValidate(); // Might not be needed
                    TimelinePlayer.OnValidate();
                    e.StopPropagation(); // Stop propagation unless T needs to be used elsewhere
                    break;

            }
        }

        /// <summary>
        /// Called when a track's transform properties (start time, duration, order index) change.
        /// If in preview mode, updates the preview elapsed time based on the header's current time.
        /// </summary>
        /// <param name="track">The track whose transform changed.</param>
        public virtual void OnTrackTransformChanged(OM_Track<T, TTrack> track)
        {
            // If currently scrubbing preview, re-evaluate based on the header's scrub time
            if (IsPreviewing && Header != null)
            {
                UpdatePreviewElapsedTime(Header.CurrentPreviewTime);
            }
        }

        /// <summary>
        /// Virtual hook to allow derived classes to add items to the main timeline context menu.
        /// </summary>
        /// <param name="menu">The <see cref="GenericMenu"/> instance (passed by reference).</param>
        protected virtual void OnContextMenu(ref GenericMenu menu) { }

        /// <summary>
        /// Gets the total duration of the timeline from the underlying player data.
        /// </summary>
        /// <returns>The timeline duration in seconds.</returns>
        public virtual float GetTimelineDuration()
        {
            return TimelinePlayer.GetTimelineDuration();
        }

        // Method to set duration was commented out in original - uncomment if needed
        // public virtual void SetTimelineDuration(float newDuration)
        // {
        //     TimelinePlayer.SetTimelineDuration(newDuration);
        //     // Potentially call OnValidate or Refresh here
        // }

        /// <summary>
        /// Virtual hook called after a track UI element has been added visually.
        /// </summary>
        /// <param name="track">The track UI element that was added.</param>
        protected virtual void OnAddTrack(OM_Track<T, TTrack> track) { }

        /// <summary>
        /// Virtual hook called after a track UI element has been removed visually.
        /// </summary>
        /// <param name="track">The track UI element that was removed.</param>
        protected virtual void OnRemoveTrack(OM_Track<T, TTrack> track) { }

        /// <summary>
        /// Virtual hook called after all track UI elements have been cleared visually.
        /// </summary>
        protected virtual void OnClearTracks() { }

        /// <summary>
        /// Virtual hook called after all track UI elements have been refreshed visually.
        /// </summary>
        protected virtual void OnRefreshAllTracks() { }

        /// <summary>
        /// Synchronizes the track UI elements with the clip data in the <see cref="TimelinePlayer"/>.
        /// Removes track UIs for clips that no longer exist and adds track UIs for new clips.
        /// </summary>
        public virtual void TriggerUpdateClipsCreateAndDelete()
        {
            // Get the current list of data clips from the player
            var currentClips = TimelinePlayer.GetClips().ToList(); // Ensure it's a list for Contains check

            var hasChanged = false;

            // --- Remove UI tracks whose clips are no longer in the player data ---
            // Use RemoveWhere for efficient in-place removal from the HashSet
            TracksList.RemoveWhere(trackElement =>
            {
                // Check if the UI element's clip is still present in the player's current clip list
                bool shouldRemove = trackElement.Clip == null || !currentClips.Contains(trackElement.Clip);
                if (shouldRemove)
                {
                    // Remove the track visually from the timeline body
                    Body.RemoveTrack(trackElement);
                    // If the removed track was selected, deselect it
                    if (SelectedTrack == trackElement)
                    {
                        SelectTrack(null); // Deselect
                    }
                    hasChanged = true; // Mark that a change occurred
                    return true; // Indicate removal from TracksList HashSet
                }
                return false; // Keep this track element
            });

            // --- Add UI tracks for new clips found in player data ---
            foreach (var clip in currentClips)
            {
                // Check if a UI track already exists for this data clip
                if (TracksList.All(x => x.Clip != clip)) // If no existing UI track matches this clip
                {
                    // Create the specific track UI element using the abstract factory method
                    var newTrack = CreateTrack(clip);
                    // Add the newly created track UI to the timeline
                    AddTrack(newTrack); // AddTrack handles adding to TracksList and Body
                    hasChanged = true; // Mark that a change occurred
                }
            }

            // If tracks were added or removed, refresh layout-dependent elements
            if (hasChanged)
            {
                RefreshAllTracks(); // Update visual state of all remaining/new tracks
                Body.UpdateHeight(); // Adjust body height based on new track count
            }
        }

        /// <summary>
        /// Abstract factory method that must be implemented by derived classes.
        /// Creates the specific type of track UI element (<typeparamref name="TTrack"/>)
        /// corresponding to the given data clip (<typeparamref name="T"/>).
        /// </summary>
        /// <param name="clip">The data clip for which to create a track UI element.</param>
        /// <returns>A new instance of the track UI element (<typeparamref name="TTrack"/>).</returns>
        public abstract TTrack CreateTrack(T clip);

        #region Copy Paste

        /// <summary>
        /// Gets the copy/paste manager instance for the specific clip type <typeparamref name="T"/>.
        /// Used to interact with the copy buffer.
        /// </summary>
        /// <returns>The <see cref="OM_CopyPasteInstance"/>, or null if not supported.</returns>
        public virtual OM_CopyPasteInstance GetCopyPasteInstance()
        {
            // Delegate to the static manager to get the type-specific instance
            return OM_CopyPasteManager.GetCopyPasteInstance<T>();
        }

        /// <summary>
        /// Checks if pasting is currently possible for the clip type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>True if the copy buffer contains compatible data, false otherwise.</returns>
        public virtual bool CanPaste()
        {
            // Delegate to the static manager
            return OM_CopyPasteManager.CanPaste<T>();
        }

        /// <summary>
        /// Pastes the clip(s) from the copy buffer into the timeline player data.
        /// The <see cref="TriggerUpdateClipsCreateAndDelete"/> mechanism handles creating the UI.
        /// </summary>
        public virtual void PasteTrack()
        {
            // Perform the paste operation via the manager, getting the list of newly created data clips
            var clipsToPaste = OM_CopyPasteManager.Paste<T>();
            if (clipsToPaste == null || !clipsToPaste.Any()) return; // Do nothing if paste failed or buffer was empty

            // Add the newly created data clips to the timeline player
            foreach (var clipToPaste in clipsToPaste)
            {
                TimelinePlayer.AddClip(clipToPaste); // Add data clip
            }

            // If exactly one clip was pasted and a track was previously selected,
            // attempt to select the newly pasted track (assuming it appears after the original selection).
            if (clipsToPaste.Count == 1 && SelectedTrack != null)
            {
                 // Find the clip data instance that was just pasted
                 T pastedClipData = clipsToPaste.First();
                 // We need to wait for the UI update or find the new track element reliably
                 // Selecting by index might be fragile if pasting modifies indices differently.
                 // A safer approach might involve finding the track via the pastedClipData after UI update.
                 // For now, selecting index+1 is an assumption.
                 SelectTrack(SelectedTrack.GetTrackIndex() + 1); // Try selecting next index
            }

            // Trigger UI update to create visual elements for the pasted clips
            TriggerUpdateClipsCreateAndDelete();
             // Trigger validation after pasting
            TimelinePlayer.OnValidate();
        }

        /// <summary>
        /// Copies the data clip associated with the specified track UI element to the copy buffer.
        /// </summary>
        /// <param name="trackToCopy">The track UI element whose clip data should be copied.</param>
        public virtual void CopyTrack(OM_Track<T, TTrack> trackToCopy)
        {
             if (trackToCopy?.Clip == null) return; // Ensure track and clip exist
            // Delegate the copy operation to the static manager
            OM_CopyPasteManager.Copy(trackToCopy.Clip);
        }

        /// <summary>
        /// Copies all clip data currently in the timeline player to the copy buffer.
        /// </summary>
        public virtual void CopyAllTracks()
        {
            // Get all clip data from the player
            var copyObjects = TimelinePlayer.GetClips().ToList();
             if (!copyObjects.Any()) return; // Do nothing if timeline is empty
            // Delegate the list copy operation to the static manager
            OM_CopyPasteManager.CopyList(copyObjects.Cast<object>().ToList()); // Manager might expect List<object>
        }

        #endregion

        /// <summary>
        /// Handles moving a track to a new order index. Updates the underlying clip data order.
        /// </summary>
        /// <param name="track">The track UI element being moved.</param>
        /// <param name="newIndex">The target order index.</param>
        public void MoveTrack(OM_Track<T, TTrack> track, int newIndex)
        {
            // Find the clip data currently at the target index
            T clip2 = TimelinePlayer.GetClips().FirstOrDefault(x => x.OrderIndex == newIndex);
            if (clip2 == null || track.Clip == null) return; // Cannot move if target or source clip is invalid

            // Store the original index of the track being moved
            var originalIndex = track.Clip.OrderIndex;

            // Record Undo for the reordering action
            TimelinePlayer.RecordUndo("Change Clip Order");

            // --- Reorder Logic ---
            // This simple swap assumes contiguous indices and only works for adjacent swaps easily.
            // A more robust approach involves iterating through all clips and adjusting indices.

            // Simple Swap Implementation (adjust if more complex reordering is needed):
            // Set the moved clip's index to the target index
             track.Clip.OrderIndex = newIndex;
             // Set the clip originally at the target index to the moved clip's original index
             clip2.OrderIndex = originalIndex;

             // TODO: Implement robust reordering if non-adjacent moves are common.
             // Example of more robust reordering:
             // 1. Temporarily remove the moved clip from ordering considerations (e.g., set index to -1).
             // 2. Shift indices of clips between originalIndex and newIndex.
             // 3. Set the moved clip's index to newIndex.
             // This requires careful handling of iteration direction based on original vs new index.

            // After reordering data, refresh the UI track visuals
            RefreshAllTracks(); // Refresh might re-sort based on OrderIndex if implemented

            // If the moved track was the selected one, update the player's selected index (data-wise)
            if (SelectedTrack != null && track.Clip == SelectedTrack.Clip)
            {
                TimelinePlayer.SetSelectedClipIndex(newIndex);
            }
            // Trigger player validation as clip order changed
            TimelinePlayer.OnValidate();
        }

        /// <summary>
        /// Toggles the "solo" state for a specific track.
        /// If activating solo: Deactivates all other tracks.
        /// If deactivating solo (or toggling when already soloed): Reactivates all tracks.
        /// </summary>
        /// <param name="omTrack">The track to solo or unsolo.</param>
        public void SoloTrack(OM_Track<T, TTrack> omTrack)
        {
             if (omTrack == null) return;

            // Determine if the action is to activate solo mode.
            // This happens if currently *not* all other tracks are inactive (i.e., some others are active).
            var activateSolo = TracksList.Any(x => x != omTrack && x.IsActive);

            // Record Undo for the solo action
            TimelinePlayer.RecordUndo(activateSolo ? "Solo Track" : "Unsolo All Tracks");

            // Iterate through all tracks and set their active state
            foreach (var track in TracksList)
            {
                if (activateSolo)
                {
                    // Activate solo: Set active only if it's the target track
                    track.SetActive(track == omTrack);
                }
                else
                {
                    // Deactivate solo: Set all tracks active
                    track.SetActive(true);
                }
                // Update the visual representation of the track immediately
                track.UpdateTrack();
            }
             // Trigger player validation as active states changed
            TimelinePlayer.OnValidate();
        }


        /// <summary>
        /// Handles the <see cref="IOM_ValidateListener.OnPlayerValidate"/> event from the player.
        /// Refreshes all track visuals and potentially updates the preview time if active.
        /// </summary>
        public virtual void OnPlayerValidate()
        {
            RefreshAllTracks(); // Refresh UI based on potentially changed data
            // If preview mode is active, ensure the displayed preview time is still valid/updated
            if (IsPreviewing && Header != null)
            {
                UpdatePreviewElapsedTime(Header.CurrentPreviewTime);
            }
        }

        /// <summary>
        /// Handles the <see cref="IOM_UndoRedoListener.OnUndoRedoPerformed"/> event.
        /// Updates the editor state, refreshes UI tracks, re-synchronizes UI with data,
        /// and re-selects the appropriate track based on the player's current selected index.
        /// </summary>
        public virtual void OnUndoRedoPerformed()
        {
            // Update the serialized object representation in the editor owner (if applicable)
            TimelineEditorOwner.Editor.serializedObject.Update();
            // Refresh the visual state of all tracks first
            RefreshAllTracks();
            // Re-sync UI tracks with player data (handles potential add/remove from Undo/Redo)
            TriggerUpdateClipsCreateAndDelete();
            // Re-select the track corresponding to the player's current selected clip index
            SelectTrack(TimelinePlayer.GetSelectedClipIndex());
        }

        /// <summary>
        /// Checks if the timeline can enter preview mode.
        /// By default, preview is disabled during Unity Play Mode.
        /// </summary>
        /// <returns>True if preview can be entered, false otherwise.</returns>
        public virtual bool CanEnterPreview()
        {
            // Disable preview scrubbing while the Unity editor is in play mode
            return Application.isPlaying == false;
        }

        /// <summary>
        /// Enters the preview scrubbing mode.
        /// </summary>
        public void StartPreview()
        {
            // Check if preview is allowed at all
            if (!CanEnterPreview() || !CanBePreviewed) return;
            // Do nothing if already previewing
            if (IsPreviewing) return;

            IsPreviewing = true; // Set the flag
            // Invoke the state changed callback for external listeners (e.g., the preview button)
            OnPreviewStateChangedCallback?.Invoke(IsPreviewing);
            // Call the internal state changed handler
            OnPreviewStateChanged(IsPreviewing);
        }

        /// <summary>
        /// Updates the timeline state based on the elapsed time during preview scrubbing.
        /// This method should be implemented by derived classes or specific timeline types
        /// to actually evaluate the timeline at the given preview time.
        /// </summary>
        /// <param name="elapsedTime">The time (in seconds) to preview.</param>
        public virtual void UpdatePreviewElapsedTime(float elapsedTime)
        {
            // Base implementation might log or do nothing.
            // Derived classes should override this to call the appropriate
            // evaluation logic on the TimelinePlayer or related systems.
             // Example: TimelinePlayer.EvaluatePreview(elapsedTime);
             // Debug.Log("Preview Elapsed Time " + elapsedTime);
             TimelinePlayer.SetElapsedTime(elapsedTime); // Update player's logical time
             // Potentially trigger evaluation on clips based on the new time
             // AnimoraClipsPlayUtility.EvaluateForce(TimelinePlayer.GetClips(), elapsedTime); // Example call
        }

        /// <summary>
        /// Exits the preview scrubbing mode.
        /// </summary>
        public void StopPreview()
        {
            // Do nothing if not currently previewing
            if (!IsPreviewing) return;

            IsPreviewing = false; // Clear the flag
            // Invoke the state changed callback for external listeners
            OnPreviewStateChangedCallback?.Invoke(IsPreviewing);
            // Call the internal state changed handler
            OnPreviewStateChanged(IsPreviewing);
             // Optionally reset elapsed time on stop preview
             // SetCursorTime(0);
             // TimelinePlayer.SetElapsedTime(0);
        }

        /// <summary>
        /// Toggles the preview mode on or off.
        /// </summary>
        public void TogglePreview()
        {
            if (IsPreviewing) StopPreview();
            else StartPreview();
        }

        /// <summary>
        /// Internal handler called when the preview state changes (either started or stopped).
        /// Resets the cursor time and notifies all tracks of the state change.
        /// </summary>
        /// <param name="isPreviewing">True if preview mode is now active, false otherwise.</param>
        protected virtual void OnPreviewStateChanged(bool isPreviewing)
        {
            // Reset cursor to the beginning when entering or exiting preview
            SetCursorTime(0);
            // Notify each track UI element about the preview state change
            foreach (var track in TracksList)
            {
                track.OnPreviewStateChanged(isPreviewing);
            }
             // If stopping preview, potentially refresh tracks to show runtime state if applicable
             // if (!isPreviewing) RefreshAllTracks();
        }

        /// <summary>
        /// Creates the preview button specific to this timeline type.
        /// </summary>
        /// <returns>A new instance of the preview button.</returns>
        public OM_TimelinePreviewButton<T, TTrack> CreatePreviewButton()
        {
            // Instantiate the preview button, passing 'this' timeline instance
            var previewButton = new OM_TimelinePreviewButton<T, TTrack>(this);
            return previewButton;
        }

         /// <summary>
         /// Virtual hook called when a track UI element is deleted.
         /// Provides a point for derived classes to perform additional cleanup or actions
         /// *before* the track is visually removed by TriggerUpdateClipsCreateAndDelete.
         /// </summary>
         /// <param name="track">The track UI element being deleted.</param>
         protected virtual void OnTrackDeleted(OM_Track<T, TTrack> track) { } // Base implementation does nothing

    }
}