using UnityEngine;

namespace OM.Editor
{
    /// <summary>
    /// Represents a selectable type/item entry in the search popup.
    /// Displays the item with its name and icon, and stores the associated data.
    /// </summary>
    public class OM_SearchPopupItemType : OM_SearchPopupItem
    {
        /// <summary>
        /// The data object representing this item (name, path, icon, etc.).
        /// </summary>
        public readonly OM_SearchPopupItemData ItemData;

        /// <summary>
        /// Constructs a new popup item from the given data.
        /// </summary>
        /// <param name="searchPopup">The parent search popup.</param>
        /// <param name="itemData">The item data to bind to this UI element.</param>
        public OM_SearchPopupItemType(OM_SearchPopup searchPopup, OM_SearchPopupItemData itemData) : base(searchPopup)
        {
            ItemData = itemData;
            SetText(itemData.Name);

            // Handle icon rendering based on icon type.
            if (itemData.IconType == OM_IconType.ResourcesFolder)
            {
                var texture2D = Resources.Load<Texture2D>(itemData.Icon);

                if (texture2D != null)
                {
                    this.style.backgroundImage = texture2D;
                }
                else
                {
                    SetIcon(itemData.Icon ?? "d_Transform Icon");
                }
            }
            else
            {
                SetIcon(itemData.Icon ?? "d_Transform Icon");
            }
        }
    }
}