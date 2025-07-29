using OM.Editor;
using UnityEngine.UIElements;

namespace OM.TimelineCreator.Editor
{
    /// <summary>
    /// A custom Button class specifically designed for use within the timeline's header area.
    /// It automatically applies the "header-button" USS class for consistent styling
    /// and includes an internal `Icon` VisualElement to easily display an image representation
    /// for the button (e.g., Play, Stop icons).
    /// </summary>
    public class OM_HeaderButton : Button // Inherits from the base UIElements Button class
    {
        /// <summary>
        /// Gets the child VisualElement intended to display the button's icon.
        /// Style this element (e.g., with `background-image` in USS) to show the desired icon.
        /// </summary>
        public VisualElement Icon { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OM_HeaderButton"/> class.
        /// </summary>
        /// <param name="onClick">The `System.Action` delegate to execute when the button is clicked.</param>
        public OM_HeaderButton(System.Action onClick) : base(onClick) // Pass the click action to the base Button constructor
        {
            // Apply the specific USS class for styling common to header buttons.
            this.AddToClassList("header-button");

            // Create the child VisualElement that will serve as the icon container.
            Icon = new VisualElement();
            // Make the Icon element ignore mouse events, so clicks pass through to the button itself.
            Icon.SetPickingMode(PickingMode.Ignore);
            // Assign a name to the Icon element for potential identification or specific styling via USS.
            Icon.SetName("Icon");

            // Add the created Icon element as a child of this button.
            // The icon will be rendered within the button's bounds according to layout and styling.
            Add(Icon);
        }
    }
}