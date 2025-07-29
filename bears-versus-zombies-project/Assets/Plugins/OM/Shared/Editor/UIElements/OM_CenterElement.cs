using UnityEngine.UIElements;

namespace OM.Editor
{
    /// <summary>
    /// A utility VisualElement that centers its child elements both vertically and horizontally.
    /// Useful for popups, messages, or centered UI components.
    /// </summary>
    public class OM_CenterElement : VisualElement
    {
        /// <summary>
        /// Initializes the element with full height and centered alignment.
        /// </summary>
        public OM_CenterElement()
        {
            style.height = new StyleLength(new Length(100, LengthUnit.Percent));
            style.justifyContent = Justify.Center;
            style.alignItems = Align.Center;
        }
    }
}