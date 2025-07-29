using UnityEngine;
using UnityEngine.UIElements;

namespace OM.Editor
{
    /// <summary>
    /// A split container with two child containers that can be sized proportionally.
    /// Useful for side-by-side or top-bottom layout arrangements.
    /// </summary>
    public class OM_Split2 : VisualElement
    {
        /// <summary>
        /// The first container, typically aligned to the left or top.
        /// </summary>
        public VisualElement Container1 { get; }

        /// <summary>
        /// The second container, typically aligned to the right or bottom.
        /// </summary>
        public VisualElement Container2 { get; }

        /// <summary>
        /// Initializes the split container with two child containers.
        /// </summary>
        public OM_Split2()
        {
            AddToClassList("om-split2");

            Container1 = new VisualElement { name = "Container1" };
            Container2 = new VisualElement { name = "Container2" };

            Add(Container1);
            Add(Container2);
        }

        /// <summary>
        /// Sets the width and height of the entire split container.
        /// </summary>
        /// <param name="size">The desired size of the split element.</param>
        /// <returns>The same instance for chaining.</returns>
        public OM_Split2 SetSize(Vector2 size)
        {
            style.width = size.x;
            style.height = size.y;
            return this;
        }

        /// <summary>
        /// Adds a visual element to the first container.
        /// </summary>
        /// <param name="element">The element to add.</param>
        public void AddToContainer1(VisualElement element)
        {
            Container1.Add(element);
        }

        /// <summary>
        /// Adds a visual element to the second container.
        /// </summary>
        /// <param name="element">The element to add.</param>
        public void AddToContainer2(VisualElement element)
        {
            Container2.Add(element);
        }

        /// <summary>
        /// Sets the size of the first container as a percentage of the total.
        /// Automatically adjusts the second container to fill the remaining space.
        /// </summary>
        /// <param name="percent">Percentage (0–100) of the total size.</param>
        public void SetContainer1SizePercent(float percent)
        {
            percent = Mathf.Clamp(percent, 0, 100);
            float remaining = 100 - percent;

            switch (resolvedStyle.flexDirection)
            {
                case FlexDirection.Row:
                case FlexDirection.RowReverse:
                    Container1.style.width = new StyleLength(new Length(percent, LengthUnit.Percent));
                    Container2.style.width = new StyleLength(new Length(remaining, LengthUnit.Percent));
                    break;
                case FlexDirection.Column:
                case FlexDirection.ColumnReverse:
                    Container1.style.height = new StyleLength(new Length(percent, LengthUnit.Percent));
                    Container2.style.height = new StyleLength(new Length(remaining, LengthUnit.Percent));
                    break;
            }
        }

        /// <summary>
        /// Sets the size of the second container as a percentage of the total.
        /// Automatically adjusts the first container to fill the remaining space.
        /// </summary>
        /// <param name="percent">Percentage (0–100) of the total size.</param>
        public void SetContainer2SizePercent(float percent)
        {
            percent = Mathf.Clamp(percent, 0, 100);
            float remaining = 100 - percent;

            switch (resolvedStyle.flexDirection)
            {
                case FlexDirection.Row:
                case FlexDirection.RowReverse:
                    Container2.style.width = new StyleLength(new Length(percent, LengthUnit.Percent));
                    Container1.style.width = new StyleLength(new Length(remaining, LengthUnit.Percent));
                    break;
                case FlexDirection.Column:
                case FlexDirection.ColumnReverse:
                    Container2.style.height = new StyleLength(new Length(percent, LengthUnit.Percent));
                    Container1.style.height = new StyleLength(new Length(remaining, LengthUnit.Percent));
                    break;
            }
        }
    }
}
