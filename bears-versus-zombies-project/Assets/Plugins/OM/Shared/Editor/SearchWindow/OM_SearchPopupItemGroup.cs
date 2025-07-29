using UnityEngine;
using UnityEngine.UIElements;

namespace OM.Editor
{
    /// <summary>
    /// Represents a group/folder item in the search popup.
    /// When selected or opened, navigates to a new sub-page.
    /// </summary>
    public class OM_SearchPopupItemGroup : OM_SearchPopupItem
    {
        /// <summary>
        /// The name displayed for the group.
        /// </summary>
        public readonly string GroupName;

        /// <summary>
        /// The path used to identify and navigate to the group's page.
        /// </summary>
        public readonly string PagePath;

        /// <summary>
        /// Constructs a new group item for the search popup.
        /// </summary>
        /// <param name="searchPopup">Reference to the parent popup window.</param>
        /// <param name="groupName">The display name of the group.</param>
        /// <param name="pagePath">The path used to identify the page of this group.</param>
        public OM_SearchPopupItemGroup(OM_SearchPopup searchPopup, string groupName, string pagePath) : base(searchPopup)
        {
            GroupName = groupName;
            PagePath = pagePath;
            SetText(groupName);

            var rightContainer = new VisualElement().SetPickingMode(PickingMode.Ignore);
            rightContainer.AddToClassList("right-container");
            Add(rightContainer);

            var arrow = new VisualElement().SetPickingMode(PickingMode.Ignore);
            arrow.AddToClassList("item-group-arrow");
            rightContainer.Add(arrow);

            SetIcon("d_Folder Icon");
        }
    }
}