using UnityEngine;
using UnityEngine.UIElements;

namespace OM.Editor
{
    /// <summary>
    /// A split layout container with three resizable content areas.
    /// Useful for complex UI layouts where left-center-right or top-middle-bottom regions are needed.
    /// </summary>
    public class OM_Split3 : VisualElement
    {
        /// <summary>
        /// The first container (e.g., left or top).
        /// </summary>
        public VisualElement Container1 { get; }

        /// <summary>
        /// The second container (e.g., center).
        /// </summary>
        public VisualElement Container2 { get; }

        /// <summary>
        /// The third container (e.g., right or bottom).
        /// </summary>
        public VisualElement Container3 { get; }

        /// <summary>
        /// Initializes the split container with three sections.
        /// </summary>
        public OM_Split3()
        {
            AddToClassList("om-split3");

            Container1 = new VisualElement { name = "Container1" };
            Container2 = new VisualElement { name = "Container2" };
            Container3 = new VisualElement { name = "Container3" };

            Add(Container1);
            Add(Container2);
            Add(Container3);
        }

        /// <summary>
        /// Sets the overall size of the split container.
        /// </summary>
        /// <param name="size">Width and height of the container.</param>
        /// <returns>The same instance for chaining.</returns>
        public OM_Split3 SetSize(Vector2 size)
        {
            style.width = size.x;
            style.height = size.y;
            return this;
        }

        /// <summary>
        /// Adds a visual element to the first container.
        /// </summary>
        public void AddToContainer1(VisualElement element)
        {
            Container1.Add(element);
        }

        /// <summary>
        /// Adds a visual element to the second container.
        /// </summary>
        public void AddToContainer2(VisualElement element)
        {
            Container2.Add(element);
        }

        /// <summary>
        /// Adds a visual element to the third container.
        /// </summary>
        public void AddToContainer3(VisualElement element)
        {
            Container3.Add(element);
        }
    }
}
