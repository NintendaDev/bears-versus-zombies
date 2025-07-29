using System.Collections.Generic;

namespace OM.Editor
{
    /// <summary>
    /// Interface for classes that provide and handle items in an OM search popup.
    /// </summary>
    public interface IOM_SearchPopupOwner
    {
        /// <summary>
        /// Returns a list of items to be displayed in the search popup.
        /// </summary>
        /// <returns>List of search popup item data.</returns>
        public List<OM_SearchPopupItemData> GetSearchItems();

        /// <summary>
        /// Called when an item from the search popup is selected.
        /// </summary>
        /// <param name="data">The selected search popup item data.</param>
        public void OnSearchItemSelected(OM_SearchPopupItemData data);
    }
}