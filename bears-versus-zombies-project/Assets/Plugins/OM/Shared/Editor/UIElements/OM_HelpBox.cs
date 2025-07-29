using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.Editor
{
    /// <summary>
    /// A custom visual element that displays an info-style help box with an icon and label.
    /// Useful for showing hints, descriptions, or validation messages in the editor UI.
    /// </summary>
    public class OM_HelpBox : VisualElement
    {
        private readonly Label _label;

        /// <summary>
        /// Constructs a new help box with the given text content.
        /// </summary>
        /// <param name="text">The message to display in the help box.</param>
        public OM_HelpBox(string text)
        {
            AddToClassList("help-box");

            var icon = new VisualElement();
            icon.AddToClassList("help-box-icon");
            icon.style.backgroundImage = EditorGUIUtility.IconContent("console.infoicon").image as Texture2D;
            Add(icon);

            _label = new Label(text);
            Add(_label);
        }

        /// <summary>
        /// Updates the message text displayed in the help box.
        /// </summary>
        /// <param name="text">The new message to display.</param>
        public void UpdateText(string text)
        {
            _label.text = text;
        }
    }
}