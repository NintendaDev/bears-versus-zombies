using OM.Editor;
using OM.Animora.Runtime;
using OM.TimelineCreator.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.Animora.Editor
{
    /// <summary>
    /// Handles the playback control logic (Play, Pause, Stop, Replay) for the Animora timeline inside the Unity Editor.
    /// Connects UI buttons with <see cref="AnimoraPlayer"/> play state logic.
    /// </summary>
    public class AnimoraPlayerEditorControlSection
    {
        private readonly AnimoraPlayerEditor AnimoraPlayerEditor;
        private readonly AnimoraPlayer AnimoraPlayer;
        private readonly OM_HeaderButton _playButton, _stopButton, _replayButton;

        /// <summary>
        /// Initializes the control section and binds UI controls to the player's state.
        /// </summary>
        /// <param name="playerEditor">The parent <see cref="AnimoraPlayerEditor"/> instance.</param>
        public AnimoraPlayerEditorControlSection(AnimoraPlayerEditor playerEditor)
        {
            AnimoraPlayerEditor = playerEditor;
            AnimoraPlayer = playerEditor.Player;

            _replayButton = playerEditor.AnimoraTimeline.Header.ReplayButton;
            _playButton = playerEditor.AnimoraTimeline.Header.PlayButton;
            _stopButton = playerEditor.AnimoraTimeline.Header.StopButton;

            playerEditor.AnimoraTimeline.Header.OnReplayButtonClicked += Play;
            playerEditor.AnimoraTimeline.Header.OnPlayButtonClicked += OnPlayButtonClicked;
            playerEditor.AnimoraTimeline.Header.OnStopButtonClicked += Stop;

            if (!Application.isPlaying)
            {
                AnimoraPlayer.SetPlayState(OM_PlayState.Stopped);
            }

            AnimoraPlayer.OnPlayStateChanged += OnPlayStateChanged;
            OnPlayStateChanged(AnimoraPlayer.PlayState);
        }

        /// <summary>
        /// Enables playmode state listeners and sets initial UI state.
        /// </summary>
        public void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnApplicationPlayModeStateChanged;
            OnApplicationPlayModeStateChanged(EditorApplication.isPlayingOrWillChangePlaymode 
                ? PlayModeStateChange.ExitingEditMode 
                : PlayModeStateChange.EnteredEditMode);
        }

        /// <summary>
        /// Cleans up playmode state listeners.
        /// </summary>
        public void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnApplicationPlayModeStateChanged;
        }

        /// <summary>
        /// Pauses the animation if in play mode.
        /// </summary>
        private void Pause()
        {
            if (Application.isPlaying)
                AnimoraPlayer.PauseAnimation();
        }

        /// <summary>
        /// Resumes the animation if in play mode.
        /// </summary>
        private void Resume()
        {
            if (Application.isPlaying)
                AnimoraPlayer.ResumeAnimation();
        }

        /// <summary>
        /// Starts the animation if in play mode.
        /// </summary>
        private void Play()
        {
            if (Application.isPlaying)
                AnimoraPlayer.PlayAnimation();
        }

        /// <summary>
        /// Stops the animation if in play mode.
        /// </summary>
        private void Stop()
        {
            if (Application.isPlaying)
                AnimoraPlayer.StopAnimation();
        }

        /// <summary>
        /// Handles logic when the play button is clicked.
        /// Switches between Play, Pause, and Resume.
        /// </summary>
        private void OnPlayButtonClicked()
        {
            switch (AnimoraPlayer.PlayState)
            {
                case OM_PlayState.Playing:
                    Pause();
                    break;
                case OM_PlayState.Paused:
                    Resume();
                    break;
                case OM_PlayState.Stopped:
                    Play();
                    break;
            }
        }

        /// <summary>
        /// Updates the button states based on Unity's play mode state.
        /// </summary>
        private void OnApplicationPlayModeStateChanged(PlayModeStateChange playModeState)
        {
            bool enable = playModeState is PlayModeStateChange.ExitingEditMode or PlayModeStateChange.EnteredPlayMode;

            _playButton.SetEnabled(enable);
            _stopButton.SetEnabled(enable);
            _replayButton.SetEnabled(enable);
        }

        /// <summary>
        /// Updates button states and icons based on the current <see cref="OM_PlayState"/>.
        /// </summary>
        private void OnPlayStateChanged(OM_PlayState newState)
        {
            switch (newState)
            {
                case OM_PlayState.Playing:
                    _stopButton.SetEnabled(true);
                    _playButton.Icon.SetBackgroundFromIconContent("PauseButton@2x");
                    break;

                case OM_PlayState.Paused:
                    _stopButton.SetEnabled(true);
                    _playButton.Icon.SetBackgroundFromIconContent("PlayButton@2x");
                    break;

                case OM_PlayState.Stopped:
                    _stopButton.SetEnabled(false);
                    _playButton.Icon.SetBackgroundFromIconContent("PlayButton@2x");
                    break;
            }
        }
    }
}
