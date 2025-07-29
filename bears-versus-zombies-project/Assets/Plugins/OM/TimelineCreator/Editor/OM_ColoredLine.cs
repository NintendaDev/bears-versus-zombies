using OM.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.TimelineCreator.Editor
{
    /// <summary>
    /// A visual element used to render a colored line in the timeline UI.
    /// Can be toggled active/inactive to indicate selection or focus state.
    /// Automatically adjusts width, opacity, and color based on state.
    /// </summary>
    public class OM_ColoredLine : VisualElement
    {
        private const float ActiveOffset = 5;
        private const float DisabledOffset = 30;

        /// <summary>
        /// Indicates whether the line is in its active state.
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// The base color of the line (before applying alpha/fade for inactive).
        /// </summary>
        public Color MainColor { get; private set; }

        /// <summary>
        /// Constructor for the colored line. Sets default styles and color.
        /// </summary>
        public OM_ColoredLine(Color mainColor)
        {
            this.AddClassNames("om-colored-line");
            MainColor = mainColor;
            style.position = Position.Absolute;
            style.left = new StyleLength(new Length(ActiveOffset, LengthUnit.Percent));
            style.right = new StyleLength(new Length(ActiveOffset, LengthUnit.Percent));
            style.bottom = 5;
            style.height = 3;
            style.backgroundColor = MainColor;
            this.SetBorderColor(MainColor);
            this.SetBorderSize(1);
            this.SetBorderRadius(2);
            this.SetPickingMode(PickingMode.Ignore);
        }

        /// <summary>
        /// Updates the color of the line.
        /// </summary>
        public OM_ColoredLine SetColor(Color color)
        {
            MainColor = color;
            style.backgroundColor = IsActive ? color : GetDisabledColor();
            this.SetBorderColor(MainColor);
            return this;
        }

        /// <summary>
        /// Sets the active state of the line. Changes styling accordingly.
        /// </summary>
        public void SetActive(bool value)
        {
            IsActive = value;
            if (value)
            {
                style.left = new StyleLength(new Length(ActiveOffset, LengthUnit.Percent));
                style.right = new StyleLength(new Length(ActiveOffset, LengthUnit.Percent));
                style.backgroundColor = MainColor;
            }
            else
            {
                style.left = new StyleLength(new Length(DisabledOffset, LengthUnit.Percent));
                style.right = new StyleLength(new Length(DisabledOffset, LengthUnit.Percent));
                style.backgroundColor = GetDisabledColor();
                this.SetBorderColor(GetDisabledColor());
            }
        }

        /// <summary>
        /// Helper to return a faded version of the main color for the inactive state.
        /// </summary>
        private Color GetDisabledColor() => new Color(MainColor.r, MainColor.g, MainColor.b, 0.2f);
    }
}