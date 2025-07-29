using UnityEngine;
using UnityEngine.UIElements;

namespace OM.TimelineCreator.Editor
{
    /// <summary>
    /// A custom VisualElement responsible for drawing the background grid lines
    /// within the timeline body area. It uses the UIElements visual content generation
    /// system (`generateVisualContent`) to draw lines directly onto the element's mesh.
    /// </summary>
    public class OM_TimelineBackground : VisualElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OM_TimelineBackground"/> class.
        /// Sets up basic properties and registers the drawing callback.
        /// </summary>
        public OM_TimelineBackground()
        {
            // Make the background element automatically stretch to fill its parent container.
            this.StretchToParentSize();
            // Ignore mouse events; the background is purely visual.
            pickingMode = PickingMode.Ignore;
            // Register the method responsible for drawing the grid lines.
            // This method will be called by the UIElements system when the element needs repainting.
            generateVisualContent += GenerateVisualContent;
            // Hide any content (like child elements, though unlikely here) that might overflow the bounds.
            style.overflow = Overflow.Hidden;
        }

        /// <summary>
        /// Callback method invoked by the UIElements system to generate the visual content (grid lines).
        /// Draws horizontal and vertical lines based on the element's size and predefined utility constants.
        /// </summary>
        /// <param name="context">The context object providing drawing utilities and element information.</param>
        private static void GenerateVisualContent(MeshGenerationContext context)
        {
            // Get the current dimensions (width and height) of this VisualElement from the layout system.
            var height = context.visualElement.layout.height;
            var width = context.visualElement.layout.width;

            // Get the 2D drawing interface from the context.
            var painter = context.painter2D;

            // Set the color and width for the grid lines using constants from OM_TimelineUtil.
            painter.strokeColor = OM_TimelineUtil.BackgroundLineColor;
            painter.lineWidth = OM_TimelineUtil.BackgroundLineSize;

            // Calculate the number of horizontal grid lines needed based on the standard clip height and spacing.
            var verticalCount = Mathf.RoundToInt(height / (OM_TimelineUtil.ClipHeight + OM_TimelineUtil.ClipSpaceBetween));

            // Begin defining the path for the lines.
            painter.BeginPath();

            // Draw the horizontal lines (one line per track slot boundary).
            for (var i = 0; i < verticalCount - 1; i++) // Loop up to count-1 because we draw boundaries between slots
            {
                // Calculate the Y offset for the current horizontal line.
                var offsetY = (i + 1) * (OM_TimelineUtil.ClipHeight + OM_TimelineUtil.ClipSpaceBetween);
                // Move the drawing cursor to the start of the line (left edge).
                painter.MoveTo(new Vector2(0, offsetY));
                // Draw the line to the end (right edge).
                painter.LineTo(new Vector2(width, offsetY));
            }

            // --- Draw Vertical Lines ---
            // Note: The original code divides width by 10 arbitrarily. This might be intended
            // for simple time markers or might need adjustment based on actual time units/zoom.
            // Let's assume it's for rough visual guides every 10% of the width.
            const int numberOfVerticalDivisions = 10; // Define how many vertical sections
            var horizontalCount = numberOfVerticalDivisions; // Number of lines to draw is related
            var widthOffset = width / numberOfVerticalDivisions; // Calculate spacing between vertical lines

            // Draw the vertical lines.
            for (var i = 0; i < horizontalCount -1; i++) // Loop to draw lines dividing the sections
            {
                // Calculate the X offset for the current vertical line.
                var offsetX = (i + 1) * widthOffset;
                // Move the drawing cursor to the start of the line (top edge).
                painter.MoveTo(new Vector2(offsetX, 0));
                // Draw the line to the end (bottom edge).
                painter.LineTo(new Vector2(offsetX, height));
            }

            // Finalize the drawing by rendering the defined path with the set stroke color/width.
            painter.Stroke();
        }
    }
}