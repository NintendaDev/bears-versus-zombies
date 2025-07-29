using UnityEngine; 

namespace OM.TimelineCreator.Runtime 
{
    /// <summary>
    /// Base class for all clip data stored within the timeline system.
    /// Contains common properties like name, description, timing, activation state,
    /// display color, and error status. Intended to be inherited by specific clip types (e.g., AnimoraClip).
    /// </summary>
    [System.Serializable]
    public abstract class OM_ClipBase // Changed to abstract as it likely isn't instantiated directly
    {
        // --- Serialized Fields ---

        // Using OM_StartGroup for editor organization (requires a custom drawer or editor script)
        [OM_StartGroup("Clip Info", "Info")] // Group Title, Group ID/Key
        [SerializeField] private string clipName = "New Clip"; // Default name for the clip

        [SerializeField, TextArea] // Use TextArea for multi-line editing in the inspector
        private string clipDescription = ""; // Optional description for the clip

        [SerializeField] private Color highlightColor = Color.white; // Color used for visual representation in the timeline editor

        [OM_StartGroup("Settings", "Settings")] // Start a new group for timing properties
        [SerializeField, OM_HideInInspector] // Hide by default, likely controlled by editor logic (e.g., track UI)
        private bool isActive = true; // Whether the clip is currently active and should be evaluated

        [SerializeField, Min(0)] // Ensure start time is not negative
        private float startTime = 0; // The time (in seconds) on the timeline where this clip begins

        [SerializeField, Min(0)] // Ensure duration is not negative
        private float duration = 1; // The length (in seconds) of this clip

        [SerializeField, OM_HideInInspector] // Hide by default, managed internally or by editor
        private int orderIndex = 0; // The vertical order/layer index of this clip on the timeline

        // --- Public Properties (Accessors for Serialized Fields) ---

        /// <summary>
        /// Gets or sets the name of the clip, displayed in the timeline editor.
        /// </summary>
        public string ClipName { get => clipName; set => clipName = value; }

        /// <summary>
        /// Gets or sets the description for the clip, potentially shown as a tooltip or in an inspector.
        /// </summary>
        public string ClipDescription { get => clipDescription; set => clipDescription = value; }

        /// <summary>
        /// Gets or sets the color used to highlight this clip in the timeline editor UI.
        /// </summary>
        public Color HighlightColor { get => highlightColor; set => highlightColor = value; }

        /// <summary>
        /// Gets or sets a value indicating whether this clip is active. Inactive clips are typically ignored during playback.
        /// </summary>
        public bool IsActive { get => isActive; set => isActive = value; }

        /// <summary>
        /// Gets or sets the start time of the clip on the timeline (in seconds).
        /// Setting this might require updating the editor UI representation.
        /// </summary>
        public float StartTime { get => startTime; set => startTime = Mathf.Max(0, value); } // Added validation

        /// <summary>
        /// Gets or sets the duration of the clip (in seconds).
        /// Setting this might require updating the editor UI representation.
        /// </summary>
        public float Duration { get => duration; set => duration = Mathf.Max(0, value); } // Added validation

        /// <summary>
        /// Gets or sets the order index (vertical layer) of the clip.
        /// This is primarily used for sorting tracks visually and resolving potential overlaps if needed.
        /// Modifying this usually requires re-sorting or updating the timeline structure.
        /// </summary>
        public int OrderIndex { get => orderIndex; set => orderIndex = value; }


        // --- Virtual Methods for Timing ---

        /// <summary>
        /// Gets the effective start time of the clip. Can be overridden if start time calculation is complex.
        /// </summary>
        /// <returns>The start time in seconds.</returns>
        public virtual float GetStartTime()
        {
            return startTime;
        }

        /// <summary>
        /// Gets the effective duration of the clip. Can be overridden if duration calculation is complex.
        /// </summary>
        /// <returns>The duration in seconds.</returns>
        public virtual float GetDuration()
        {
            return duration;
        }

        /// <summary>
        /// Sets the effective duration of the clip. Can be overridden for custom logic.
        /// </summary>
        /// <param name="newDuration">The new duration in seconds.</param>
        public virtual void SetDuration(float newDuration)
        {
            // Basic implementation with validation
            duration = Mathf.Max(0, newDuration);
            // Derived classes might need to trigger UI updates or other logic here
        }

        /// <summary>
        /// Sets the effective start time of the clip. Can be overridden for custom logic.
        /// </summary>
        /// <param name="newStartTime">The new start time in seconds.</param>
        public virtual void SetStartTime(float newStartTime)
        {
            // Basic implementation with validation
            startTime = Mathf.Max(0, newStartTime);
            // Derived classes might need to trigger UI updates or other logic here
        }

        /// <summary>
        /// Calculates and gets the effective end time of the clip (StartTime + Duration).
        /// </summary>
        /// <returns>The end time in seconds.</returns>
        public virtual float GetEndTime()
        {
            // Standard calculation, ensures virtual methods for start/duration are used
            return GetStartTime() + GetDuration();
        }


        // --- Virtual Methods for Playback Eligibility and State ---

        /// <summary>
        /// Determines if the clip is currently in a state where it can be played.
        /// Base implementation checks IsActive and HasError. Derived classes (like AnimoraClip)
        /// can add further conditions (e.g., play chance).
        /// </summary>
        /// <returns>True if the clip can potentially be played, false otherwise.</returns>
        public virtual bool CanBePlayed()
        {
            // Default condition: must be active and have no errors.
            return IsActive && HasError(out _) == false;
        }

        /// <summary>
        /// Checks if the clip has any configuration errors that would prevent playback.
        /// Base implementation always returns false (no errors). Derived classes should override this
        /// to perform specific validation (e.g., check if targets are assigned).
        /// </summary>
        /// <param name="error">Output parameter containing the error message if an error is found.</param>
        /// <returns>True if an error exists, false otherwise.</returns>
        public virtual bool HasError(out string error)
        {
            error = string.Empty; // Default: no error
            return false;
        }

        /// <summary>
        /// Resets the clip's internal state. Called when resetting the timeline or potentially on clip creation/modification.
        /// Base implementation does nothing. Derived classes can override to reset specific state variables.
        /// </summary>
        public virtual void Reset()
        {
            // No base state to reset here. Derived classes handle their own state.
        }

         /// <summary>
        /// A simple override for ToString() to provide a meaningful representation, often the clip name.
        /// Useful for debugging and logging.
        /// </summary>
        /// <returns>The clip's name or a default string representation.</returns>
        public override string ToString()
        {
            return string.IsNullOrEmpty(clipName) ? base.ToString() : clipName;
        }
    }
}