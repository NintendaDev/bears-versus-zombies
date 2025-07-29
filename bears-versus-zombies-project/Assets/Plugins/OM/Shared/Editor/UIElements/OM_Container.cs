using UnityEngine;
using UnityEngine.UIElements;

namespace OM.Editor
{
    /// <summary>
    /// A styled container element with padding, border, and background.
    /// Used to group UI content with consistent visual boundaries.
    /// </summary>
    public class OM_Container : VisualElement
    {
        /// <summary>
        /// Initializes the container with predefined styling: 
        /// padding, rounded border, dark background, and clipped overflow.
        /// </summary>
        public OM_Container()
        {
            this.SetPadding(5)
                .SetBorderRadius(5)
                .SetBorderSize(1)
                .SetBorderColor(new Color(0.19f, 0.19f, 0.19f))
                .SetBackgroundColor(new Color(0.2f, 0.2f, 0.2f))
                .SetOverflow(Overflow.Hidden);
        }
    }
}