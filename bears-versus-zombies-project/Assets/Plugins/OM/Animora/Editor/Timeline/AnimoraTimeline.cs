using OM.Editor;
using OM.Animora.Runtime;
using OM.TimelineCreator.Editor;
using OM.Animora.Editor;

namespace OM.Animora.Editor
{
    /// <summary>
    /// Timeline editor implementation for <see cref="AnimoraPlayer"/>.
    /// Manages tracks for <see cref="AnimoraClip"/>s and controls timeline preview behavior.
    /// </summary>
    public class AnimoraTimeline : OM_Timeline<AnimoraClip, AnimoraTrack>
    {
        /// <summary>
        /// Reference to the parent editor managing this timeline.
        /// </summary>
        public AnimoraPlayerEditor PlayerEditor { get; }

        /// <summary>
        /// Constructs the Animora timeline and links it to its owner and player editor.
        /// </summary>
        /// <param name="timelineEditorOwner">The owning timeline editor context.</param>
        /// <param name="playerEditor">The editor for the associated <see cref="AnimoraPlayer"/>.</param>
        public AnimoraTimeline(IOM_TimelineEditorOwner<AnimoraClip> timelineEditorOwner, AnimoraPlayerEditor playerEditor)
            : base(timelineEditorOwner, playerEditor.Player)
        {
            PlayerEditor = playerEditor;
        }

        /// <summary>
        /// Opens the search popup to add a new <see cref="AnimoraClip"/> as a timeline track.
        /// </summary>
        public override void OnAddTrackClicked()
        {
            OM_SearchPopup.Open(this, PlayerEditor);
        }

        /// <summary>
        /// Instantiates and initializes a new <see cref="AnimoraTrack"/> for a given clip.
        /// </summary>
        /// <param name="clip">The <see cref="AnimoraClip"/> to wrap in a track.</param>
        /// <returns>A new initialized <see cref="AnimoraTrack"/>.</returns>
        public override AnimoraTrack CreateTrack(AnimoraClip clip)
        {
            var track = new AnimoraTrack(this, clip, PlayerEditor);
            track.Init();
            return track;
        }

        /// <summary>
        /// Updates the <see cref="AnimoraPlayer"/> preview state at the specified time.
        /// </summary>
        /// <param name="elapsedTime">The time to evaluate preview state at.</param>
        public override void UpdatePreviewElapsedTime(float elapsedTime)
        {
            PlayerEditor.Player.EvaluatePreview(elapsedTime);
        }

        /// <summary>
        /// Called when entering or exiting preview mode. Resets cursor and notifies all tracks and the player.
        /// </summary>
        /// <param name="isPreviewing">Whether the timeline is currently previewing.</param>
        protected override void OnPreviewStateChanged(bool isPreviewing)
        {
            SetCursorTime(0);

            foreach (var track in TracksList)
            {
                track.OnPreviewStateChanged(isPreviewing);
            }

            PlayerEditor.Player.OnPreviewStateChanged(isPreviewing);
        }
    }
}
