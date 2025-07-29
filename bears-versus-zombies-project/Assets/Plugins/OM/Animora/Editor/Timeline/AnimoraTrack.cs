using System.Linq;
using System.Reflection;
using OM.Animora.Runtime;
using OM.TimelineCreator.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.Animora.Editor
{
    /// <summary>
    /// Represents a timeline track for an <see cref="AnimoraClip"/> in the Animora timeline editor.
    /// Handles icon display, drag-and-drop behavior, and target highlighting.
    /// </summary>
    public class AnimoraTrack : OM_Track<AnimoraClip, AnimoraTrack>
    {
        /// <summary>
        /// The underlying clip represented by this track.
        /// </summary>
        public AnimoraClip AnimoraClip { get; private set; }

        /// <summary>
        /// Reference to the parent editor that owns the timeline.
        /// </summary>
        public AnimoraPlayerEditor AnimoraPlayerEditor { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimoraTrack"/> class.
        /// </summary>
        /// <param name="timeline">The timeline instance this track belongs to.</param>
        /// <param name="clip">The clip associated with this track.</param>
        /// <param name="animoraPlayerEditor">The editor that owns the timeline/player.</param>
        public AnimoraTrack(OM_Timeline<AnimoraClip, AnimoraTrack> timeline, AnimoraClip clip, AnimoraPlayerEditor animoraPlayerEditor)
            : base(timeline, clip)
        {
            AnimoraClip = clip;
            AnimoraPlayerEditor = animoraPlayerEditor;
        }

        /// <summary>
        /// Called when the track icon is clicked.
        /// Pings the first target object of the clip in the editor (if any).
        /// </summary>
        public override void OnIconClicked()
        {
            base.OnIconClicked();

            if (AnimoraClip.GetTargets().Count > 0)
            {
                EditorGUIUtility.PingObject(AnimoraClip.GetTargets().FirstOrDefault());
            }
        }

        /// <summary>
        /// Retrieves the icon to display for this clip's track.
        /// Falls back to the default Unity "AnimationClip" icon if none is specified.
        /// </summary>
        /// <returns>A <see cref="Texture2D"/> representing the icon.</returns>
        public override Texture2D GetClipIcon()
        {
            var iconAttribute = AnimoraClip.GetType().GetCustomAttribute<AnimoraIconAttribute>();
            if (iconAttribute == null)
                return EditorGUIUtility.IconContent("AnimationClip Icon").image as Texture2D;

            var icon = iconAttribute.GetIcon();
            return icon != null ? icon : EditorGUIUtility.IconContent("AnimationClip Icon").image as Texture2D;
        }

        /// <summary>
        /// Handles drag-and-drop logic when objects are dropped on the track.
        /// Passes the dropped objects to the clip's <see cref="AnimoraClip.OnDrop"/> method.
        /// </summary>
        /// <param name="e">The drag perform event.</param>
        protected override void OnDragPerform(DragPerformEvent e)
        {
            Timeline.TimelinePlayer.RecordUndo("On Drop Performed");

            if (AnimoraClip.OnDrop(DragAndDrop.objectReferences, Timeline.TimelinePlayer))
            {
                DragAndDrop.AcceptDrag();
                Timeline.TimelinePlayer.OnValidate();
            }
        }
    }
}
