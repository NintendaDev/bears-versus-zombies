using OM.TimelineCreator.Runtime;

namespace OM.Animora.Runtime
{
    /// <summary>
    /// Manages a collection of <see cref="AnimoraClip"/> instances specifically for the Animora system.
    /// It extends the generic clip management functionality provided by <see cref="OM_ClipsManager{T}"/>
    /// and adds validation logic relevant to <see cref="AnimoraPlayer"/> and <see cref="AnimoraClip"/>.
    /// This class is typically serialized as part of an <see cref="AnimoraPlayer"/>.
    /// </summary>
    [System.Serializable] // Ensures this class can be serialized by Unity when embedded in another component like AnimoraPlayer.
    public class AnimoraClipsManager : OM_ClipsManager<AnimoraClip> // Inherits from the base clips manager, specializing it for AnimoraClip type.
    {
        /// <summary>
        /// Validates all the <see cref="AnimoraClip"/> instances managed by this manager.
        /// This method is designed to be called from the owning <see cref="AnimoraPlayer"/>'s `OnValidate` method,
        /// particularly useful for reacting to changes made in the Unity Editor inspector.
        /// It iterates through each clip, checks for null entries (which might occur due to editor list manipulation),
        /// and calls the `OnValidate` method on each valid clip, passing the owning player for context.
        /// </summary>
        /// <param name="player">The <see cref="AnimoraPlayer"/> instance that owns this manager and its clips.
        /// This allows clips to potentially validate themselves against the player's state or components.</param>
        public void OnValidate(AnimoraPlayer player)
        {
            // Ensure the clips list is initialized before proceeding.
            // GetClips() likely retrieves the 'clips' list from the base class.
            if (GetClips() == null) return;

            // Iterate through the collection of managed clips.
            foreach (var clip in GetClips())
            {
                // If a slot in the list contains null (e.g., element removed in inspector),
                // it indicates an inconsistent state.
                if (clip == null)
                {
                    // Re-initialize the manager for the editor context.
                    // This base class method likely handles cleaning up null entries
                    // and potentially re-assigning clip order indices.
                    InitForEditor();
                    // Since the collection might have been modified by InitForEditor,
                    // it's safest to break the current iteration. The editor will likely
                    // trigger OnValidate again if necessary.
                    break;
                }
                // If the clip instance is valid, call its own OnValidate method.
                // This allows each AnimoraClip type to perform specific validation logic,
                // potentially using the provided player context.
                clip.OnValidate(player);
            }
        }

        // Note: Core functionalities like adding, removing, retrieving clips (AddClip, RemoveClip, GetClips),
        // and editor initialization (InitForEditor) are assumed to be provided by the base
        // OM_ClipsManager<AnimoraClip> class. This derived class only adds specific validation logic.
    }
}