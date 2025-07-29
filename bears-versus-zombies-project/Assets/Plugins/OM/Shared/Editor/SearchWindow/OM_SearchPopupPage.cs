using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.Editor
{
    /// <summary>
    /// Represents a single page in the search popup system. 
    /// Handles rendering and interaction with grouped or individual search items.
    /// </summary>
    public class OM_SearchPopupPage : VisualElement
    {
        /// <summary>
        /// Reference to the parent search popup window.
        /// </summary>
        public readonly OM_SearchPopup SearchPopup;

        /// <summary>
        /// Path used to determine the hierarchical level of the page.
        /// </summary>
        public readonly string PagePath;

        private readonly ScrollView _scrollView;
        private readonly List<OM_SearchPopupItem> _allItemsForSearch = new();
        private readonly List<OM_SearchPopupItem> _items = new();

        private bool _isSearchActive;
        private OM_SearchPopupItem _selectedItem;

        /// <summary>
        /// The currently selected item on this page.
        /// </summary>
        public OM_SearchPopupItem SelectedItem
        {
            get => _selectedItem;
            private set
            {
                if (_selectedItem == value)
                {
                    Debug.Log("Trying to set same Item");
                    return;
                }

                _selectedItem?.DeselectElement();
                _selectedItem = value;
                _selectedItem?.SelectElement();
            }
        }

        /// <summary>
        /// Initializes a new instance of a search popup page.
        /// </summary>
        /// <param name="searchPopup">Reference to the search popup.</param>
        /// <param name="pagePath">The folder path this page represents.</param>
        /// <param name="isMainPage">True if this is the root page.</param>
        public OM_SearchPopupPage(OM_SearchPopup searchPopup, string pagePath, bool isMainPage = false)
        {
            name = "Page " + pagePath;
            SearchPopup = searchPopup;
            PagePath = pagePath;

            var header = new VisualElement().SetName(isMainPage ? "Header" : "Header-Button");
            Add(header);

            var backIcon = new VisualElement().SetPickingMode(PickingMode.Ignore).AddClassNames("header-back-icon");
            backIcon.SetBackgroundFromIconContent("d_tab_prev@2x");

            if (!isMainPage)
            {
                header.Add(backIcon);
                header.RegisterCallback<ClickEvent>(_ => SearchPopup.CloseCurrentPage());
                header.AddClassNames("back-button");
            }

            var label = new Label().SetName("PageHeader").SetText(PagePath == "" ? "Main Page" : PagePath).SetPickingMode(PickingMode.Ignore);
            header.Add(label);

            _scrollView = new ScrollView { mode = ScrollViewMode.Vertical };
            Add(_scrollView);

            foreach (var itemData in SearchPopup.AllSearchItemsData)
                DrawItem(itemData);

            if (isMainPage)
            {
                foreach (var itemData in SearchPopup.AllSearchItemsData)
                {
                    var item = new OM_SearchPopupItemType(searchPopup, itemData);
                    _allItemsForSearch.Add(item);
                }
            }
        }

        /// <summary>
        /// Creates and adds an item to the page if it belongs to this path.
        /// </summary>
        /// <param name="item">The item data to render.</param>
        private void DrawItem(OM_SearchPopupItemData item)
        {
            if (!item.Path.Contains(PagePath)) return;

            var remainingPath = item.Path.Remove(0, PagePath.Length);
            if (remainingPath[0] == '/') remainingPath = remainingPath.Remove(0, 1);

            var strings = remainingPath.Split('/');
            if (strings.Length > 1 && strings[1] != "")
            {
                var existingGroup = _items.OfType<OM_SearchPopupItemGroup>().FirstOrDefault(x => x.GroupName == strings[0]);
                if (existingGroup != null) return;

                var newPagePath = string.IsNullOrEmpty(PagePath) ? strings[0] : $"{PagePath}/{strings[0]}";
                var group = new OM_SearchPopupItemGroup(SearchPopup, strings[0], newPagePath);
                AddElement(group);
                return;
            }

            var leafItem = new OM_SearchPopupItemType(SearchPopup, item);
            AddElement(leafItem);
        }

        /// <summary>
        /// Adds a UI element to the scroll view and wires up click events.
        /// </summary>
        /// <param name="item">The search popup item to add.</param>
        private void AddElement(OM_SearchPopupItem item)
        {
            _scrollView.Add(item);
            _items.Add(item);
            item.SetClickAction(OnClick);
        }

        /// <summary>
        /// Handles item click logic (group navigation or type selection).
        /// </summary>
        /// <param name="obj">The clicked item.</param>
        private void OnClick(OM_SearchPopupItem obj)
        {
            switch (obj)
            {
                case OM_SearchPopupItemGroup group:
                    SearchPopup.OpenNewPage(group.PagePath);
                    break;
                case OM_SearchPopupItemType type:
                    SearchPopup.OnSearchItemSelected(type.ItemData);
                    break;
            }
        }

        /// <summary>
        /// Selects the next visible item in the scroll view.
        /// </summary>
        public void SelectNextItem()
        {
            var visibleList = _scrollView.Children().Where(x => x.resolvedStyle.display == DisplayStyle.Flex).ToList();
            var indexOf = visibleList.IndexOf(SelectedItem);

            if (indexOf == -1)
            {
                SelectedItem = visibleList.FirstOrDefault() as OM_SearchPopupItem;
            }
            else if (indexOf + 1 < visibleList.Count)
            {
                SelectedItem = visibleList[indexOf + 1] as OM_SearchPopupItem;
            }
        }

        /// <summary>
        /// Selects the previous visible item in the scroll view.
        /// </summary>
        public void SelectPreviousItem()
        {
            var visibleList = _scrollView.Children().Where(x => x.resolvedStyle.display == DisplayStyle.Flex).ToList();
            var indexOf = visibleList.IndexOf(SelectedItem);

            if (indexOf == -1)
            {
                SelectedItem = visibleList.LastOrDefault() as OM_SearchPopupItem;
            }
            else if (indexOf - 1 >= 0)
            {
                SelectedItem = visibleList[indexOf - 1] as OM_SearchPopupItem;
            }
        }

        /// <summary>
        /// Executes the current item's action (e.g. opens or selects).
        /// </summary>
        public void ChooseSelectedItem()
        {
            OnClick(_selectedItem);
        }

        /// <summary>
        /// Ends the search mode and resets to normal page view.
        /// </summary>
        public void EndSearch()
        {
            foreach (var item in _items)
                _scrollView.Add(item);

            foreach (var item in _allItemsForSearch)
                item.RemoveFromHierarchy();

            _isSearchActive = false;
        }

        /// <summary>
        /// Updates the page to only show search-matching items.
        /// </summary>
        /// <param name="search">The search query string.</param>
        public void UpdateSearch(string search)
        {
            search = search.ToLower().Trim();

            if (!_isSearchActive)
            {
                _isSearchActive = true;

                foreach (var item in _items)
                    item.RemoveFromHierarchy();

                foreach (var item in _allItemsForSearch)
                    _scrollView.Add(item);
            }

            foreach (var item in _allItemsForSearch.OfType<OM_SearchPopupItemType>())
            {
                var keywords = item.ItemData.Keywords;
                keywords.Add(item.ItemData.Name); // also allow name to match

                bool found = keywords.Any(k => k.ToLower().Contains(search));
                item.SetDisplay(found);
            }
        }
    }
}
