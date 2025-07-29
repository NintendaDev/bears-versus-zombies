using UnityEngine.UIElements;

namespace OM.Editor
{
    /// <summary>
    /// A simple UI separator line used for spacing or dividing sections in the editor UI.
    /// Can be vertical or horizontal based on the constructor parameter.
    /// </summary>
    public class OM_Separator : VisualElement
    {
        /// <summary>
        /// Creates a separator element.
        /// </summary>
        /// <param name="vertical">If true, creates a vertical line; otherwise horizontal.</param>
        /// <param name="height">The thickness/length of the separator.</param>
        public OM_Separator(bool vertical, float height)
        {
            if (vertical)
            {
                style.height = height;
            }
            else
            {
                style.width = height;
            }
        }
    }
}