using OM.Editor;
using UnityEngine.UIElements;

namespace OM.Animora.Editor
{
    /// <summary>
    /// A styled button with an icon container used in the Animora editor UI.
    /// </summary>
    public class AnimoraIconButton : Button
    {
        /// <summary>
        /// The inner icon container element.
        /// </summary>
        public VisualElement Icon { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimoraIconButton"/> class with a click action.
        /// </summary>
        /// <param name="onClick">The callback invoked when the button is clicked.</param>
        public AnimoraIconButton(System.Action onClick) : base(onClick)
        {
            this.AddToClassList("control-section-btn");

            Icon = new VisualElement();
            Icon.SetName("Icon");
            Add(Icon);
        }
    }
}