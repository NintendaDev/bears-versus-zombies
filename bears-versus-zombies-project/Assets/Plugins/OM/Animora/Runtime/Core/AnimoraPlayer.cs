using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OM.TimelineCreator.Runtime;
using UnityEngine;

namespace OM.Animora.Runtime
{
    /// <summary>
    /// The main runtime component responsible for playing back timelines composed of <see cref="AnimoraClip"/> instances.
    /// It manages the overall playback state (Playing, Paused, Stopped), time progression, looping behavior,
    /// clip management (via <see cref="AnimoraClipsManager"/>), event handling, and interaction with the editor preview system.
    /// It acts as the central orchestrator for an Animora animation sequence.
    /// </summary>
    public class AnimoraPlayer : MonoBehaviour, IOM_TimelinePlayer<AnimoraClip> // Implements the generic timeline player interface for AnimoraClip
    {
        /// <summary>
        /// Internal enum defining flags for selective debug logging within the AnimoraPlayer.
        /// Allows enabling/disabling specific log messages via the inspector.
        /// </summary>
        [Flags]
        private enum AnimoraPlayerDebugFlags
        {
            None = 0, // No debug logs
            StartPlaying = 1 << 0, // Log when the main playback sequence starts
            CompletePlaying = 1 << 1, // Log when the main playback sequence completes
            StartTimeline = 1 << 2, // Log when a loop iteration (or the initial start) begins
            CompleteTimeline = 1 << 3, // Log when a loop iteration completes
        }

        // --- Public Events ---

        /// <summary>
        /// Invoked when the player's playback state (<see cref="OM_PlayState.Playing"/>, <see cref="OM_PlayState.Paused"/>, <see cref="OM_PlayState.Stopped"/>) changes.
        /// </summary>
        public event Action<OM_PlayState> OnPlayStateChanged;
        /// <summary>
        /// Invoked when the player requests a refresh of the editor UI (e.g., after loading save data).
        /// Typically subscribed to by the associated Timeline Editor window/inspector.
        /// </summary>
        public event Action OnTriggerEditorRefresh;
        /// <summary>
        /// Invoked frequently during playback (usually every frame via Evaluate) with the current elapsed time.
        /// Primarily used by the editor UI to update the time cursor position.
        /// </summary>
        public event Action<float> OnElapsedTimeChangedCallback;
        /// <summary>
        /// Invoked when the player's serialized data is validated (e.g., in `OnValidate`).
        /// Allows editor UI to refresh if player settings change.
        /// </summary>
        public event Action OnPlayerValidateCallback;
        /// <summary>
        /// Invoked whenever a clip is added to or removed from the player's clip manager.
        /// Signals the editor UI to update the track list.
        /// </summary>
        public event Action OnClipAddedOrRemoved;
        /// <summary>
        /// Invoked specifically when a clip is added, providing the added clip instance.
        /// </summary>
        public event Action<AnimoraClip> OnClipAdded;
        /// <summary>
        /// Invoked specifically when a clip is removed, providing the removed clip instance.
        /// </summary>
        public event Action<AnimoraClip> OnClipRemoved;

        // --- Serialized Fields (Configurable in Inspector) ---

        [SerializeField] private string playerUniqueID = "AnimoraPlayer"; 
        
        /// <summary>
        /// Index of the currently selected clip in the editor UI. Used for syncing selection state.
        /// Hidden in the default inspector but used by custom editor/timeline UI.
        /// </summary>
        [SerializeField, OM_HideInInspector] // Assuming OM_HideInInspector hides this from default inspector
        private int selectedClipIndex = -1;

        /// <summary>
        /// Determines when the animation playback should automatically start.
        /// </summary>
        [SerializeField]
        [Tooltip("When the animation should automatically start playing.")]
        private AnimoraPlayMode playMode = AnimoraPlayMode.OnStart;

        /// <summary>
        /// Specifies whether the playback time should be affected by Time.timeScale (ScaledTime) or run independently (UnscaledTime).
        /// </summary>
        [SerializeField]
        [Tooltip("Determines if playback speed is affected by Time.timeScale.")]
        private OM_TimeMode timeMode = OM_TimeMode.ScaledTime;

        /// <summary>
        /// The default direction for playback when starting or looping in certain modes.
        /// </summary>
        [SerializeField]
        [Tooltip("The initial direction of playback.")]
        private OM_PlayDirection defaultPlayDirection = OM_PlayDirection.Forward;

        /// <summary>
        /// Defines the looping behavior of the timeline (Loop indefinitely, PingPong back and forth, play Once).
        /// </summary>
        [SerializeField]
        [Tooltip("How the timeline behaves when reaching the end: Loop, PingPong, or play Once.")]
        private AnimoraPlayLoopType playLoopType = AnimoraPlayLoopType.Loop;

        /// <summary>
        /// Flags to enable specific debug log messages for this player instance.
        /// </summary>
        [SerializeField]
        [Tooltip("Enable specific debug logs for this player.")]
        private AnimoraPlayerDebugFlags debugFlags = AnimoraPlayerDebugFlags.None;

        /// <summary>
        /// The total duration of the timeline in seconds. Clips outside this duration might not be evaluated correctly depending on logic.
        /// </summary>
        [SerializeField, Min(0)] // Ensure duration cannot be negative in the inspector
        [Tooltip("Total duration of the timeline in seconds.")]
        private float timelineDuration = 2;

        /// <summary>
        /// Playback speed multiplier. 1 is normal speed, 2 is double speed, 0.5 is half speed.
        /// </summary>
        [SerializeField, Min(0)] // Ensure playback speed cannot be negative
        [Tooltip("Playback speed multiplier (1 = normal speed).")]
        private float playbackSpeed = 1;

        /// <summary>
        /// Number of times the timeline should loop. Set to -1 for infinite looping when playLoopType is Loop or PingPong.
        /// Used in conjunction with <see cref="AnimoraPlayLoopType"/>. Ignored if playLoopType is Once.
        /// </summary>
        [SerializeField, AnimoraLoopCount(0, "endless loop"), Min(-1)] // Custom attribute likely provides better UI, Min enforces minimum value
        [Tooltip("Number of loops (-1 for infinite). Only used for Loop and PingPong modes.")]
        private int loopCount = 1; // Default to playing once (0 loops after initial play) if Loop/PingPong, or ignored if Once

        /// <summary>
        /// Container for UnityEvents triggered at various points in the player's lifecycle (StartPlaying, CompletePlaying, etc.).
        /// Allows designers to hook up actions in the inspector without coding.
        /// </summary>
        [SerializeField, OM_Group("Events", "Events", "enabled")] // Assuming OM_Group provides inspector grouping
        [Tooltip("Events triggered during the player lifecycle.")]
        private AnimoraPlayerEvents animoraPlayerEvents;

        /// <summary>
        /// Manages the collection of <see cref="AnimoraClip"/> data instances associated with this player.
        /// Hidden in the default inspector, managed via custom editor UI.
        /// </summary>
        [SerializeField, OM_HideInInspector] // Hide from default inspector
        private AnimoraClipsManager clipsManager;

        // --- Getters and Public Properties ---

        /// <summary>
        /// Gets the manager responsible for handling the collection of <see cref="AnimoraClip"/> instances.
        /// Implements the <see cref="IOM_TimelinePlayer{T}.ClipsManager"/> property.
        /// </summary>
        public OM_ClipsManager<AnimoraClip> ClipsManager => clipsManager;

        /// <summary>
        /// Gets the current playback state of the player (Stopped, Playing, Paused).
        /// The setter is private, controlled via <see cref="SetPlayState"/>.
        /// </summary>
        public OM_PlayState PlayState { get; private set; } = OM_PlayState.Stopped; // Initialize to Stopped

        /// <summary>
        /// Gets the current elapsed time of the playback within the timeline duration.
        /// The setter is private, controlled via <see cref="SetElapsedTime"/> (which clamps the value).
        /// </summary>
        public float ElapsedTime { get; private set; }

        /// <summary>
        /// Gets the container for UnityEvents associated with this player.
        /// Provides easy access to the configured events.
        /// </summary>
        public AnimoraPlayerEvents AnimoraPlayerEvents => animoraPlayerEvents;

        public string PlayerUniqueID => playerUniqueID;

        /// <summary>
        /// Gets the index of the currently selected clip in the editor.
        /// Implements the <see cref="IOM_TimelinePlayer{T}.SelectedClipIndex"/> property.
        /// Setter is private, intended to be controlled via <see cref="SetSelectedClipIndex"/>.
        /// </summary>
        public int SelectedClipIndex
        {
            get => selectedClipIndex;
            private set => selectedClipIndex = value; // Private setter
        }

        // --- Private Internal State Variables ---

        /// <summary>
        /// Flag indicating if the playback sequence has been started at least once since initialization or last stop.
        /// Used by <see cref="Evaluate"/> to handle the first frame logic.
        /// </summary>
        private bool _hasStarted;
        /// <summary>
        /// Flag indicating if the player is currently attempting to play (true when playing or paused, false when stopped).
        /// Checked in <see cref="Update"/> to process time progression.
        /// </summary>
        private bool _playing;
        /// <summary>
        /// Flag indicating if the player is currently in editor preview scrubbing mode.
        /// Set via <see cref="OnPreviewStateChanged"/>.
        /// </summary>
        private bool _isPreviewing;
        /// <summary>
        /// Counter for the number of loops completed during playback. Used with <see cref="loopCount"/>.
        /// </summary>
        private int _currentLoop;
        /// <summary>
        /// The current direction of playback (Forward or Backward). Can change during PingPong mode.
        /// </summary>
        private OM_PlayDirection _currentPlayDirection;

        /// <summary>
        /// Gets the read-only list of clips that are currently active and being evaluated during playback.
        /// This list is populated when playback starts based on `clipsManager` and `CanBePlayed` status.
        /// The setter is private.
        /// </summary>
        public IReadOnlyList<AnimoraClip> ClipsToPlay { get; private set; }

        /// <summary>
        /// Initializes the player components, specifically the <see cref="clipsManager"/>, for use within the Unity Editor.
        /// Ensures the manager exists and cleans up any null clip entries.
        /// Typically called by the associated custom editor or timeline UI.
        /// </summary>
        public void InitPlayerForEditor()
        {
            // Lazily initialize the clips manager if it doesn't exist.
            clipsManager ??= new AnimoraClipsManager();
            // Call the manager's editor initialization method.
            clipsManager.InitForEditor();
        }

        #region Unity Callbacks

        /// <summary>
        /// Unity callback function. Called in the editor when the script is loaded or a value is changed in the Inspector.
        /// Ensures values are clamped, initializes the clip manager for the editor, and invokes validation callbacks.
        /// </summary>
        public void OnValidate()
        {
            // Clamp duration and playback speed to non-negative values.
            timelineDuration = Mathf.Max(0, timelineDuration);
            playbackSpeed = Mathf.Max(0, playbackSpeed);
            // Clamp loop count (allow -1 for infinite).
            loopCount = Mathf.Max(-1, loopCount);

            // Invoke the validation callback for external listeners (like editor UI).
            OnPlayerValidateCallback?.Invoke();
            // Validate the clip manager and its clips, passing this player instance for context.
            // The null-conditional operator (?.) prevents errors if clipsManager is null.
            clipsManager?.OnValidate(this);
        }

        /// <summary>
        /// Unity callback function. Called on the frame when a script is enabled before any of the Update methods are called the first time.
        /// If <see cref="playMode"/> is set to <see cref="AnimoraPlayMode.OnStart"/>, starts the animation playback.
        /// </summary>
        private void Start()
        {
            // Automatically start playback if configured to do so on Start.
            if (playMode == AnimoraPlayMode.OnStart)
            {
                PlayAnimation();
            }
        }

        /// <summary>
        /// Unity callback function. Called when the object becomes enabled and active.
        /// If <see cref="playMode"/> is set to <see cref="AnimoraPlayMode.OnEnable"/>, starts the animation playback.
        /// </summary>
        private void OnEnable()
        {
            AnimoraManager.RegisterAnimoraPlayer(this);
            // Automatically start playback if configured to do so on Enable.
            if (playMode == AnimoraPlayMode.OnEnable)
            {
                PlayAnimation();
            }
        }

        /// <summary>
        /// Unity callback function. Called when the MonoBehaviour is disabled or destroyed.
        /// </summary>
        private void OnDisable()
        {
            // Stop playback if the player is disabled.
            StopAnimation();
            // Unregister from the AnimoraManager to clean up references.
            AnimoraManager.UnregisterAnimoraPlayer(this);
        }

        /// <summary>
        /// Unity callback function. Called every frame if the component is enabled.
        /// Handles time progression based on <see cref="timeMode"/> and <see cref="playbackSpeed"/>.
        /// Calls the <see cref="Evaluate"/> method to update the state of the timeline and clips.
        /// </summary>
        private void Update()
        {
            // Only process time if playback is active (not stopped).
            if (_playing == false) return;
            // If paused, skip time progression and evaluation.
            if (PlayState == OM_PlayState.Paused) return;

            // Calculate the time delta for this frame based on the selected time mode.
            float deltaTime = timeMode.GetDeltaTime(); // Uses Time.deltaTime or Time.unscaledDeltaTime

            // Calculate the change in elapsed time based on delta time, playback speed, and direction.
            float timeIncrement = deltaTime * playbackSpeed * _currentPlayDirection.GetDirectionMultiplier(); // Multiplier is 1 for Forward, -1 for Backward

            // Calculate the new potential elapsed time.
            var newElapsedTime = ElapsedTime + timeIncrement;

            // Clamp the new time within the valid timeline range [0, timelineDuration].
            newElapsedTime = Mathf.Clamp(newElapsedTime, 0, GetTimelineDuration());

            // Evaluate the timeline state at the new elapsed time.
            Evaluate(newElapsedTime);
        }

        #endregion

        #region Play Methods

        /// <summary>
        /// Starts or restarts the animation playback sequence from the beginning.
        /// If already playing, it stops the current playback first.
        /// Sets up the initial state (time, loop count, direction) and calls <see cref="StartPlayingAndStartFirstLoop"/>.
        /// </summary>
        public void PlayAnimation()
        {
            // If already playing, stop the current playback to ensure a clean restart.
            if (_playing)
            {
                StopAnimation(); // Stop first ensures proper cleanup and state reset
            }

            // --- Reset state for new playback ---
            _hasStarted = false; // Mark as not started yet (first Evaluate call will handle setup)
            _playing = true; // Set the flag to indicate playback is now active
            _currentLoop = 0; // Reset loop counter
            //_currentPlayDirection = defaultPlayDirection; // Set initial direction (handled in StartPlayingAndStartFirstLoop)
            //ElapsedTime = GetElapsedTimeStartTime(_currentPlayDirection); // Set initial time (handled in StartPlayingAndStartFirstLoop)

             // Set the initial play state to Playing (invokes OnPlayStateChanged event)
             // Note: Setting state is now handled inside StartPlayingAndStartFirstLoop

            // Start the actual playback logic (initial setup and first frame evaluation)
            // This is now deferred to the first call to Evaluate, triggered by the Update loop
            // The following line would force immediate setup, but letting Evaluate handle it is cleaner:
            // StartPlayingAndStartFirstLoop(defaultPlayDirection, false);
        }

        /// <summary>
        /// Starts or restarts the animation playback as a coroutine.
        /// Useful for integrating timeline playback with other coroutine-based logic.
        /// NOTE: This implementation currently just sets flags and relies on the Update loop for progression.
        /// A true coroutine implementation would likely involve `yield return` within the loop logic itself,
        /// potentially making the Update loop redundant for coroutine playback.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> for the coroutine.</returns>
        public IEnumerator PlayAnimationCoroutine()
        {
            // If already playing, stop first for a clean restart.
            if (_playing)
            {
                StopAnimation();
            }

            // Reset state.
            _hasStarted = false;
            _playing = true;
            _currentLoop = 0;
             // SetPlayState(OM_PlayState.Playing); // State is set in StartPlayingAndStartFirstLoop via Evaluate

            // The yield return null simply waits for the next frame.
            // The actual time progression and evaluation happen in the Update method.
            // This makes the coroutine aspect somewhat superficial in the current structure.
            while (_playing) // Loop as long as playback is active
            {
                yield return null; // Wait for the next frame, Update will handle evaluation
            }
        }

        /// <summary>
        /// Starts or restarts the animation playback asynchronously using Tasks.
        /// Useful for async/await patterns.
        /// NOTE: Similar to the coroutine version, this implementation relies on the Update loop.
        /// A fully async implementation might require a different approach to time management
        /// that doesn't depend on MonoBehaviour's Update.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous playback operation.</returns>
        public async Task PlayAnimationAsync()
        {
            // If already playing, stop first.
            if (_playing)
            {
                StopAnimation();
            }

            // Reset state.
            _hasStarted = false;
            _playing = true;
            _currentLoop = 0;
            // SetPlayState(OM_PlayState.Playing); // State is set in StartPlayingAndStartFirstLoop via Evaluate

            // The Task.Yield allows other async operations to proceed.
            // Like the coroutine, the actual work happens in Update.
            while (_playing) // Loop as long as playback is active
            {
                await Task.Yield(); // Yield control back, Update handles progression
            }
        }

        /// <summary>
        /// Resumes playback if the animation is currently paused.
        /// Sets the <see cref="PlayState"/> to <see cref="OM_PlayState.Playing"/> and notifies clips.
        /// </summary>
        public void ResumeAnimation()
        {
            // Cannot resume if called from editor code while editor is not in play mode.
            if (Application.isPlaying == false) return;
            // Only resume if currently paused.
            if (PlayState != OM_PlayState.Paused) return;

            // Set the state back to Playing.
            SetPlayState(OM_PlayState.Playing);

            // Notify all active clips that playback is resuming.
            if (ClipsToPlay != null)
            {
                foreach (var clip in ClipsToPlay)
                {
                    clip?.OnResume(); // Call OnResume on each clip
                }
            }
            // Invoke the corresponding UnityEvent and Action delegate.
            animoraPlayerEvents?.InvokeOnResume(); // Use null-conditional operator
        }

        /// <summary>
        /// Pauses playback if the animation is currently playing.
        /// Sets the <see cref="PlayState"/> to <see cref="OM_PlayState.Paused"/> and notifies clips.
        /// </summary>
        public void PauseAnimation()
        {
            // Cannot pause if called from editor code while editor is not in play mode.
            if (Application.isPlaying == false) return;
            // Only pause if currently playing.
            if (PlayState != OM_PlayState.Playing) return;

            // Set the state to Paused.
            SetPlayState(OM_PlayState.Paused);

            // Notify all active clips that playback is pausing.
            if (ClipsToPlay != null)
            {
                foreach (var clip in ClipsToPlay)
                {
                    clip?.OnPause(); // Call OnPause on each clip
                }
            }
            // Invoke the corresponding UnityEvent and Action delegate.
            animoraPlayerEvents?.InvokeOnPause();
        }

        /// <summary>
        /// Stops playback completely.
        /// Resets playback state, elapsed time, internal flags, and notifies clips.
        /// Sets the <see cref="PlayState"/> to <see cref="OM_PlayState.Stopped"/>.
        /// </summary>
        public void StopAnimation()
        {
            // Cannot stop if called from editor code while editor is not in play mode (usually irrelevant for Stop).
            if (Application.isPlaying == false && !_isPreviewing) return; // Allow stop if previewing
            // Only stop if not already stopped.
            if (PlayState == OM_PlayState.Stopped) return;

            // Clear the playing flag to stop the Update loop processing.
            _playing = false;
            // Set the state to Stopped.
            SetPlayState(OM_PlayState.Stopped);
            // Reset elapsed time to 0.
            SetElapsedTime(0);

            // Notify all previously active clips that playback is stopping.
            if (ClipsToPlay != null)
            {
                foreach (var clip in ClipsToPlay)
                {
                    clip?.OnStop(); // Call OnStop on each clip
                }
            }
            // Clear the list of clips being played.
            ClipsToPlay = null;
            // Invoke the corresponding UnityEvent and Action delegate.
            animoraPlayerEvents?.InvokeOnStop();

             // Reset internal flags
             _hasStarted = false;
             _currentLoop = 0;
        }

        #endregion

        /// <summary>
        /// Core evaluation method called every frame during playback (from Update) or when scrubbing.
        /// Updates the <see cref="ElapsedTime"/> and determines if the timeline has reached its end
        /// based on the current <paramref name="time"/> and <see cref="_currentPlayDirection"/>.
        /// Handles loop completion, state transitions (calling <see cref="CompleteLoop"/>, <see cref="StartLoop"/>, <see cref="CompletePlaying"/>),
        /// and delegates the evaluation of individual clips to <see cref="AnimoraClipsPlayUtility.Evaluate"/>.
        /// </summary>
        /// <param name="time">The current time point to evaluate at (usually the new target ElapsedTime).</param>
        public void Evaluate(float time)
        {
            // Update the internal elapsed time state (and invoke callback).
            SetElapsedTime(time);

            // --- First Frame Logic ---
            // If playback just started (_hasStarted is false), perform initial setup.
            if (_hasStarted == false)
            {
                // Initialize playback state, direction, reset clips, and evaluate the very first frame.
                StartPlayingAndStartFirstLoop(defaultPlayDirection, false); // false indicates not preview mode

                // Evaluate clips at the initial starting time.
                AnimoraClipsPlayUtility.Evaluate(ClipsToPlay, GetElapsedTimeStartTime(_currentPlayDirection), _currentPlayDirection);
                // Skip further evaluation on this frame as setup is complete.
                return;
            }

            // --- Check for Timeline End ---
            bool reachedEnd = false;
            // Condition depends on playback direction.
            if (_currentPlayDirection == OM_PlayDirection.Forward)
            {
                reachedEnd = time >= timelineDuration; // Reached end if time >= duration
            }
            else // Backward playback
            {
                reachedEnd = time <= 0; // Reached end if time <= 0
            }

            // --- Handle Reaching Timeline End ---
            if (reachedEnd)
            {
                // Evaluate clips one last time exactly at the boundary (duration or 0).
                float boundaryTime = (_currentPlayDirection == OM_PlayDirection.Forward) ? GetTimelineDuration() : 0;
                AnimoraClipsPlayUtility.Evaluate(ClipsToPlay, boundaryTime, _currentPlayDirection);

                // Signal completion of the current loop iteration.
                CompleteLoop(); // Notifies clips, invokes events
                // Increment the loop counter.
                _currentLoop++;

                // --- Loop Type Logic ---
                // 1. Play Once: Stop playback immediately after the first completion.
                if (playLoopType == AnimoraPlayLoopType.Once)
                {
                    CompletePlaying(); // Signal completion of the entire sequence
                    return; // End evaluation for this frame
                }

                // 2. Loop Count Check: Stop if the loop count is reached (and not infinite).
                if (loopCount != -1 && _currentLoop >= loopCount) // Check loop count (-1 means infinite)
                {
                    CompletePlaying(); // Signal completion
                    return; // End evaluation
                }

                // 3. PingPong Mode: Reverse direction and start the next loop.
                if (playLoopType == AnimoraPlayLoopType.PingPong)
                {
                    _currentPlayDirection = _currentPlayDirection.Reverse(); // Reverse direction
                    StartLoop(_currentPlayDirection); // Setup for the next loop in the new direction
                    // Evaluate immediately at the start of the new loop direction
                    Evaluate(GetElapsedTimeStartTime(_currentPlayDirection));
                    return; // End evaluation for this frame
                }

                // 4. Loop Mode: Reset direction to default and start the next loop.
                if (playLoopType == AnimoraPlayLoopType.Loop)
                {
                    _currentPlayDirection = defaultPlayDirection; // Reset to default direction
                    StartLoop(_currentPlayDirection); // Setup for the next loop
                     // Evaluate immediately at the start of the new loop
                    Evaluate(GetElapsedTimeStartTime(_currentPlayDirection));
                    return; // End evaluation for this frame
                }
                 // Note: If loop type is invalid or somehow missed, playback might stall here.
                 // Added a final return for safety.
                 return;
            }

            // --- Normal Evaluation ---
            // If the end of the timeline was not reached, evaluate clips at the current time.
            AnimoraClipsPlayUtility.Evaluate(ClipsToPlay, ElapsedTime, _currentPlayDirection);
        }

        /// <summary>
        /// Performs the initial setup when playback starts for the very first time or restarts.
        /// Initializes the clip manager, resets clips, determines the list of clips to play,
        /// sets initial state values, and invokes startup events.
        /// </summary>
        /// <param name="playDirection">The initial playback direction.</param>
        /// <param name="previewMode">Indicates if starting in preview mode.</param>
        public void StartPlayingAndStartFirstLoop(OM_PlayDirection playDirection, bool previewMode)
        {
            // Ensure the clip manager is initialized.
            clipsManager.Init();

            // Reset all clips to their initial state before any playback begins.
            AnimoraClipsPlayUtility.ResetBeforePlay(ClipsManager.GetClips(), previewMode, playDirection);
            // Reset clips specifically for the start of the first loop iteration.
            AnimoraClipsPlayUtility.ResetBeforeStartLoop(ClipsManager.GetClips(), previewMode, playDirection);

            // Set initial playback state variables.
            _hasStarted = true; // Mark as started
            _currentPlayDirection = playDirection; // Set the starting direction
            _currentLoop = 0; // Reset loop counter
            // Set the initial elapsed time based on direction (0 for Forward, duration for Backward).
            ElapsedTime = GetElapsedTimeStartTime(_currentPlayDirection);

            // Set the overall playback state to Playing.
            SetPlayState(OM_PlayState.Playing);

            // Determine the list of clips to actually evaluate during playback.
            // Filters based on CanBePlayed() and potentially CanBePreviewed() if in preview mode.
            // Orders clips by their OrderIndex.
            ClipsToPlay = previewMode
                ? clipsManager.GetClips() // Get all clips...
                    .Where(x => x != null && x.CanBePlayed() && x.CanBePreviewed(this)) // Filter for playable & previewable
                    .OrderBy(x => x.OrderIndex).ToList() // Order them
                : clipsManager.GetClips() // Get all clips...
                    .Where(x => x != null && x.CanBePlayed()) // Filter for playable only
                    .OrderBy(x => x.OrderIndex).ToList(); // Order them

            // --- Invoke Start Events ---
            // Log start event if debug flag is enabled.
            OM_Debug.Log("On Start Playing", debugFlags.HasFlag(AnimoraPlayerDebugFlags.StartPlaying), this);
            // Invoke the UnityEvent and Action delegate for starting playback.
            animoraPlayerEvents?.InvokeOnStartPlaying();
            // Call the internal hook method (currently empty).
            OnStartPlaying();

            // Notify the utility to call OnStartPlaying on each active clip.
            AnimoraClipsPlayUtility.StartPlaying(ClipsToPlay, this, previewMode, playDirection);

            // --- Start First Loop Events --- (These often overlap with StartPlaying but represent the loop concept)
             // Log timeline start event if debug flag is enabled.
            OM_Debug.Log("On Start Timeline", debugFlags.HasFlag(AnimoraPlayerDebugFlags.StartTimeline), this);
             // Invoke the UnityEvent and Action delegate for starting the timeline/loop.
            animoraPlayerEvents?.InvokeOnStartLoop();
            // Call the internal hook method.
            OnStartLoop();

            // Notify the utility to call OnStartLoop on each active clip.
            AnimoraClipsPlayUtility.StartLoop(ClipsToPlay, previewMode, playDirection);
        }


        /// <summary>
        /// Finalizes the entire playback sequence.
        /// Sets state to Stopped, resets time, notifies clips via utility, and invokes completion events.
        /// </summary>
        public void CompletePlaying()
        {
             // Don't complete if already stopped
             if (PlayState == OM_PlayState.Stopped) return;

            // Reset elapsed time and playing flag.
            SetElapsedTime(0);
            _playing = false; // Stop the Update loop processing
            // Set the final playback state.
            SetPlayState(OM_PlayState.Stopped);

            // Notify the utility to call OnCompletePlaying on each previously active clip.
            AnimoraClipsPlayUtility.CompletePlaying(ClipsToPlay); // Use ClipsToPlay as they were the ones playing

            // Log completion event if debug flag is enabled.
            OM_Debug.Log("On Complete Playing", debugFlags.HasFlag(AnimoraPlayerDebugFlags.CompletePlaying), this);
            // Invoke the UnityEvent and Action delegate for completing playback.
            animoraPlayerEvents?.InvokeOnCompletePlaying();
            // Call the internal hook method.
            OnCompletePlaying();

            // Clear the list of clips being played.
             ClipsToPlay = null;
             // Reset internal state flags
             _hasStarted = false;
             _currentLoop = 0;
        }

        /// <summary>
        /// Sets up the timeline for the beginning of a new loop iteration.
        /// Resets elapsed time, sets state to Playing, resets clips for the loop, and invokes loop start events.
        /// </summary>
        /// <param name="playDirection">The playback direction for this new loop iteration.</param>
        public void StartLoop(OM_PlayDirection playDirection)
        {
            // Ensure we have clips to play (safeguard against calling StartLoop inappropriately).
            if (ClipsToPlay == null)
            {
                 // This indicates an issue, likely calling StartLoop without PlayAnimation first.
                Debug.LogError("AnimoraPlayer: ClipsToPlay is null in StartLoop. Was PlayAnimation called first?", this);
                 // Attempt recovery by restarting? Or just return? Returning is safer.
                 // PlayAnimation(); // Potential recovery, but might cause infinite loops if called incorrectly.
                return;
            }

            // Set the elapsed time to the beginning based on the loop's direction.
            ElapsedTime = GetElapsedTimeStartTime(playDirection);
            SetElapsedTime(ElapsedTime); // Update state and invoke callback

            // Ensure the play state is set to Playing.
            SetPlayState(OM_PlayState.Playing);

            // Reset all clips specifically for the start of this new loop iteration.
            // Use clipsManager.GetClips() here to reset *all* potential clips, not just ClipsToPlay,
            // as some might become playable again in PingPong mode. Consider if ClipsToPlay is sufficient.
            AnimoraClipsPlayUtility.ResetBeforeStartLoop(clipsManager.GetClips(), _isPreviewing, playDirection); // Reset all clips managed

            // Notify the utility to call OnStartLoop on the clips that will be evaluated this loop.
            AnimoraClipsPlayUtility.StartLoop(ClipsToPlay, _isPreviewing, playDirection);

            // Log loop start event if debug flag is enabled.
            OM_Debug.Log("On Start Timeline", debugFlags.HasFlag(AnimoraPlayerDebugFlags.StartTimeline), this);
            // Invoke the UnityEvent and Action delegate for starting a loop.
            animoraPlayerEvents?.InvokeOnStartLoop();
            // Call the internal hook method.
            OnStartLoop();

             // Evaluate the first frame of the new loop immediately.
             // AnimoraClipsPlayUtility.Evaluate(ClipsToPlay, ElapsedTime, playDirection); // Now handled by the caller (Evaluate) or next Update
        }

        /// <summary>
        /// Finalizes the current loop iteration.
        /// Notifies clips via utility, invokes loop completion events, and potentially sets state to Stopped (if needed, though usually handled by Evaluate/CompletePlaying).
        /// </summary>
        public void CompleteLoop()
        {
            // Notify the utility to call OnCompleteLoop on each active clip.
            AnimoraClipsPlayUtility.CompleteLoop(ClipsToPlay);

            // Log loop completion event if debug flag is enabled.
            OM_Debug.Log("On Complete Timeline", debugFlags.HasFlag(AnimoraPlayerDebugFlags.CompleteTimeline), this);
            // Invoke the UnityEvent and Action delegate for completing a loop.
            animoraPlayerEvents?.InvokeOnCompleteLoop();
            // Call the internal hook method.
            OnCompleteLoop();

            // Setting state to Stopped here might be premature if another loop follows.
            // Evaluate/CompletePlaying handles the final state change.
            // SetPlayState(OM_PlayState.Stopped);
        }

        // --- Internal Hook Methods (Can be overridden by derived classes if needed) ---

        /// <summary> Hook called internally when playback starts. </summary>
        private void OnStartPlaying() { /* Base implementation does nothing */ }
        /// <summary> Hook called internally when playback completes. </summary>
        private void OnCompletePlaying() { /* Base implementation does nothing */ }
        /// <summary> Hook called internally when a loop iteration starts. </summary>
        private void OnStartLoop() { /* Base implementation does nothing */ }
        /// <summary> Hook called internally when a loop iteration completes. </summary>
        private void OnCompleteLoop() { /* Base implementation does nothing */ }

        #region Getters and Setters

        /// <summary>
        /// Sets the player's playback state and invokes the <see cref="OnPlayStateChanged"/> event.
        /// </summary>
        /// <param name="newState">The new playback state.</param>
        public void SetPlayState(OM_PlayState newState)
        {
            // Only update and invoke event if the state has actually changed.
            if (newState == PlayState) return;
            PlayState = newState;
            OnPlayStateChanged?.Invoke(newState); // Notify listeners
        }

        /// <summary>
        /// Gets the appropriate starting time for playback based on the direction.
        /// </summary>
        /// <param name="playDirection">The direction of playback.</param>
        /// <returns>0 if playing Forward, timelineDuration if playing Backward.</returns>
        private float GetElapsedTimeStartTime(OM_PlayDirection playDirection)
        {
            return playDirection == OM_PlayDirection.Forward ? 0 : GetTimelineDuration();
        }

        /// <summary>
        /// Sets the index of the selected clip (primarily for editor UI synchronization).
        /// Implements the setter part of <see cref="IOM_TimelinePlayer{T}.SetSelectedClipIndex"/>.
        /// </summary>
        /// <param name="index">The new selected clip index.</param>
        public void SetSelectedClipIndex(int index)
        {
            SelectedClipIndex = index; // Uses the private setter of the property
        }

        /// <summary>
        /// Gets the index of the selected clip.
        /// Implements the getter part of <see cref="IOM_TimelinePlayer{T}.GetSelectedClipIndex"/>.
        /// </summary>
        /// <returns>The selected clip index.</returns>
        public int GetSelectedClipIndex()
        {
            return SelectedClipIndex; // Uses the getter of the property
        }

        /// <summary>
        /// Gets the total duration of the timeline.
        /// Implements <see cref="IOM_TimelinePlayer{T}.GetTimelineDuration"/>.
        /// </summary>
        /// <returns>The timeline duration in seconds.</returns>
        public float GetTimelineDuration()
        {
            return timelineDuration;
        }

        /// <summary>
        /// Sets the total duration of the timeline.
        /// Implements <see cref="IOM_TimelinePlayer{T}.SetTimelineDuration"/>.
        /// </summary>
        /// <param name="newDuration">The new duration in seconds.</param>
        public void SetTimelineDuration(float newDuration)
        {
            // Clamp the new duration to be non-negative.
            timelineDuration = Mathf.Max(0f, newDuration);
             // Optionally call OnValidate here if needed, though it's often called externally after this.
             // OnValidate();
        }

        /// <summary>
        /// Sets the current elapsed time of the player, clamping it within the valid range [0, duration].
        /// Invokes the <see cref="OnElapsedTimeChangedCallback"/> event.
        /// Implements <see cref="IOM_TimelinePlayer{T}.SetElapsedTime"/>.
        /// </summary>
        /// <param name="elapsedTimeParam">The desired elapsed time.</param>
        public void SetElapsedTime(float elapsedTimeParam)
        {
             // Clamp the value before assigning it
            ElapsedTime = Mathf.Clamp(elapsedTimeParam, 0, GetTimelineDuration());
            // Notify listeners about the time change.
            OnElapsedTimeChangedCallback?.Invoke(ElapsedTime);
        }

        #endregion

        #region Editor Functions

        /// <summary>
        /// Handles changes in the editor's preview mode state.
        /// Starts or stops the preview sequence, including resetting clips and evaluating the initial state.
        /// </summary>
        /// <param name="isPreviewingParam">True if preview mode is being activated, false if deactivated.</param>
        public void OnPreviewStateChanged(bool isPreviewingParam)
        {
            _isPreviewing = isPreviewingParam; // Update internal flag

            if (isPreviewingParam)
            {
                // --- Starting Preview ---
                // Ensure playback is stopped if it was running
                if(_playing) StopAnimation();

                // Notify clips that preview state is changing (before starting loop etc)
                AnimoraClipsPlayUtility.OnPreviewStateChanged(GetClips(), this, true); // True for starting

                // Setup the playback state for preview (similar to PlayAnimation but with preview flag)
                StartPlayingAndStartFirstLoop(OM_PlayDirection.Forward, true); // Start preview forward

                // Evaluate the initial frame (time 0) forcefully for preview
                AnimoraClipsPlayUtility.EvaluateForce(ClipsToPlay, 0);
            }
            else
            {
                // --- Stopping Preview ---
                // Ensure playback state reflects stopping preview
                CompleteLoop();      // Signal loop completion
                CompletePlaying();   // Signal sequence completion (sets state to Stopped)

                // Notify clips that preview state is changing (after stopping loop etc)
                AnimoraClipsPlayUtility.OnPreviewStateChanged(GetClips(), this, false); // False for stopping

                // Clear the list of clips being previewed
                ClipsToPlay = null;
            }
        }

        /// <summary>
        /// Evaluates the timeline specifically for editor preview scrubbing at a given time.
        /// Uses <see cref="AnimoraClipsPlayUtility.EvaluateForce"/> which bypasses normal Enter/Exit logic.
        /// </summary>
        /// <param name="time">The time point to evaluate the preview at.</param>
        public void EvaluatePreview(float time)
        {
            // // Basic safeguard - preview evaluation should only happen if in preview mode
            // if (!_isPreviewing) return; // Can cause issues if called slightly after stopping preview

            // Ensure we have a valid list of clips to evaluate (set during OnPreviewStateChanged(true))
            if (ClipsToPlay == null)
            {
                 // This might happen if called after OnPreviewStateChanged(false) but before Update stops calling it.
                 // Or if preview wasn't started correctly.
                // Debug.LogWarning("EvaluatePreview called but ClipsToPlay is null.", this);
                return;
            }
            // Force evaluate all active clips at the specified time for preview.
            AnimoraClipsPlayUtility.EvaluateForce(ClipsToPlay, time);
             // Update the logical elapsed time to match the preview time
             SetElapsedTime(time);
        }

        /// <summary>
        /// Records an undo operation using Unity's Undo system. Used for editor integration.
        /// Implements <see cref="IOM_TimelinePlayer{T}.RecordUndo"/>.
        /// Requires UNITY_EDITOR compilation directive.
        /// </summary>
        /// <param name="undoName">The name of the undo operation displayed in the editor's Undo history.</param>
        public void RecordUndo(string undoName)
        {
            // Only execute if running inside the Unity Editor.
            #if UNITY_EDITOR
            // Register the entire AnimoraPlayer component state for Undo.
            UnityEditor.Undo.RecordObject(this, undoName);
            #endif
        }

        /// <summary>
        /// Creates a data structure containing the current serializable state of the player.
        /// Used for saving the player's configuration.
        /// </summary>
        /// <returns>An <see cref="AnimoraPlayerSaveData"/> object.</returns>
        public AnimoraPlayerSaveData GetSaveData()
        {
            // Create a new save data object and populate it with current player settings.
            return new AnimoraPlayerSaveData()
            {
                playMode = this.playMode,
                timeMode = this.timeMode,
                timelineDuration = this.timelineDuration,
                playbackSpeed = this.playbackSpeed,
                loopCount = this.loopCount,
                animoraPlayerEvents = this.animoraPlayerEvents, // Assumes AnimoraPlayerEvents is serializable
                // Get a copy of the clips list from the manager.
                clips = this.clipsManager?.GetClips()?.ToList() ?? new List<AnimoraClip>() // Handle null manager/list
            };
        }

        /// <summary>
        /// Loads the player's state from a provided save data object.
        /// Records an Undo step before applying changes. Updates player settings and clips.
        /// </summary>
        /// <param name="saveData">The <see cref="AnimoraPlayerSaveData"/> object containing the state to load.</param>
        public void LoadSaveData(AnimoraPlayerSaveData saveData)
        {
            // Ensure save data is not null.
            if (saveData == null)
            {
                Debug.LogError("LoadSaveData received null saveData.", this);
                return;
            }

            // Record Undo operation before applying loaded data.
            RecordUndo("Load Animora Player");

            // Apply settings from save data to this player instance.
            this.playMode = saveData.playMode;
            this.timeMode = saveData.timeMode;
            this.timelineDuration = saveData.timelineDuration;
            this.playbackSpeed = saveData.playbackSpeed;
            this.loopCount = saveData.loopCount;
            this.animoraPlayerEvents = saveData.animoraPlayerEvents; // Assumes deep copy isn't needed or handled by serialization

            // Ensure clip manager exists before populating clips.
            this.clipsManager ??= new AnimoraClipsManager();
            // Populate the clip manager with the clips from the save data.
            // The PopulateWithClips method likely replaces the manager's internal list.
            this.clipsManager.PopulateWithClips(saveData.clips);

            // Trigger events to notify the editor/UI about the changes.
            OnPlayerValidateCallback?.Invoke(); // Ensure inspector reflects changes
            OnTriggerEditorRefresh?.Invoke(); // Ensure timeline UI refreshes
            OnValidate(); // Perform internal validation clamps/updates
        }

        #endregion

        #region Clips Methods (Delegated to ClipsManager via IOM_TimelinePlayer interface)

        /// <summary>
        /// Adds a clip data instance to the player's clip manager.
        /// Implements <see cref="IOM_TimelinePlayer{T}.AddClip"/>.
        /// </summary>
        /// <param name="clipToAdd">The <see cref="AnimoraClip"/> instance to add.</param>
        public void AddClip(AnimoraClip clipToAdd)
        {
            // Ensure manager exists.
            clipsManager ??= new AnimoraClipsManager();
            // Delegate adding the clip to the manager, passing context.
            clipsManager.AddClip(clipToAdd, this); // 'this' provides IOM_TimelinePlayer context for RecordUndo
            // Invoke events for UI updates.
            OnClipAddedOrRemoved?.Invoke();
            OnClipAdded?.Invoke(clipToAdd);
            // Validate after adding.
            OnValidate();
        }

        /// <summary>
        /// Removes a clip data instance from the player's clip manager.
        /// Implements <see cref="IOM_TimelinePlayer{T}.RemoveClip"/>.
        /// </summary>
        /// <param name="clipToRemove">The <see cref="AnimoraClip"/> instance to remove.</param>
        public void RemoveClip(AnimoraClip clipToRemove)
        {
             // Only proceed if manager and clip exist.
            if (clipsManager == null || clipToRemove == null) return;
            // Delegate removal to the manager.
            clipsManager.RemoveClip(clipToRemove, this);
            // Invoke events for UI updates.
            OnClipAddedOrRemoved?.Invoke();
            OnClipRemoved?.Invoke(clipToRemove);
            // Validate after removing.
            OnValidate();
        }

        /// <summary>
        /// Duplicates a clip data instance within the player's clip manager.
        /// Implements <see cref="IOM_TimelinePlayer{T}.DuplicateClip"/>.
        /// </summary>
        /// <param name="clipToDuplicate">The <see cref="AnimoraClip"/> instance to duplicate.</param>
        public void DuplicateClip(AnimoraClip clipToDuplicate)
        {
             // Only proceed if manager and clip exist.
            if (clipsManager == null || clipToDuplicate == null) return;
            // Delegate duplication to the manager.
            clipsManager.DuplicateClip(clipToDuplicate, this);
            // Invoke event for UI updates (a clip was effectively added).
            OnClipAddedOrRemoved?.Invoke();
            // OnClipAdded?.Invoke(newlyCreatedClip); // Need reference to new clip if Added event is desired
            // Validate after duplicating.
            OnValidate();
        }

        /// <summary>
        /// Gets the collection of all clip data instances managed by the player.
        /// Implements <see cref="IOM_TimelinePlayer{T}.GetClips"/>.
        /// </summary>
        /// <returns>An enumerable collection of <see cref="AnimoraClip"/>, or an empty collection if none exist.</returns>
        public IEnumerable<AnimoraClip> GetClips()
        {
            // Delegate retrieval to the manager. Return empty list if manager is null.
            return clipsManager?.GetClips() ?? Enumerable.Empty<AnimoraClip>();
        }

        #endregion
    }
}