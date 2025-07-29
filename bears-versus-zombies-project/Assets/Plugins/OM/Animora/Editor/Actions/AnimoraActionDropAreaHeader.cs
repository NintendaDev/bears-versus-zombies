using System.Text.RegularExpressions;
using OM.Animora.Runtime;
using OM.Editor;
using UnityEngine.UIElements;

namespace OM.Animora.Editor
{
    /// <summary>
    /// UI element that renders a header for an <see cref="AnimoraActionDropArea"/>,
    /// displaying the component group's name with decorative lines.
    /// </summary>
    public class AnimoraActionDropAreaHeader : VisualElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimoraActionDropAreaHeader"/> class.
        /// </summary>
        /// <param name="actionGroup">The component group whose name will be displayed in the header.</param>
        public AnimoraActionDropAreaHeader(AnimoraActionGroup actionGroup)
        {
            this.SetPickingMode(PickingMode.Ignore);
            this.AddToClassList("drop-area-header");

            var line = new VisualElement().SetName("line");
            line.AddClassNames("drop-area-header-line");
            Add(line);

            var label = new Label(SplitCamelCase(actionGroup.ToString()));
            label.SetName("label");
            Add(label);

            var line2 = new VisualElement().SetName("line");
            line2.AddClassNames("drop-area-header-line");
            Add(line2);
        }

        /// <summary>
        /// Splits a camelCase or PascalCase string into words separated by spaces.
        /// </summary>
        /// <param name="input">The camelCase or PascalCase string.</param>
        /// <returns>A human-readable string with spaces between words.</returns>
        public static string SplitCamelCase(string input)
        {
            return Regex.Replace(input, "(?<!^)([A-Z])", " $1");
        }
    }
}