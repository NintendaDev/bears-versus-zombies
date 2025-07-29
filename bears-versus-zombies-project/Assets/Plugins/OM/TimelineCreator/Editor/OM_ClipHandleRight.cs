using OM.Editor;
using OM.TimelineCreator.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.TimelineCreator.Editor
{
    /// <summary>
    /// UI element that allows resizing a clip by dragging its right edge.
    /// Updates clip duration accordingly and supports snapping.
    /// </summary>
    public class OM_ClipHandleRight<T, TTrack> : OM_ClipHandleBase<T, TTrack>
        where T : OM_ClipBase
        where TTrack : OM_Track<T, TTrack>
    {
        public OM_ClipHandleRight(OM_Track<T, TTrack> track) : base(track)
        {
            var body = this.CreateVisualElement("clip-handle-right", PickingMode.Ignore, "clip-handle-body",
                parent: this);
            body.style.left = 4;
            style.right = -5f;
        }

        /// <summary>
        /// Called during dragging to extend or shrink the clip's duration from the right.
        /// </summary>
        public override void Drag(Vector2 delta, Vector2 mousePosition)
        {
            var newEndTime = StartTime + StartDuration + (delta.x / Track.Timeline.GetPixelPerSecond());
            newEndTime = Mathf.Max(newEndTime, StartTime); // Clamp to not allow negative duration
            var newDuration = newEndTime - StartTime;

            HandleSnapping(ref newDuration);
            Track.SetDuration(newDuration);
        }

        /// <summary>
        /// If snapping is enabled, aligns new end time to neighbor clips.
        /// </summary>
        private void HandleSnapping(ref float newDuration)
        {
            if (!OM_TimelineSettings.Instance.UseSnapping) return;
            var snapRange = OM_TimelineSettings.Instance.SnappingValue / Track.Timeline.GetPixelPerSecond();
            float newEndTime = StartTime + newDuration;

            foreach (var neighbourTrack in Track.GetNeighbourTracks())
            {
                // Snap to neighbor start
                if (OM_Utility.IsWithinRange(newEndTime, neighbourTrack.GetStartTime(), snapRange))
                {
                    newDuration = neighbourTrack.GetStartTime() - StartTime;
                    Track.GetSnappingLine().SetPosition(true, neighbourTrack.TrackClip.layout.x, Track.layout.center.y);
                    Track.GetSnappingLine().SetFromTo(Track.layout.center, neighbourTrack.layout.center);
                    break;
                }

                // Snap to neighbor end
                if (OM_Utility.IsWithinRange(newEndTime, neighbourTrack.GetStartTime() + neighbourTrack.GetDuration(),
                        snapRange))
                {
                    newDuration = (neighbourTrack.GetStartTime() + neighbourTrack.GetDuration()) - StartTime;
                    Track.GetSnappingLine()
                        .SetPosition(true, neighbourTrack.TrackClip.layout.xMax, Track.layout.center.y);
                    Track.GetSnappingLine().SetFromTo(Track.layout.center, neighbourTrack.layout.center);
                    break;
                }

                // No snap
                Track.GetSnappingLine().SetPosition(false, 0,0);
            }
        }
    }
}