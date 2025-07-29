using System;
using UnityEngine.UIElements;

namespace OM.Editor
{
    /// <summary>
    /// A UI element that displays a label which can be clicked to enter an editable text field.
    /// Useful for inline renaming or editable tags.
    /// </summary>
    public class OM_EditableLabel : VisualElement
    {
        /// <summary>
        /// The read-only label shown by default.
        /// </summary>
        public Label Label { get; private set; }

        /// <summary>
        /// The editable text field shown when entering edit mode.
        /// </summary>
        public TextField TextField { get; private set; }

        /// <summary>
        /// Creates a new editable label with the given text and change callback.
        /// </summary>
        /// <param name="text">The initial text to display.</param>
        /// <param name="onTextChanged">Callback invoked when editing is completed and the text is changed.</param>
        public OM_EditableLabel(string text, Action<string> onTextChanged)
        {
            var onTextChanged1 = onTextChanged;

            Label = new Label(text);
            Add(Label);

            TextField = new TextField
            {
                style = { display = DisplayStyle.None }
            };
            TextField.RegisterValueChangedCallback(OnTextFieldValueChanged);
            Add(TextField);

            // Enter edit mode on label click
            Label.RegisterCallback<MouseDownEvent>(_ =>
            {
                StartEditing();
            });

            // Commit edit on focus loss
            TextField.RegisterCallback<FocusOutEvent>(_ =>
            {
                TextField.style.display = DisplayStyle.None;
                Label.style.display = DisplayStyle.Flex;

                Label.text = TextField.value;
                onTextChanged1?.Invoke(TextField.value);
            });
        }

        /// <summary>
        /// Sets the label text programmatically.
        /// </summary>
        /// <param name="text">The new label text.</param>
        public void SetText(string text)
        {
            Label.text = text;
        }

        /// <summary>
        /// Handles live text changes in the text field (currently unused).
        /// </summary>
        /// <param name="evt">The change event data.</param>
        private void OnTextFieldValueChanged(ChangeEvent<string> evt)
        {
            // No-op; could be used for live validation or previewing.
        }

        /// <summary>
        /// Switches the UI to edit mode, showing the text field and hiding the label.
        /// </summary>
        public void StartEditing()
        {
            TextField.style.display = DisplayStyle.Flex;
            TextField.value = Label.text;
            TextField.Focus();
            Label.style.display = DisplayStyle.None;
        }
    }
}
