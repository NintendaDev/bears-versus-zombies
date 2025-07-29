using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.Editor
{
    /// <summary>
    /// A custom text field that triggers an event when a key is pressed.
    /// </summary>
    public class OM_TextField : TextField
    {
#if UNITY_6000_0_OR_NEWER
        public event System.Action<KeyDownEvent> OnKeyDownEvent;

        protected override void HandleEventTrickleDown(EventBase evt)
        {
            base.HandleEventTrickleDown(evt);
            
            if (evt is KeyDownEvent keyDownEvent)
            {
                OnKeyDownEvent?.Invoke(keyDownEvent);
            }
        }
#endif
    }

    /// <summary>
    /// A searchable dropdown popup window for selecting items with animated transitions and keyboard navigation.
    /// </summary>
    public class OM_SearchPopup : EditorWindow
    {
        private const int MinWidth = 200;
        private const int MinHeight = 400;
        private const float OpenDuration = 0.6f;

        private static OM_SearchPopup _searchPopup;
        private int _mainWidth;
        private VisualElement _header;
        private OM_SearchWindowInfoSection _infoSection;
        private VisualElement _body;
        private readonly List<OM_SearchPopupPage> _pagesList = new();
        private readonly List<OM_SearchPopupPage> _pagesToClose = new();
        private OM_SearchPopupPage _mainPage;
        private IOM_SearchPopupOwner Owner { get; set; }

        private float _timer;
        private float _lastTime;
        private bool _isAnimating;

        /// <summary>
        /// Returns the currently active page in the search popup.
        /// </summary>
        public OM_SearchPopupPage CurrentPage => (_pagesList == null || _pagesList.Count <= 0) ? _mainPage : _pagesList.Last();

        private static readonly AnimationCurve AnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        /// <summary>
        /// All items currently shown in the search popup.
        /// </summary>
        public List<OM_SearchPopupItemData> AllSearchItemsData { get; private set; } = new();

        /// <summary>
        /// Refreshes the list of search items from the owner.
        /// </summary>
        public void RefreshItems()
        {
            AllSearchItemsData.Clear();
            AllSearchItemsData = Owner.GetSearchItems();
        }

        /// <summary>
        /// Opens the popup as a dropdown below the specified UI element.
        /// </summary>
        /// <param name="visualElement">The element to anchor the popup to.</param>
        /// <param name="owner">The owner that supplies data and handles selection.</param>
        public static void Open(VisualElement visualElement, IOM_SearchPopupOwner owner)
        {
            if (_searchPopup != null)
            {
                _searchPopup.Close();
            }

            _searchPopup = CreateInstance<OM_SearchPopup>();
            _searchPopup.Owner = owner;

            var position = new Rect(
                focusedWindow.position.x + visualElement.worldBound.x,
                focusedWindow.position.y + visualElement.worldBound.y,
                visualElement.worldBound.width,
                visualElement.worldBound.height * 4);

            var size = new Vector2(Mathf.Max(visualElement.resolvedStyle.width, MinWidth), MinHeight);

            _searchPopup._mainWidth = (int)size.x;
            _searchPopup.ShowAsDropDown(position, size);
        }

        /// <summary>
        /// Creates a new instance of the search popup without showing it.
        /// </summary>
        /// <param name="visualElement">UI element to get sizing info from.</param>
        /// <param name="owner">The data owner for the popup.</param>
        /// <returns>A new unshown instance of the search popup.</returns>
        public static OM_SearchPopup GetInstance(VisualElement visualElement, IOM_SearchPopupOwner owner)
        {
            var instance = CreateInstance<OM_SearchPopup>();
            instance.Owner = owner;

            var size = new Vector2(Mathf.Max(visualElement.resolvedStyle.width, MinWidth), MinHeight);
            instance._mainWidth = (int)size.x;

            return instance;
        }

        /// <summary>
        /// Called every frame to update animation states.
        /// </summary>
        private void Update()
        {
            var currentTime = (float)EditorApplication.timeSinceStartup;
            var deltaTime = currentTime - _lastTime;
            _timer += deltaTime;
            _lastTime = currentTime;

            if (_isAnimating == false) return;

            var pagesCount = _pagesList?.Count ?? 0;
            _body.style.left = Mathf.Lerp(_body.style.left.value.value, pagesCount * -_mainWidth, AnimationCurve.Evaluate(_timer / OpenDuration));

            if (_timer >= OpenDuration)
            {
                _isAnimating = false;

                foreach (var page in _pagesToClose)
                {
                    page.RemoveFromHierarchy();
                }

                _pagesToClose.Clear();
            }
        }

        /// <summary>
        /// Resets the static reference when the window is destroyed.
        /// </summary>
        private void OnDestroy()
        {
            _searchPopup = null;
        }

        /// <summary>
        /// Initializes the visual UI layout of the popup.
        /// </summary>
        private void CreateGUI()
        {
            rootVisualElement.AddStyleSheet("OM_SearchPopup");

            _header = new VisualElement().SetName("Main-Header");
            _body = new VisualElement().SetName("Main-Body");

            rootVisualElement.Add(_header);
            rootVisualElement.Add(_body);

            RefreshItems();

            DrawHeader();
            DrawBody();
            DrawFooter();
        }

        /// <summary>
        /// Handles key press input for navigating and interacting with the popup.
        /// </summary>
        private void OnGUI()
        {
            var current = Event.current;
            if (current.type == EventType.KeyDown)
            {
                if (HandleKeyboard(current.keyCode))
                {
                    current.Use();
                }
            }
        }

        /// <summary>
        /// Handles keyboard events like arrow keys and enter/escape.
        /// </summary>
        /// <param name="keyCode">The key pressed.</param>
        /// <returns>True if the key was handled, otherwise false.</returns>
        private bool HandleKeyboard(KeyCode keyCode)
        {
            switch (keyCode)
            {
                case KeyCode.Escape:
                    Close();
                    return true;
                case KeyCode.DownArrow:
                    CurrentPage.SelectNextItem();
                    return true;
                case KeyCode.UpArrow:
                    CurrentPage.SelectPreviousItem();
                    return true;
                case KeyCode.Return:
                    CurrentPage.ChooseSelectedItem();
                    return true;
                case KeyCode.RightArrow:
                    if (CurrentPage.SelectedItem is OM_SearchPopupItemGroup group)
                    {
                        OpenNewPage(group.PagePath);
                    }
                    return true;
                case KeyCode.Backspace:
                case KeyCode.LeftArrow:
                    CloseCurrentPage();
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Draws the search field and handles text change/search filtering.
        /// </summary>
        private void DrawHeader()
        {
            var searchField = new OM_TextField();
            searchField.schedule.Execute(() =>
            {
                var textInputContainer = searchField.Q<VisualElement>("unity-text-input");
                var searchIcon = new VisualElement().SetPickingMode(PickingMode.Ignore);
                searchIcon.style.height = textInputContainer.resolvedStyle.height - 4;
                searchIcon.style.width = textInputContainer.resolvedStyle.height - 4;
                searchIcon.style.position = Position.Absolute;
                searchIcon.style.right = 0;
                searchIcon.style.top = 2;
                searchIcon.SetBackgroundFromIconContent("d_Search Icon");
                textInputContainer.Add(searchIcon);
            });
            searchField.RegisterValueChangedCallback(evt =>
            {
                if (_pagesList.Count > 0)
                {
                    foreach (var page in _pagesList)
                    {
                        page.RemoveFromHierarchy();
                    }
                    _pagesList.Clear();
                    _body.style.left = 0;
                }

                if (evt.newValue.Length > 0)
                {
                    _mainPage.UpdateSearch(evt.newValue);
                }
                else
                {
                    _mainPage.EndSearch();
                }
            });


#if UNITY_6000_0_OR_NEWER
            searchField.OnKeyDownEvent += e =>
            {
                HandleKeyboard(e.keyCode);
            };
#else
            searchField.RegisterCallback<KeyDownEvent>(e =>
            {
                HandleKeyboard(e.keyCode);
                e.StopPropagation();
            });
#endif


            

            searchField.keyboardType = TouchScreenKeyboardType.Search;
            _header.Add(searchField);
            searchField.Focus();
        }

        /// <summary>
        /// Draws the main content area of the popup.
        /// </summary>
        private void DrawBody()
        {
            _mainPage = new OM_SearchPopupPage(this, "", true);
            _body.Add(_mainPage);
        }

        /// <summary>
        /// Draws the footer section with item info.
        /// </summary>
        private void DrawFooter()
        {
            _infoSection = new OM_SearchWindowInfoSection(this);
            rootVisualElement.Add(_infoSection);
        }

        /// <summary>
        /// Opens a new page when navigating into a group.
        /// </summary>
        /// <param name="groupPagePath">Path identifying the group to open.</param>
        public void OpenNewPage(string groupPagePath)
        {
            var page = new OM_SearchPopupPage(this, groupPagePath);
            _pagesList.Add(page);
            _body.Add(page);

            page.style.left = _pagesList.Count * _mainWidth;
            page.style.width = _mainWidth;

            _timer = 0;
            _isAnimating = true;
        }

        /// <summary>
        /// Closes the currently active nested page.
        /// </summary>
        public void CloseCurrentPage()
        {
            if (_pagesList.Count == 0) return;

            var lastPage = _pagesList.Last();
            _pagesList.Remove(lastPage);
            _pagesToClose.Add(lastPage);

            _timer = 0;
            _isAnimating = true;
        }

        /// <summary>
        /// Called when a user selects an item.
        /// </summary>
        /// <param name="typeItemData">The selected item data.</param>
        public void OnSearchItemSelected(OM_SearchPopupItemData typeItemData)
        {
            Owner.OnSearchItemSelected(typeItemData);
            Close();
        }

        /// <summary>
        /// Called when an item is highlighted to show more info.
        /// </summary>
        /// <param name="searchPopupItem">The currently highlighted item.</param>
        public void OnItemHighlighted(OM_SearchPopupItem searchPopupItem)
        {
            _infoSection.SetItem(searchPopupItem);
        }
    }
}
