using System;
using UnityEngine;

namespace OM.TimelineCreator.Editor
{
    // === IOM_Timeline Interface ===
    /// <summary>
    /// Defines the contract for a Timeline editor system.
    /// Provides editor-related functionality such as track management,
    /// playback control, cursor interaction, and undo/redo operations.
    /// </summary>
    public interface IOM_Timeline
    {
        /// <summary>
        /// Called whenever previewing state is toggled.
        /// </summary>
        public event Action<bool> OnPreviewStateChangedCallback;

        /// <summary>
        /// Initializes the timeline and related subsystems.
        /// </summary>
        public void Init();

        /// <summary>
        /// Moves the cursor visually using pixel space (X).
        /// </summary>
        public void SetCursorLeft(float left);

        /// <summary>
        /// Moves the cursor logically using time in seconds.
        /// </summary>
        public void SetCursorTime(float time);

        /// <summary>
        /// Called each frame in preview to update timeline based on elapsed time.
        /// </summary>
        public void UpdatePreviewElapsedTime(float elapsedTime);

        /// <summary>
        /// Removes all track visuals and data.
        /// </summary>
        public void ClearTracks();

        /// <summary>
        /// Recalculates and updates all visual tracks.
        /// </summary>
        public void RefreshAllTracks();

        /// <summary>
        /// Converts time into pixel width for layout scaling.
        /// </summary>
        public float GetPixelPerSecond();

        /// <summary>
        /// Displays the right-click context menu at the given mouse position.
        /// </summary>
        public void ShowContextMenu(Vector2 mousePosition);

        /// <summary>
        /// Called when the user adds a new track.
        /// </summary>
        public void OnAddTrackClicked();

        /// <summary>
        /// Selects the previous track in the list (for keyboard nav).
        /// </summary>
        public void SelectPreviousTrack();

        /// <summary>
        /// Selects the next track in the list (for keyboard nav).
        /// </summary>
        public void SelectNextTrack();

        /// <summary>
        /// Core update function to sync clip data with visual track representation.
        /// </summary>
        public void TriggerUpdateClipsCreateAndDelete();

        /// <summary>
        /// Determines if previewing is allowed (e.g., not in Play mode).
        /// </summary>
        public bool CanEnterPreview();

        /// <summary>
        /// Called after player validate event (used to update UI).
        /// </summary>
        public void OnPlayerValidate();

        /// <summary>
        /// Called after an Undo or Redo is performed.
        /// </summary>
        public void OnUndoRedoPerformed();
    }
}