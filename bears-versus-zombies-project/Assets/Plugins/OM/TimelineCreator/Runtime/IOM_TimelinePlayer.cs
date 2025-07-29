using System.Collections.Generic;

namespace OM.TimelineCreator.Runtime 
{
    /// <summary>
    /// Interface defining the contract for a timeline playback engine.
    /// Implementors are responsible for managing clips, controlling playback state (Play, Pause, Stop),
    /// evaluating clips over time, and providing essential properties and events for editor integration
    /// and runtime interaction.
    /// </summary>
    /// <typeparam name="T">The specific type of OM_ClipBase managed by this player (e.g., AnimoraClip).</typeparam>
    public interface IOM_TimelinePlayer<T> where T : OM_ClipBase
    {
        // --- Events ---

        /// <summary>
        /// Event triggered frequently during playback, providing the current elapsed time.
        /// Useful for updating UI elements or synchronizing other systems.
        /// Parameter: float (elapsedTime)
        /// </summary>
        public event System.Action<float> OnElapsedTimeChangedCallback;

        /// <summary>
        /// Event triggered when internal data relevant to editor validation might have changed.
        /// Signals the editor or custom drawers to re-validate the player's state.
        /// </summary>
        public event System.Action OnPlayerValidateCallback;

        /// <summary>
        /// Event triggered whenever the playback state (Playing, Paused, Stopped) changes.
        /// Parameter: OM_PlayState (newState)
        /// </summary>
        public event System.Action<OM_PlayState> OnPlayStateChanged;

        /// <summary>
        /// Event triggered to signal the editor UI that it should refresh its display.
        /// Typically invoked after significant changes like loading data or structural modifications.
        /// </summary>
        public event System.Action OnTriggerEditorRefresh;

        /// <summary>
        /// Event triggered whenever a clip is added to or removed from the player's clip manager.
        /// Useful for editor UI updates.
        /// </summary>
        public event System.Action OnClipAddedOrRemoved;

        /// <summary>
        /// Event triggered specifically when a clip is added.
        /// Parameter: T (clipAdded) - The clip that was added.
        /// </summary>
        public event System.Action<T> OnClipAdded;

        /// <summary>
        /// Event triggered specifically when a clip is removed.
        /// Parameter: T (clipRemoved) - The clip that was removed.
        /// </summary>
        public event System.Action<T> OnClipRemoved;


        // --- Properties ---

        /// <summary>
        /// Gets the clip manager instance responsible for storing and managing the clips associated with this player.
        /// </summary>
        public OM_ClipsManager<T> ClipsManager { get; }

        /// <summary>
        /// Gets the current elapsed time within the timeline's duration (in seconds).
        /// This value typically progresses from 0 to GetTimelineDuration() (or vice-versa if playing backward).
        /// </summary>
        public float ElapsedTime { get; }

        /// <summary>
        /// Gets the index of the currently selected clip in the editor UI context.
        /// Returns -1 if no clip is selected.
        /// </summary>
        public int SelectedClipIndex { get; }


        // --- Methods ---

        /// <summary>
        /// Performs initialization steps required specifically for the editor environment.
        /// This might include setting up editor-only data structures or validating initial state.
        /// </summary>
        public void InitPlayerForEditor();

        /// <summary>
        /// Gets the total duration of the timeline (in seconds).
        /// Playback typically loops or stops when ElapsedTime reaches this value (or 0 if playing backward).
        /// </summary>
        /// <returns>The total duration in seconds.</returns>
        public float GetTimelineDuration();

        /// <summary>
        /// Sets the total duration of the timeline (in seconds).
        /// </summary>
        /// <param name="newDuration">The new total duration.</param>
        public void SetTimelineDuration(float newDuration);

        /// <summary>
        /// Sets the current elapsed time of the player. Clamps the value between 0 and the timeline duration.
        /// This is used for scrubbing or seeking within the timeline.
        /// </summary>
        /// <param name="newElapsedTime">The desired elapsed time (in seconds).</param>
        public void SetElapsedTime(float newElapsedTime);

        /// <summary>
        /// Adds a new clip to the timeline via the ClipsManager.
        /// </summary>
        /// <param name="clipToAdd">The clip instance to add.</param>
        public void AddClip(T clipToAdd);

        /// <summary>
        /// Removes a clip from the timeline via the ClipsManager.
        /// </summary>
        /// <param name="clipToRemove">The clip instance to remove.</param>
        public void RemoveClip(T clipToRemove);

        /// <summary>
        /// Duplicates an existing clip and adds the copy to the timeline, typically adjacent to the original.
        /// </summary>
        /// <param name="clipToDuplicate">The clip instance to duplicate.</param>
        public void DuplicateClip(T clipToDuplicate);

        /// <summary>
        /// Gets an enumerable collection of all clips currently managed by the player.
        /// The order might depend on the implementation (e.g., insertion order or sorted by OrderIndex).
        /// </summary>
        /// <returns>An IEnumerable<T> containing all clips.</returns>
        public IEnumerable<T> GetClips();

        /// <summary>
        /// Records an action for the Unity Undo system. Essential for making editor changes undoable.
        /// </summary>
        /// <param name="undoName">The name displayed in the Undo history (e.g., "Move Clip").</param>
        public void RecordUndo(string undoName);

        /// <summary>
        /// Called when the player's data might need validation, often triggered by changes in the editor.
        /// Implementors should perform necessary checks and potentially update internal state or editor UI.
        /// </summary>
        public void OnValidate();

        /// <summary>
        /// Sets the index of the currently selected clip in the editor context.
        /// Used by the editor UI to inform the player about the selection state.
        /// </summary>
        /// <param name="index">The index of the selected clip, or -1 for no selection.</param>
        public void SetSelectedClipIndex(int index);

        /// <summary>
        /// Gets the index of the currently selected clip.
        /// Used by the editor UI or other systems to query the selection state.
        /// </summary>
        /// <returns>The index of the selected clip, or -1 if none.</returns>
        public int GetSelectedClipIndex();
        
    }
}