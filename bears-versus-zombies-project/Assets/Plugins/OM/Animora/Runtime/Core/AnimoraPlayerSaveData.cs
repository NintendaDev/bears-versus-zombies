using System;
using System.Collections.Generic;
using UnityEngine;

namespace OM.Animora.Runtime
{
    /// <summary>
    /// A serializable data structure designed to hold the state of an <see cref="AnimoraPlayer"/>
    /// for saving and loading purposes. It includes configuration settings like play mode,
    /// timing, looping, events, and the list of associated clips.
    /// </summary>
    [Serializable] // Ensures this class can be serialized by Unity's serialization system (e.g., saved to a file or used with Prefabs/JsonUtility).
    public class AnimoraPlayerSaveData
    {
        /// <summary>
        /// Stores the playback start mode (<see cref="AnimoraPlayMode"/>) of the saved player.
        /// Determines when playback should automatically begin (e.g., OnStart, OnEnable, Manual).
        /// </summary>
        public AnimoraPlayMode playMode = AnimoraPlayMode.OnStart; // Default value if not loaded

        /// <summary>
        /// Stores the time mode (<see cref="OM_TimeMode"/>) used by the saved player.
        /// Determines if playback is affected by Time.timeScale (ScaledTime) or uses unscaled time.
        /// </summary>
        public OM_TimeMode timeMode = OM_TimeMode.ScaledTime; // Default value

        /// <summary>
        /// Stores the total duration (in seconds) of the saved timeline.
        /// </summary>
        public float timelineDuration = 2; // Default value

        /// <summary>
        /// Stores the playback speed multiplier of the saved player.
        /// </summary>
        public float playbackSpeed = 1; // Default value (normal speed)

        /// <summary>
        /// Stores the loop count setting of the saved player (-1 for infinite).
        /// Works in conjunction with the saved loop type.
        /// </summary>
        public int loopCount = 1; // Default value (play sequence once + 1 loop, effectively playing twice if Loop/PingPong)

        /// <summary>
        /// Stores the serialized state of the player's lifecycle events (<see cref="AnimoraPlayerEvents"/>).
        /// Includes listeners configured in the Inspector via UnityEvents.
        /// Uses [FormerlySerializedAs] to handle potential renaming from "playerEvents" during development, ensuring backward compatibility with older save data.
        /// </summary>
        public AnimoraPlayerEvents animoraPlayerEvents; // Should be serializable itself

        /// <summary>
        /// Stores the list of <see cref="AnimoraClip"/> data instances associated with the saved player.
        /// Uses [SerializeReference] to handle polymorphism correctly, allowing different derived types
        /// of AnimoraClip to be serialized within the same list by reference rather than by value,
        /// preserving their specific data fields.
        /// </summary>
        [SerializeReference] // Crucial for serializing lists containing potentially different derived types of AnimoraClip
        public List<AnimoraClip> clips;

        // Note: This class acts purely as a data container. It doesn't contain logic itself,
        // but is used by AnimoraPlayer's GetSaveData() and LoadSaveData() methods.
    }
}