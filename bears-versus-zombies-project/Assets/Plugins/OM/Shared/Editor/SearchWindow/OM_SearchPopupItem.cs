using System;
using UnityEngine.UIElements;

namespace OM.Editor
{
    /// <summary>
    /// Represents a selectable item within the OM_SearchPopup UI.
    /// Handles selection, icon display, label, and mouse interactions.
    /// </summary>
    public class OM_SearchPopupItem : VisualElement
    {
        /// <summary>
        /// Reference to the owning search popup.
        /// </summary>
        protected readonly OM_SearchPopup SearchPopup;

        /// <summary>
        /// UI label element for the item text.
        /// </summary>
        protected readonly Label Label;

        /// <summary>
        /// Action to execute when the item is clicked.
        /// </summary>
        protected Action<OM_SearchPopupItem> ClickAction;

        /// <summary>
        /// UI element used to display the item's icon.
        /// </summary>
        protected readonly VisualElement Icon;

        /// <summary>
        /// Indicates whether the item is currently selected.
        /// </summary>
        public bool IsSelected { get; private set; }

        /// <summary>
        /// Marks the item as selected, applies styling, and notifies the popup.
        /// </summary>
        public void SelectElement()
        {
            IsSelected = true;
            AddToClassList("selected");
            SearchPopup.OnItemHighlighted(this);
        }

        /// <summary>
        /// Deselects the item and removes selection styling.
        /// </summary>
        public void DeselectElement()
        {
            IsSelected = false;
            RemoveFromClassList("selected");
        }

        /// <summary>
        /// Sets the click action to be invoked when the item is selected.
        /// </summary>
        /// <param name="action">The callback function to call on click.</param>
        public void SetClickAction(Action<OM_SearchPopupItem> action)
        {
            ClickAction = action;
        }

        /// <summary>
        /// Constructor that sets up the item's visual structure and event handlers.
        /// </summary>
        /// <param name="searchPopup">The parent popup window that owns this item.</param>
        public OM_SearchPopupItem(OM_SearchPopup searchPopup)
        {
            SearchPopup = searchPopup;
            this.AddToClassList("select-item");

            var leftContainer = new VisualElement().SetPickingMode(PickingMode.Ignore);
            leftContainer.AddToClassList("left-container");
            Add(leftContainer);

            Icon = new VisualElement().SetPickingMode(PickingMode.Ignore).AddClassNames("icon");
            leftContainer.Add(Icon);

            Label = new Label().SetPickingMode(PickingMode.Ignore);
            Add(Label);

            RegisterCallback<ClickEvent>(evt =>
            {
                evt.StopPropagation();
                ClickAction?.Invoke(this);
            });

            RegisterCallback<MouseEnterEvent>(e =>
            {
                searchPopup.OnItemHighlighted(this);
            });

            RegisterCallback<MouseLeaveEvent>(e =>
            {
                searchPopup.OnItemHighlighted(null);
            });
        }

        /// <summary>
        /// Sets the display text for the item.
        /// </summary>
        /// <param name="text">The text to show in the label.</param>
        protected void SetText(string text)
        {
            Label.text = text;
        }

        /// <summary>
        /// Sets the item's icon using a Unity icon name.
        /// </summary>
        /// <param name="icon">Name of the icon (e.g., from EditorGUIUtility).</param>
        protected void SetIcon(string icon)
        {
            Icon.SetBackgroundFromIconContent(icon);
        }
    }
}
