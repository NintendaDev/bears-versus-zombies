using UnityEngine;

namespace OM
{
    /// <summary>
    /// A simple data structure (struct) holding the specific parameters needed to play a single audio instance.
    /// This is typically generated from an OM_AudioClip configuration just before playback.
    /// </summary>
    public struct OM_AudioClipData
    {
        /// <summary>
        /// The actual AudioClip asset to be played.
        /// </summary>
        public AudioClip Clip;

        /// <summary>
        /// An optional identifier for this audio clip, potentially used for tracking or specific logic.
        /// </summary>
        public int Id;

        /// <summary>
        /// The specific volume at which this audio instance should be played (0.0 to 1.0).
        /// </summary>
        public float Volume;

        /// <summary>
        /// The specific pitch shift for this audio instance (1.0 is normal pitch).
        /// </summary>
        public float Pitch;

        /// <summary>
        /// An optional 3D position for spatialized audio playback. Null indicates non-spatialized audio.
        /// </summary>
        public Vector3? Position;

        /// <summary>
        /// Checks if this audio data structure contains a valid AudioClip that can be played.
        /// </summary>
        /// <returns>True if the Clip is not null, false otherwise.</returns>
        public bool CanBePlayed()
        {
            // Playback requires a valid AudioClip.
            return Clip != null;
        }
    }

    /// <summary>
    /// Represents a configurable audio clip definition, often used in the Unity Inspector.
    /// This class allows specifying ranges for pitch and volume, introducing randomization to playback.
    /// It can be converted into an OM_AudioClipData struct for actual playback instances.
    /// </summary>
    [System.Serializable] // Allows instances of this class to be serialized by Unity (e.g., saved in scenes or prefabs).
    public class OM_AudioClip
    {
        // --- Serialized Fields (Configurable in Unity Inspector) ---

        [SerializeField] // Exposes the 'clip' field in the Unity Inspector.
        private AudioClip clip; // The base AudioClip asset.

        [SerializeField] // Exposes the 'id' field in the Unity Inspector.
        private int id; // Identifier for this audio definition.

        [SerializeField, OM_MinMaxSlider(-1, 2)] // Exposes 'pitchRange' and uses a custom MinMaxSlider drawer (attribute) in the inspector.
        private Vector2 pitchRange = new Vector2(1, 1); // Range (min, max) for random pitch variation. Default is no variation (pitch = 1).

        [SerializeField, OM_MinMaxSlider(0, 1)] // Exposes 'volumeRange' and uses a custom MinMaxSlider drawer (attribute) in the inspector.
        private Vector2 volumeRange = new Vector2(1, 1); // Range (min, max) for random volume variation. Default is full volume (1).

        // --- Public Properties (Read-only accessors) ---

        /// <summary>
        /// Gets the assigned AudioClip asset.
        /// </summary>
        public AudioClip Clip => clip;

        /// <summary>
        /// Gets the assigned identifier.
        /// </summary>
        public int Id => id;

        /// <summary>
        /// Gets the configured pitch range (x = min, y = max).
        /// </summary>
        public Vector2 PitchRange => pitchRange;

        /// <summary>
        /// Gets the configured volume range (x = min, y = max).
        /// </summary>
        public Vector2 VolumeRange => volumeRange;

        // --- Public Methods ---

        /// <summary>
        /// Checks if this audio configuration contains a valid AudioClip.
        /// </summary>
        /// <returns>True if the underlying 'clip' field is not null, false otherwise.</returns>
        public bool CanBePlayed()
        {
            return clip != null;
        }

        /// <summary>
        /// Gets a randomized pitch value within the configured <see cref="PitchRange"/>.
        /// </summary>
        /// <returns>A float representing the randomized pitch.</returns>
        public float GetPitch()
        {
            // Return a random value between the min (x) and max (y) of the pitchRange Vector2.
            return Random.Range(pitchRange.x, pitchRange.y);
        }

        /// <summary>
        /// Gets a randomized volume value within the configured <see cref="VolumeRange"/>.
        /// </summary>
        /// <returns>A float representing the randomized volume (typically between 0.0 and 1.0).</returns>
        public float GetVolume()
        {
            // Return a random value between the min (x) and max (y) of the volumeRange Vector2.
            return Random.Range(volumeRange.x, volumeRange.y);
        }

        /// <summary>
        /// Creates an <see cref="OM_AudioClipData"/> instance based on this configuration,
        /// applying randomized pitch and volume.
        /// </summary>
        /// <returns>An OM_AudioClipData struct ready for playback.</returns>
        /// <remarks>
        /// The position in the returned data is always set to null, indicating non-spatialized audio
        /// unless explicitly set later or handled differently by the audio player.
        /// </remarks>
        public OM_AudioClipData ConvertToAudioClipData()
        {
            // Create a new data struct instance.
            return new OM_AudioClipData
            {
                Clip = clip,         // Use the configured clip.
                Id = id,             // Use the configured ID.
                Volume = GetVolume(), // Get a randomized volume.
                Pitch = GetPitch(),   // Get a randomized pitch.
                Position = null      // Position is not handled by this configuration object directly.
            };
        }
    }
}