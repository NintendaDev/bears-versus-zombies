using OM.Editor;
using OM.TimelineCreator.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.TimelineCreator.Editor
{
    /// <summary>
    /// UI element that allows resizing a clip by dragging its left edge.
    /// Updates start time and duration accordingly. Supports snapping.
    /// </summary>
    public class OM_ClipHandleLeft<T, TTrack> : OM_ClipHandleBase<T, TTrack>
        where T : OM_ClipBase
        where TTrack : OM_Track<T, TTrack>
    {
        public OM_ClipHandleLeft(OM_Track<T, TTrack> track) : base(track)
        {
            // Visual setup: adds a visual sub-element to represent the handle
            var body = this.CreateVisualElement("clip-handle-left", PickingMode.Ignore, "clip-handle-body",
                parent: this);
            body.style.right = 4;
            style.left = -5f; // offset handle outside the clip slightly
        }

        /// <summary>
        /// Called during dragging to update clip start time and duration.
        /// </summary>
        public override void Drag(Vector2 delta, Vector2 mousePosition)
        {
            // Translate drag distance to time delta based on zoom level
            var newStartTime = StartTime + (delta.x / Track.Timeline.GetPixelPerSecond());
            newStartTime = Mathf.Clamp(newStartTime, 0, StartTime + StartDuration);

            // Recalculate duration to preserve end time
            var newDuration = (StartDuration + StartTime) - newStartTime;

            HandleSnapping(ref newStartTime, ref newDuration);

            // Apply changes
            Track.SetDuration(newDuration);
            Track.SetStartTime(newStartTime);
        }

        /// <summary>
        /// If snapping is enabled, aligns new start time to nearby clips.
        /// </summary>
        private void HandleSnapping(ref float newStartTime, ref float newDuration)
        {
            if (!OM_TimelineSettings.Instance.UseSnapping) return;
            var snapRange = OM_TimelineSettings.Instance.SnappingValue / Track.Timeline.GetPixelPerSecond();

            foreach (var neighbourTrack in Track.GetNeighbourTracks())
            {
                // Snap to other clip's start
                if (OM_Utility.IsWithinRange(newStartTime, neighbourTrack.GetStartTime(), snapRange))
                {
                    newStartTime = neighbourTrack.GetStartTime();
                    newDuration = (StartTime + StartDuration) - newStartTime;
                    Track.GetSnappingLine().SetPosition(true, neighbourTrack.TrackClip.layout.x, Track.layout.center.y);
                    Track.GetSnappingLine().SetFromTo(Track.layout.center, neighbourTrack.layout.center);
                    break;
                }

                // Snap to other clip's end
                if (OM_Utility.IsWithinRange(newStartTime, neighbourTrack.GetStartTime() + neighbourTrack.GetDuration(),
                        snapRange))
                {
                    newStartTime = neighbourTrack.GetStartTime() + neighbourTrack.GetDuration();
                    newDuration = (StartTime + StartDuration) - newStartTime;
                    Track.GetSnappingLine()
                        .SetPosition(true, neighbourTrack.TrackClip.layout.xMax, Track.layout.center.y);
                    Track.GetSnappingLine().SetFromTo(Track.layout.center, neighbourTrack.layout.center);
                    break;
                }

                // Hide snap line if no match found
                Track.GetSnappingLine().SetPosition(false, 0,0);
            }
        }
    }
}