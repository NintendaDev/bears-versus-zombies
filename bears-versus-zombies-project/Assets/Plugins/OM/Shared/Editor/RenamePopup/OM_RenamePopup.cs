using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.Editor
{
    /// <summary>
    /// A simple rename popup window that allows users to input a new name.
    /// Supports live preview and confirm actions.
    /// </summary>
    public class OM_RenamePopup : EditorWindow
    {
        private const float MinWidth = 200;
        private const float MinHeight = 50;

        private static OM_RenamePopup _renamePopup;

        private Action<string> _onRename;
        private Action<string> _onPreview;
        private string _startName;

        /// <summary>
        /// Opens the rename popup and positions it relative to the provided visual element.
        /// </summary>
        /// <param name="element">The visual element to anchor the popup to.</param>
        /// <param name="startName">The initial name to populate in the text field.</param>
        /// <param name="onRename">Callback to execute when the name is confirmed.</param>
        /// <param name="onPreview">Optional callback to preview name changes live.</param>
        public static void Open(VisualElement element, string startName, Action<string> onRename, Action<string> onPreview = null)
        {
            if (_renamePopup != null)
            {
                _renamePopup.Close();
            }

            _renamePopup = CreateInstance<OM_RenamePopup>();

            _renamePopup._startName = startName;
            _renamePopup._onRename = onRename;
            _renamePopup._onPreview = onPreview;

            var currentWindow = focusedWindow ?? EditorWindow.mouseOverWindow;

            Rect position;
            if (currentWindow != null && element != null)
            {
                // Position the popup below the element
                position = new Rect(
                    currentWindow.position.x + element.worldBound.x,
                    currentWindow.position.y + element.worldBound.y + element.worldBound.height,
                    element.worldBound.width,
                    element.worldBound.height
                );
            }
            else
            {
                Debug.LogWarning("Could not determine focused window or element for Rename Popup positioning. Using fallback position.");
                position = new Rect(Screen.width / 2f - MinWidth / 2f, Screen.height / 2f - MinHeight / 2f, MinWidth, MinHeight);
            }

            var size = new Vector2(
                Mathf.Max(element?.resolvedStyle.width ?? MinWidth, MinWidth),
                MinHeight
            );

            _renamePopup.titleContent = new GUIContent("Rename");
            _renamePopup.minSize = new Vector2(MinWidth, MinHeight);
            _renamePopup.maxSize = new Vector2(Mathf.Max(size.x, MinWidth), MinHeight);

            _renamePopup.ShowAsDropDown(position, size);
        }

        /// <summary>
        /// Creates and configures the popup UI elements.
        /// </summary>
        private void CreateGUI()
        {
            var renameLabel = new Label("Rename");
            renameLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            rootVisualElement.Add(renameLabel);

            var textField = new TextField
            {
                value = _startName,
                name = "RenameTextField"
            };

            // Live preview callback on text change
            textField.RegisterValueChangedCallback(evt =>
            {
                _onPreview?.Invoke(evt.newValue);
            });

            // Confirm rename on focus out
            textField.RegisterCallback<FocusOutEvent>(_ =>
            {
                _onRename?.Invoke(textField.value);
                Close();
            });

            // Handle Enter/Escape key input
            textField.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
                {
                    _onRename?.Invoke(textField.value);
                    Close();
                    evt.StopPropagation();
                }
                else if (evt.keyCode == KeyCode.Escape)
                {
                    Close();
                    evt.StopPropagation();
                }
            });

            rootVisualElement.Add(textField);

            // Slight delay to ensure the field is properly focused
            textField.schedule.Execute(() => textField.Focus()).StartingIn(10);
        }

        /// <summary>
        /// Cleans up the static instance reference when the popup is destroyed.
        /// </summary>
        private void OnDestroy()
        {
            if (_renamePopup == this)
            {
                _renamePopup = null;
            }
        }
    }
}
