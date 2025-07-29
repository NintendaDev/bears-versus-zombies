using System;
using UnityEngine;

namespace OM
{
    /// <summary>
    /// Represents a single audio player instance within the sound system.
    /// It utilizes an attached AudioSource component to play audio clips
    /// and is typically managed (created, played, released) by the OM_SoundManager
    /// via an object pool.
    /// </summary>
    [RequireComponent(typeof(AudioSource))] // Ensures an AudioSource component is always present on the same GameObject.
    public class OM_AudioPlayer : MonoBehaviour
    {
        /// <summary>
        /// Identifier for the audio clip currently assigned or playing.
        /// Set to -1 when playing a raw AudioClip directly.
        /// </summary>
        public int AudioId { get; private set; }

        /// <summary>
        /// Reference to the sound manager that owns/manages this audio player.
        /// </summary>
        public OM_SoundManager SoundManager { get; private set; }

        /// <summary>
        /// The Unity AudioSource component used for actual sound playback.
        /// </summary>
        public AudioSource AudioSource { get; private set; }

        /// <summary>
        /// Initializes the audio player, linking it to the sound manager
        /// and setting up the AudioSource component with default settings.
        /// </summary>
        /// <param name="soundManager">The sound manager instance.</param>
        public void Setup(OM_SoundManager soundManager)
        {
            SoundManager = soundManager;
            AudioSource = GetComponent<AudioSource>();
            // Ensure the audio doesn't play automatically when the component awakens.
            AudioSource.playOnAwake = false; // Changed from true in original code - likely a typo correction as pooled objects shouldn't play on awake.
            // Ensure the audio clip does not loop by default. Looping is usually handled by the manager or specific logic.
            AudioSource.loop = false;
        }

        /// <summary>
        /// Plays an audio clip using the provided structured OM_AudioClipData.
        /// Handles setting volume, pitch, and position.
        /// </summary>
        /// <param name="clip">The audio data to play.</param>
        public void Play(OM_AudioClipData clip)
        {
            AudioId = clip.Id; // Store the ID from the clip data.
            AudioSource.clip = clip.Clip; // Assign the AudioClip to the source.
            AudioSource.mute = false; // Ensure the audio is not muted.
            AudioSource.volume = clip.Volume; // Set the volume from the clip data.
            AudioSource.pitch = clip.Pitch; // Set the pitch from the clip data.
            AudioSource.Play(); // Start playback.

            // Set the position of this player GameObject. If the clip data has a position, use it; otherwise, use the world origin.
            transform.position = clip.Position ?? Vector3.zero;
        }

        /// <summary>
        /// Plays an audio clip using the provided OM_AudioClip, which allows for randomized volume and pitch.
        /// Plays the sound at the world origin.
        /// </summary>
        /// <param name="clip">The OM_AudioClip definition to play.</param>
        public void Play(OM_AudioClip clip)
        {
            AudioId = clip.Id; // Store the ID from the clip definition.
            AudioSource.clip = clip.Clip; // Assign the AudioClip to the source.
            AudioSource.mute = false; // Ensure the audio is not muted.
            // Get potentially randomized volume/pitch from the OM_AudioClip definition.
            AudioSource.volume = clip.GetVolume();
            AudioSource.pitch = clip.GetPitch();
            AudioSource.Play(); // Start playback.

            // Play this sound at the world origin.
            transform.position = Vector3.zero;
        }

        /// <summary>
        /// Plays a raw AudioClip with a specified volume and default pitch.
        /// Sets AudioId to -1 as it's not tied to a specific OM_AudioClip/Data.
        /// Plays the sound at the world origin.
        /// </summary>
        /// <param name="clip">The raw AudioClip to play.</param>
        /// <param name="volume">The desired volume (0.0 to 1.0).</param>
        public void Play(AudioClip clip, float volume = 1)
        {
            AudioId = -1; // Indicate no specific ID is associated.
            AudioSource.clip = clip; // Assign the AudioClip to the source.
            AudioSource.mute = false; // Ensure the audio is not muted.
            AudioSource.volume = volume; // Set the specified volume.
            AudioSource.pitch = 1; // Use default pitch.
            AudioSource.Play(); // Start playback.

            // Play this sound at the world origin.
            transform.position = Vector3.zero;
        }

        /// <summary>
        /// Stops the currently playing audio on the attached AudioSource.
        /// </summary>
        public void Stop()
        {
            AudioSource.Stop();
        }

        /// <summary>
        /// Checks if the attached AudioSource is currently playing audio.
        /// </summary>
        /// <returns>True if audio is playing, false otherwise.</returns>
        public bool IsPlaying()
        {
            // Note: isPlaying might remain true for a frame after Stop() is called or after playback finishes naturally.
            // For pooled objects, checking if the clip has finished is often done in the manager's Update loop.
            return AudioSource.isPlaying;
        }
    }
}