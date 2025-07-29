using OM.Editor;
using OM.TimelineCreator.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.TimelineCreator.Editor
{
    /// <summary>
    /// Base class for draggable clip handles (left/right).
    /// Handles drag state management, input tracking, and interaction.
    /// Inherited by OM_ClipHandleLeft and OM_ClipHandleRight.
    /// </summary>
    public abstract class OM_ClipHandleBase<T, TTrack> : VisualElement, IOM_DragControlDraggable,
        IOM_DragControlClickable
        where T : OM_ClipBase
        where TTrack : OM_Track<T, TTrack>
    {
        // Reference to the track this handle controls
        protected readonly OM_Track<T, TTrack> Track;

        // Shortcut to the track's visual clip element
        protected OM_TrackClip<T, TTrack> TrackClip => Track.TrackClip;

        // Drag state variables
        protected float StartXLeft;
        protected float StartXRight;
        protected float StartWidth;
        protected float StartTime;
        protected float StartDuration;

        /// <summary>
        /// Constructor, sets reference and applies styling class.
        /// </summary>
        protected OM_ClipHandleBase(OM_Track<T, TTrack> track)
        {
            Track = track;
            AddToClassList("clip-handle");
        }

        /// <summary>
        /// Called at the beginning of a drag.
        /// Captures initial layout and clip timing state.
        /// </summary>
        public virtual void StartDrag(Vector2 mousePosition)
        {
            Track.SetIsDragging(true);
            StartXLeft = TrackClip.resolvedStyle.left;
            StartXRight = TrackClip.layout.x + TrackClip.layout.width;
            StartWidth = TrackClip.layout.width;
            StartTime = Track.GetStartTime();
            StartDuration = Track.GetDuration();
        }

        /// <summary>
        /// Called continuously while dragging. Implemented by subclasses.
        /// Should update the clip timing based on delta.
        /// </summary>
        public abstract void Drag(Vector2 delta, Vector2 mousePosition);

        /// <summary>
        /// Called at the end of a drag to finalize changes and clear state.
        /// </summary>
        public virtual void EndDrag(Vector2 delta, Vector2 mousePosition)
        {
            Track.SetIsDragging(false);
            Track.GetSnappingLine()?.SetDisplay(false);
        }

        /// <summary>
        /// Called on click. Delegates to the clip visual element for focus or selection.
        /// </summary>
        public virtual void Click(MouseButton mouseButton, MouseUpEvent e, Vector2 mousePosition)
        {
            TrackClip.Click(mouseButton, e, mousePosition);
        }
    }
}