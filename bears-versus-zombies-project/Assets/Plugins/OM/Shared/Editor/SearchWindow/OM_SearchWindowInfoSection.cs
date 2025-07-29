using UnityEngine.UIElements;

namespace OM.Editor
{
    /// <summary>
    /// Displays additional information (name, icon, and description) about the currently highlighted search popup item.
    /// </summary>
    public class OM_SearchWindowInfoSection : VisualElement
    {
        private readonly OM_SearchPopup _searchPopup;
        private OM_SearchPopupItem _selectedItem;
        private readonly Label _nameLabel;
        private readonly Label _descriptionLabel;
        private readonly VisualElement _icon;

        /// <summary>
        /// Constructs the info section and initializes UI layout for top (name + icon) and bottom (description).
        /// </summary>
        /// <param name="searchPopup">The parent search popup providing context and current selection.</param>
        public OM_SearchWindowInfoSection(OM_SearchPopup searchPopup)
        {
            _searchPopup = searchPopup;

            var topContainer = new VisualElement().SetPickingMode(PickingMode.Ignore);
            topContainer.AddToClassList("top-container");
            Add(topContainer);

            var bottomContainer = new VisualElement().SetPickingMode(PickingMode.Ignore);
            bottomContainer.AddToClassList("bottom-container");
            Add(bottomContainer);

            _icon = new VisualElement().SetPickingMode(PickingMode.Ignore);
            _icon.AddToClassList("icon");
            topContainer.Add(_icon);

            _icon.style.width = 16;
            _icon.style.height = 16;

            _nameLabel = new Label("Some Text").SetName("InfoText");
            topContainer.Add(_nameLabel);

            _descriptionLabel = new Label("Some Description").SetName("InfoText");
            bottomContainer.Add(_descriptionLabel);

            style.display = DisplayStyle.None;
        }

        /// <summary>
        /// Updates the info section to reflect the currently highlighted or selected item.
        /// </summary>
        /// <param name="item">The item to show info for. If null, uses the currently selected item.</param>
        public void SetItem(OM_SearchPopupItem item)
        {
            if (item == null && _searchPopup.CurrentPage.SelectedItem != null)
            {
                item = _searchPopup.CurrentPage.SelectedItem;
            }
            _selectedItem = item;

            RefreshData();
        }

        /// <summary>
        /// Updates the name, icon, and description UI elements based on the selected item.
        /// </summary>
        private void RefreshData()
        {
            switch (_selectedItem)
            {
                case null:
                    style.display = DisplayStyle.None;
                    return;

                case OM_SearchPopupItemType type:
                    style.display = DisplayStyle.Flex;
                    _nameLabel.text = type.ItemData.Name;
                    if (type.ItemData.Icon != null)
                        _icon.SetBackgroundFromIconContent(type.ItemData.Icon);
                    _descriptionLabel.text = type.ItemData.Description;
                    break;

                default:
                    style.display = DisplayStyle.None;
                    break;
            }
        }
    }
}
