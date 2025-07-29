using UnityEngine; 

namespace OM.TimelineCreator.Editor
{
    /// <summary>
    /// Utility class providing constant values for styling and layout within the Timeline Editor.
    /// These constants help maintain consistency across different visual elements of the timeline.
    /// </summary>
    public static class OM_TimelineUtil
    {
        /// <summary>
        /// The standard height (in pixels) for a single track clip visual element.
        /// </summary>
        public const float ClipHeight = 40f;

        /// <summary>
        /// The vertical spacing (in pixels) between adjacent track clip visual elements.
        /// </summary>
        public const float ClipSpaceBetween = 6;

        /// <summary>
        /// The width (in pixels) of the subtle background lines drawn on the timeline body.
        /// </summary>
        public const float BackgroundLineSize = 1f;

        /// <summary>
        /// The color used for the subtle background lines drawn on the timeline body.
        /// </summary>
        public static readonly Color BackgroundLineColor = new Color(0.12f, 0.12f, 0.12f, 0.51f); // A dark, semi-transparent gray
    }
}