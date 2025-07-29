using System.Collections.Generic;
using System.Linq;
using UnityEngine; 

namespace OM.TimelineCreator.Runtime 
{
    /// <summary>
    /// Manages a collection of OM_ClipBase-derived clips for an IOM_TimelinePlayer.
    /// Handles initialization, retrieval, adding, removing, and manipulation of clips within the timeline.
    /// Uses [SerializeReference] to correctly handle polymorphic lists of clip types.
    /// </summary>
    /// <typeparam name="T">The specific type of OM_ClipBase being managed (e.g., AnimoraClip).</typeparam>
    [System.Serializable]
    public class OM_ClipsManager<T> where T : OM_ClipBase
    {
        /// <summary>
        /// The list containing all the clips managed by this instance.
        /// Marked with [SerializeReference] to support storing different subclasses of T.
        /// </summary>
        [SerializeReference] protected List<T> clips;

        /// <summary>
        /// Flag indicating whether the manager has been initialized (e.g., null checks performed).
        /// Prevents redundant initialization work.
        /// </summary>
        protected bool IsInitialized;

        /// <summary>
        /// Initializes the clips manager specifically for editor use.
        /// Ensures the clips list exists and removes any null entries that might occur due to serialization issues or manual deletion.
        /// Re-calculates the OrderIndex for all remaining clips to ensure consistency.
        /// </summary>
        public virtual void InitForEditor()
        {
            clips ??= new List<T>(); // Ensure the list is not null
            var hasChanged = false;

            // Iterate backwards to safely remove null entries
            for (var i = clips.Count - 1; i >= 0; i--)
            {
                if (clips[i] == null)
                {
                    clips.RemoveAt(i);
                    hasChanged = true; // Mark that the list was modified
                }
            }

            // If null entries were removed, re-index the remaining clips
            if (!hasChanged) return; // No need to re-index if no changes were made

            for (var i = 0; i < clips.Count; i++)
            {
                if (clips[i] != null) // Double-check for null just in case
                {
                    clips[i].OrderIndex = i; // Assign sequential order index
                }
            }
        }

        /// <summary>
        /// Initializes the clips manager for runtime use.
        /// Ensures the clips list exists and removes null entries.
        /// Sets the IsInitialized flag to prevent re-initialization.
        /// </summary>
        public virtual void Init()
        {
            if (IsInitialized) return; // Prevent multiple initializations
            IsInitialized = true;

            clips ??= new List<T>(); // Ensure the list is not null

            // Iterate backwards to safely remove null entries (less critical at runtime unless expecting external modification)
            for (var i = clips.Count - 1; i >= 0; i--)
            {
                if (clips[i] == null)
                {
                    clips.RemoveAt(i);
                    // Consider logging a warning here if null clips are unexpected at runtime
                }
            }
             // Note: Runtime Init doesn't typically re-index like InitForEditor
        }

        /// <summary>
        /// Gets the complete, mutable list of all clips managed by this instance.
        /// </summary>
        /// <returns>The List<T> containing the clips.</returns>
        public List<T> GetClips()
        {
            clips ??= new List<T>(); // Ensure list exists before returning
            return clips;
        }

        /// <summary>
        /// Gets an enumerable collection of clips that are currently considered playable (based on their CanBePlayed() method).
        /// </summary>
        /// <returns>An IEnumerable<T> containing only the playable clips.</returns>
        public IEnumerable<T> GetCanBePlayedClips()
        {
            clips ??= new List<T>();
            // Use LINQ Where to filter based on the clip's CanBePlayed status
            return clips.Where(clip => clip != null && clip.CanBePlayed()); // Added null check
        }

        /// <summary>
        /// Gets an enumerable collection of clips sorted by their OrderIndex property.
        /// </summary>
        /// <returns>An IEnumerable<T> containing the clips sorted by OrderIndex.</returns>
        public IEnumerable<T> GetClipsBasedOnOrderIndex()
        {
            clips ??= new List<T>();
            // Use LINQ OrderBy to sort based on the clip's OrderIndex
            return clips.Where(clip => clip != null).OrderBy(clip => clip.OrderIndex); // Added null check
        }

        /// <summary>
        /// Adds a new clip to the end of the managed list and sets its OrderIndex.
        /// Records an undo operation in the editor context.
        /// </summary>
        /// <param name="clip">The clip instance to add.</param>
        /// <param name="target">The timeline player instance (used for recording undo).</param>
        public virtual void AddClip(T clip, IOM_TimelinePlayer<T> target)
        {
            if (clip == null)
            {
                Debug.LogError("Attempted to add a null clip.");
                return;
            }
            clips ??= new List<T>();
            target.RecordUndo("Add Clip"); // Record the action for editor undo
            clips.Add(clip);
            // Assign the order index based on the new list count (0-based index)
            clip.OrderIndex = clips.Count - 1;
            // Consider calling target.OnValidate() or similar if needed after adding
        }

        /// <summary>
        /// Removes a specified clip from the managed list and adjusts the OrderIndex of subsequent clips.
        /// Records an undo operation in the editor context.
        /// </summary>
        /// <param name="clipToRemove">The clip instance to remove.</param>
        /// <param name="target">The timeline player instance (used for recording undo).</param>
        public virtual void RemoveClip(T clipToRemove, IOM_TimelinePlayer<T> target)
        {
            if (clipToRemove == null || clips == null) return; // Nothing to remove or list doesn't exist

            target.RecordUndo("Remove Clip"); // Record the action for editor undo

            int removedIndex = clipToRemove.OrderIndex; // Store the index before removing
            if (clips.Remove(clipToRemove)) // Attempt to remove the clip
            {
                // Adjust the order index of the clips that came after the removed one
                foreach (var clip in clips)
                {
                    if (clip != null && clip.OrderIndex > removedIndex)
                    {
                        clip.OrderIndex--; // Decrement index to fill the gap
                    }
                }
                 // Consider calling target.OnValidate() or similar if needed after removing
            }
            else
            {
                Debug.LogWarning("Attempted to remove a clip that was not found in the manager.");
            }
        }

        /// <summary>
        /// Inserts a clip at a specific index in the list, adjusting the OrderIndex of existing clips.
        /// Records an undo operation.
        /// </summary>
        /// <param name="clipToInsert">The clip to insert.</param>
        /// <param name="target">The timeline player instance (for recording undo).</param>
        /// <param name="index">The target index where the clip should be inserted.</param>
        /// <returns>True if insertion was successful, false otherwise (e.g., null clip, invalid index).</returns>
        public virtual bool InsertClip(T clipToInsert, IOM_TimelinePlayer<T> target, int index)
        {
            // --- Input Validation ---
            if (clipToInsert == null)
            {
                Debug.LogError("Trying to insert a null clip to the clips list");
                return false;
            }
            clips ??= new List<T>(); // Ensure list exists
            // Allow inserting at the end (index == Count)
            if (index < 0 || index > clips.Count)
            {
                Debug.LogError($"Trying to insert a clip at an invalid index ({index}). Valid range is [0..{clips.Count}].");
                return false;
            }

            // --- Logic ---
            target.RecordUndo("Insert Clip"); // Record action

            // Increment OrderIndex for existing clips at or after the target index
            foreach (var clip in clips)
            {
                if (clip != null && clip.OrderIndex >= index)
                {
                    clip.OrderIndex++;
                }
            }

            // Set the new clip's index and add it
            clipToInsert.OrderIndex = index;
            // Use List.Insert to place it at the correct position if maintaining list order matters,
            // otherwise just Add and rely on OrderIndex property for sorting later. Add is simpler.
            clips.Add(clipToInsert);

            // Optional: Re-sort the list immediately based on OrderIndex if needed
            // clips = clips.OrderBy(c => c.OrderIndex).ToList();
            // Consider calling target.OnValidate() or similar if needed after inserting

            return true;
        }

        /// <summary>
        /// Duplicates a specified clip and inserts the copy immediately after the original.
        /// Records an undo operation. Uses JsonUtility for deep copying the clip data.
        /// </summary>
        /// <param name="clipToDuplicate">The clip instance to duplicate.</param>
        /// <param name="target">The timeline player instance (used for recording undo).</param>
        public virtual void DuplicateClip(T clipToDuplicate, IOM_TimelinePlayer<T> target)
        {
             if (clipToDuplicate == null || clips == null)
             {
                 Debug.LogError("Attempted to duplicate a null clip or manager has no clip list.");
                 return;
             }

            int index = clipToDuplicate.OrderIndex;
            // Find the actual index in the list, OrderIndex might not match if list wasn't recently sorted/validated
            // int listIndex = clips.IndexOf(clipToDuplicate);
            // if (listIndex == -1) { ... handle error ... }

            var type = clipToDuplicate.GetType(); // Get the specific derived type

            // Use JsonUtility to create a deep copy of the clip's serialized data
            var json = JsonUtility.ToJson(clipToDuplicate);
            var newClip = (T)JsonUtility.FromJson(json, type);

            if (newClip == null)
            {
                 Debug.LogError($"Failed to duplicate clip '{clipToDuplicate.ClipName}' using JsonUtility.");
                 return;
            }

            // Use InsertClip to add the duplicate at the next index
            // RecordUndo is handled within InsertClip
            InsertClip(newClip, target, index + 1);
        }

        /// <summary>
        /// Replaces the current list of clips with a new list.
        /// Primarily used for loading saved data or replacing the entire timeline content.
        /// </summary>
        /// <param name="newClips">The new list of clips to manage.</param>
        public void PopulateWithClips(List<T> newClips)
        {
            // Directly replace the list. Assumes newClips is valid.
            // Consider adding validation or re-indexing if necessary.
            this.clips = newClips ?? new List<T>(); // Ensure it's not set to null
            IsInitialized = false; // Mark as uninitialized so Init() can run again if needed
            // Consider calling InitForEditor() or Init() immediately after if required.
        }
    }
}