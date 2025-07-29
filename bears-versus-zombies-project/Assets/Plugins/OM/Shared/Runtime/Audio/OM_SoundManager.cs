
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace OM
{
    /// <summary>
    /// Manages sound playback within the application using a Singleton pattern.
    /// Handles playing sounds via pooled OM_AudioPlayer instances for efficiency
    /// and also provides methods for playing simple one-shot sounds directly
    /// using its own AudioSource component.
    /// </summary>
    [RequireComponent(typeof(AudioSource))] // Ensures this manager GameObject always has an AudioSource for PlayOneShot sounds.
    public class OM_SoundManager : MonoBehaviour
    {
        // --- Constants for Resource Loading ---

        /// <summary>
        /// The path within the Resources folder where the OM_SoundManager prefab is located.
        /// Used by the Singleton pattern if no instance exists and it needs to be instantiated.
        /// </summary>
        private const string PrefabPathInResources = "OM_SoundManager";

        /// <summary>
        /// The path within the Resources folder where the OM_AudioPlayer prefab is located.
        /// Used to instantiate new audio players for the object pool.
        /// </summary>
        private const string AudioPlayerPrefabInResources = "OM_AudioPlayer";

        // --- Singleton Implementation ---

        /// <summary>
        /// The static backing field for the Singleton instance.
        /// </summary>
        private static OM_SoundManager _instance;

        /// <summary>
        /// Provides global access to the single OM_SoundManager instance.
        /// Handles finding an existing instance, loading it from Resources, or creating a new one if necessary.
        /// Ensures the manager is initialized via the Init() method.
        /// </summary>
        public static OM_SoundManager Instance
        {
            get
            {
                // 1. Check if the instance already exists.
                if (_instance != null) return _instance;

                // 2. If not, try to find an existing OM_SoundManager in the scene.
                // Note: FindFirstObjectByType is generally preferred over FindObjectOfType in modern Unity.
                _instance = FindFirstObjectByType<OM_SoundManager>();
                if (_instance != null)
                {
                    // Found an existing instance, ensure it's initialized and return it.
                    if (!_instance._initialized) _instance.Init();
                    return _instance;
                }

                // 3. If no instance exists in the scene, try loading the prefab from Resources.
                var prefab = Resources.Load<OM_SoundManager>(PrefabPathInResources);
                if (prefab != null)
                {
                    // Instantiate the prefab, initialize it, and return the new instance.
                    _instance = Instantiate(prefab);
                    _instance.Init(); // Init is called *after* instantiation.
                    return _instance;
                }

                // 4. If no prefab was found, create a new GameObject and add the OM_SoundManager component.
                // This is a fallback scenario.
                Debug.LogWarning($"OM_SoundManager prefab not found at Resources/{PrefabPathInResources}. Creating a new instance dynamically.");
                _instance = new GameObject("OM_SoundManager").AddComponent<OM_SoundManager>();
                _instance.Init(); // Init is called *after* component creation.
                return _instance;
            }
        }

        // --- Object Pooling ---

        /// <summary>
        /// The object pool responsible for managing OM_AudioPlayer instances.
        /// This improves performance by reusing player objects instead of constantly creating and destroying them.
        /// </summary>
        public ObjectPool<OM_AudioPlayer> AudioPlayerPool { get; private set; }

        /// <summary>
        /// Flag to prevent redundant initialization.
        /// </summary>
        private bool _initialized = false;

        /// <summary>
        /// The AudioSource component attached to the SoundManager itself.
        /// Used primarily for PlayOneShot sounds that don't require pooling or complex control.
        /// </summary>
        private AudioSource _audioSource;

        /// <summary>
        /// Cached reference to the loaded OM_AudioPlayer prefab.
        /// </summary>
        private OM_AudioPlayer _audioPlayerPrefab;

        /// <summary>
        /// A list containing references to all currently active (borrowed from the pool) OM_AudioPlayer instances.
        /// Used by the Update loop to check when players finish playing.
        /// </summary>
        private readonly List<OM_AudioPlayer> _activePlayers = new List<OM_AudioPlayer>();


        // --- Initialization ---

        /// <summary>
        /// Initializes the Sound Manager. This is called automatically by the Instance getter
        /// when the instance is first accessed or created.
        /// Sets up the manager's AudioSource, DontDestroyOnLoad, loads the player prefab,
        /// and configures the ObjectPool.
        /// </summary>
        private void Init()
        {
            if (_initialized) return; // Prevent multiple initializations.
            _initialized = true;

            // Get or add the manager's own AudioSource component.
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null) _audioSource = gameObject.AddComponent<AudioSource>();

            // Configure the manager's AudioSource for one-shot sounds (non-looping, doesn't play on awake).
            _audioSource.playOnAwake = false;
            _audioSource.loop = false;

            // Ensure the Sound Manager persists across scene loads.
            DontDestroyOnLoad(gameObject);

            // Load the OM_AudioPlayer prefab from Resources.
            _audioPlayerPrefab = Resources.Load<OM_AudioPlayer>(AudioPlayerPrefabInResources);
            if (_audioPlayerPrefab == null)
            {
                // Log an error if the prefab is missing, as pooling won't work.
                Debug.LogError($"AudioPlayer prefab not found in Resources at path: {AudioPlayerPrefabInResources}");
                // Potentially disable pooling or throw an exception here depending on desired robustness.
            }

            // Create and configure the ObjectPool for OM_AudioPlayer instances.
            AudioPlayerPool = new ObjectPool<OM_AudioPlayer>(
                createFunc: () => // Action to create a new player instance when the pool is empty.
                {
                    if (_audioPlayerPrefab == null)
                    {
                        Debug.LogError("Cannot create new AudioPlayer, prefab is missing!");
                        return null; // Or handle this error more gracefully
                    }
                    // Instantiate the prefab as a child of the SoundManager.
                    var audioPlayer = Instantiate(_audioPlayerPrefab, transform);
                    audioPlayer.Setup(this); // Initialize the new player instance.
                    return audioPlayer;
                },
                actionOnGet: OnAudioPlayerGet,     // Action called when an item is taken from the pool.
                actionOnRelease: OnAudioPlayerRelease, // Action called when an item is returned to the pool.
                actionOnDestroy: OnAudioPlayerDestroy, // Action called when an item is destroyed (e.g., if pool capacity is exceeded).
                collectionCheck: true, // Enable checks to prevent returning an item multiple times.
                defaultCapacity: 10,  // Initial capacity (adjust as needed).
                maxSize: 50           // Maximum number of players the pool will hold (adjust as needed).
            );
        }

        // --- Pool Callbacks ---

        /// <summary>
        /// Called when an OM_AudioPlayer is destroyed by the pool (e.g., if maxSize is exceeded).
        /// Currently logs a warning but could be used for cleanup if necessary.
        /// </summary>
        /// <param name="player">The player instance being destroyed.</param>
        private void OnAudioPlayerDestroy(OM_AudioPlayer player)
        {
            // Ensure the GameObject is actually destroyed if the pool decides to remove it.
             if (player != null && player.gameObject != null)
             {
                Destroy(player.gameObject);
             }
            // Debug.LogWarning($"Destroying AudioPlayer: {player.name}"); // Optional: Log destruction
        }

        /// <summary>
        /// Called when an OM_AudioPlayer is returned to the object pool.
        /// Deactivates the player's GameObject and removes it from the active players list.
        /// </summary>
        /// <param name="player">The player instance being released.</param>
        private void OnAudioPlayerRelease(OM_AudioPlayer player)
        {
            player.gameObject.SetActive(false); // Hide the player.
            _activePlayers.Remove(player);      // Remove from tracking.
            // Optional: Reset parent if necessary, though Instantiate in createFunc sets it initially.
            // player.transform.SetParent(transform);
        }

        /// <summary>
        /// Called when an OM_AudioPlayer is taken from the object pool.
        /// Activates the player's GameObject and adds it to the active players list for tracking.
        /// </summary>
        /// <param name="player">The player instance being activated.</param>
        private void OnAudioPlayerGet(OM_AudioPlayer player)
        {
            player.gameObject.SetActive(true);  // Make the player visible/active.
            _activePlayers.Add(player);       // Add to tracking.
        }


        // --- Playback Methods ---

        /// <summary>
        /// Plays audio using a pooled OM_AudioPlayer instance based on the provided data.
        /// Suitable for sounds that might need positioning or longer playback duration.
        /// </summary>
        /// <param name="clipData">The structured data containing the AudioClip, volume, pitch, ID, and optional position.</param>
        public void Play(OM_AudioClipData clipData)
        {
            // Basic check to ensure the clip is playable.
            if (!clipData.CanBePlayed())
            {
                Debug.LogWarning($"Attempted to play an invalid AudioClipData (ID: {clipData.Id}).");
                return;
            }

            // Get an OM_AudioPlayer instance from the pool.
            var player = AudioPlayerPool.Get();
            if (player != null) // Check if Get succeeded (it might fail if prefab is null)
            {
                 player.Play(clipData); // Tell the player instance to play the clip data.
            }
        }

        /// <summary>
        /// Plays a sound immediately using the SoundManager's own AudioSource.
        /// Does *not* use the object pool. Ideal for short, frequent sounds like UI clicks
        /// where the overhead of pooling might not be necessary or desired.
        /// Uses volume from OM_AudioClipData, ignores pitch and position.
        /// </summary>
        /// <param name="clipData">The structured data containing the AudioClip and volume.</param>
        public void PlayOneShot(OM_AudioClipData clipData)
        {
            // Basic check to ensure the clip is playable.
            if (!clipData.CanBePlayed())
            {
                 Debug.LogWarning($"Attempted to PlayOneShot an invalid AudioClipData (ID: {clipData.Id}).");
                return;
            }
            // Play using the manager's AudioSource.
            _audioSource.PlayOneShot(clipData.Clip, clipData.Volume);
        }

        /// <summary>
        /// Plays a raw AudioClip immediately using the SoundManager's own AudioSource.
        /// Does *not* use the object pool.
        /// </summary>
        /// <param name="clip">The raw AudioClip to play.</param>
        /// <param name="volume">The desired volume scale (0.0 to 1.0).</param>
        public void PlayOneShot(AudioClip clip, float volume = 1)
        {
            if (clip == null)
            {
                Debug.LogWarning("Attempted to PlayOneShot a null AudioClip.");
                return;
            }
            // Play using the manager's AudioSource.
            _audioSource.PlayOneShot(clip, volume);
        }

        /// <summary>
        /// Plays a sound immediately using the SoundManager's own AudioSource.
        /// Uses the potentially randomized volume from the OM_AudioClip definition.
        /// Does *not* use the object pool. Ignores pitch randomization and position.
        /// </summary>
        /// <param name="clip">The OM_AudioClip definition containing the AudioClip and volume/pitch ranges.</param>
        public void PlayOneShot(OM_AudioClip clip)
        {
             // Basic check to ensure the clip is playable.
            if (!clip.CanBePlayed())
            {
                Debug.LogWarning($"Attempted to PlayOneShot an invalid OM_AudioClip (ID: {clip.Id}).");
                return;
            }
            // Play using the manager's AudioSource, getting a volume value from the definition.
            _audioSource.PlayOneShot(clip.Clip, clip.GetVolume());
        }

        // --- Utility Methods ---

        /// <summary>
        /// Retrieves all active OM_AudioPlayer instances currently playing a specific audio ID.
        /// </summary>
        /// <param name="audioId">The ID of the audio clip to search for.</param>
        /// <returns>An enumerable collection of matching active OM_AudioPlayer instances.</returns>
        public IEnumerable<OM_AudioPlayer> GetAudioPlayers(int audioId)
        {
            // Using yield return creates an iterator block, efficiently returning players one by one.
            foreach (var activePlayer in _activePlayers)
            {
                if (activePlayer.AudioId == audioId)
                {
                    yield return activePlayer;
                }
            }
            // Alternative using LINQ (potentially less performant due to allocation):
            // return _activePlayers.Where(player => player.AudioId == audioId);
        }


        // --- Update Loop ---

        /// <summary>
        /// Called every frame by Unity.
        /// Checks all active OM_AudioPlayer instances and returns any that have finished playing
        /// back to the object pool.
        /// </summary>
        private void Update()
        {
            // Iterate backwards through the list to safely remove items while iterating.
            for (var i = _activePlayers.Count - 1; i >= 0; i--)
            {
                var player = _activePlayers[i];

                // If the player's AudioSource component is still playing, skip it.
                 // Note: isPlaying can sometimes remain true for a frame after sound stops.
                 // More robust checks might involve checking time or clip status if needed.
                if (player.AudioSource.isPlaying) continue;

                // If the player is no longer playing, release it back to the pool.
                // This automatically calls OnAudioPlayerRelease via the pool's configuration.
                AudioPlayerPool.Release(player);
            }
        }
    }
}