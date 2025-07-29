using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.Editor
{
    /// <summary>
    /// A customizable foldout group UI element for Unity's UI Toolkit.
    /// Supports collapsible content, custom labels, colors, and right-click menu integration.
    /// </summary>
    public class OM_FoldoutGroup : VisualElement
    {
        private const string IconOn = "d_IN_foldout_on@2x";

        /// <summary>
        /// Optional identifier for this foldout group.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// The clickable header container of the foldout.
        /// </summary>
        public VisualElement Header { get; }

        /// <summary>
        /// The left side of the header (label and toggle icon).
        /// </summary>
        public VisualElement HeaderLeftContainer { get; }

        /// <summary>
        /// The right side of the header (e.g. menu buttons).
        /// </summary>
        public VisualElement HeaderRightContainer { get; }

        /// <summary>
        /// The container for child content, shown or hidden based on expansion state.
        /// </summary>
        public VisualElement Content { get; }

        /// <summary>
        /// The label displaying the foldout's title.
        /// </summary>
        public Label Label { get; }

        private Action<bool> _onValueChanged;
        private readonly VisualElement _toggleArrow;

        private bool _value;

        /// <summary>
        /// Gets or sets whether the foldout is expanded.
        /// </summary>
        public bool Value
        {
            get => _value;
            set
            {
                _value = value;

                Content.style.display = _value ? DisplayStyle.Flex : DisplayStyle.None;
                _toggleArrow.style.rotate = new StyleRotate(new Rotate(_value ? 0 : -90));
                Header.style.borderBottomLeftRadius = _value ? 0 : 5;
                Header.style.borderBottomRightRadius = _value ? 0 : 5;

                if (_value)
                    AddToClassList("foldout-group-active");
                else
                    RemoveFromClassList("foldout-group-active");

                _onValueChanged?.Invoke(_value);
                OnValueChanged(_value);
            }
        }

        /// <summary>
        /// Creates a new foldout group with a label and optional callback.
        /// </summary>
        /// <param name="startValue">Whether the group starts expanded.</param>
        /// <param name="label">Label text for the foldout.</param>
        /// <param name="onValueChanged">Callback when the group is expanded or collapsed.</param>
        public OM_FoldoutGroup(bool startValue, string label, Action<bool> onValueChanged = null)
        {
            var styleSheet = Resources.Load<StyleSheet>("OM_FoldoutGroup");
            styleSheets.Add(styleSheet);

            _onValueChanged = onValueChanged;

            AddToClassList("foldout-group");
            Header = new VisualElement().SetName("Header").AddClassNames("foldout-group-header").AddTo(this);
            Header.RegisterCallback<ClickEvent>(OnHeaderClick);

            HeaderLeftContainer = new VisualElement().SetPickingMode(PickingMode.Ignore).SetName("left-container").AddClassNames("foldout-group-header-container").AddTo(Header);
            HeaderLeftContainer.style.flexGrow = 3;

            HeaderRightContainer = new VisualElement().SetPickingMode(PickingMode.Ignore).SetName("right-container").AddClassNames("foldout-group-header-container").AddTo(Header);
            HeaderRightContainer.style.flexGrow = 1;
            HeaderRightContainer.style.justifyContent = Justify.FlexEnd;

            _toggleArrow = new VisualElement().SetPickingMode(PickingMode.Ignore).AddClassNames("foldout-group-arrow").SetBackgroundFromIconContent(IconOn).AddTo(HeaderLeftContainer);
            Label = new Label(label).SetPickingMode(PickingMode.Ignore).AddTo(HeaderLeftContainer);
            Content = new VisualElement().AddClassNames("foldout-group-content").AddTo(this);

            Value = startValue;
        }

        /// <summary>
        /// Adds a callback triggered when the foldout expands or collapses.
        /// </summary>
        public OM_FoldoutGroup OnExpandValueChanged(Action<bool> onValueChanged)
        {
            _onValueChanged += onValueChanged;
            return this;
        }

        /// <summary>
        /// Adds a colored vertical line to the left of the foldout to indicate category/type.
        /// </summary>
        public OM_FoldoutGroup SetColor(Color? color)
        {
            if (color == null) return this;

            var coloredLine = new VisualElement();
            coloredLine.AddToClassList("foldout-group-line");
            coloredLine.style.backgroundColor = color.Value;
            Add(coloredLine);

            return this;
        }

        /// <summary>
        /// Adds a menu button to the header that can invoke a custom callback.
        /// </summary>
        public OM_FoldoutGroup OnMenuClick(Action<Vector2> onMenuClick, bool useBackground = true, string menuBackgroundName = "d__Menu", string menuButtonName = null)
        {
            var menuButton = new Button();
            menuButton.RegisterCallback<ClickEvent>(e =>
            {
                OnMenuClick(e);
                onMenuClick?.Invoke(e.position);
                e.StopPropagation();
            });

            if (useBackground)
            {
                menuButton.style.backgroundImage = EditorGUIUtility.IconContent(menuBackgroundName).image as Texture2D;
            }
            else
            {
                menuButton.text = menuButtonName;
            }

            menuButton.AddToClassList("btn");
            menuButton.AddToClassList("foldout-group-menu");
            HeaderRightContainer.Add(menuButton);

            return this;
        }

        /// <summary>
        /// Adds a custom element to the right side of the foldout header.
        /// </summary>
        public void AddToRightHeaderContainer(VisualElement element)
        {
            Header.Q("right-container").Add(element);
        }

        /// <summary>
        /// Adds a custom element to the left side of the foldout header.
        /// </summary>
        public void AddToLeftHeaderContainer(VisualElement element)
        {
            Header.Q("left-container").Add(element);
        }

        /// <summary>
        /// Adds content to the foldout's expandable body.
        /// </summary>
        public void AddToContent(VisualElement element)
        {
            Content.Add(element);
        }

        /// <summary>
        /// Handles toggling expansion when the header is clicked.
        /// </summary>
        private void OnHeaderClick(ClickEvent evt)
        {
            Value = !Value;
        }

        /// <summary>
        /// Can be overridden for custom response when expanded/collapsed.
        /// </summary>
        protected virtual void OnValueChanged(bool value) { }

        /// <summary>
        /// Sets the label text of the foldout header.
        /// </summary>
        public void SetLabel(string text)
        {
            Label.text = text;
        }

        /// <summary>
        /// Can be overridden for custom menu behavior when using OnMenuClick.
        /// </summary>
        protected virtual void OnMenuClick(ClickEvent evt) { }

        /// <summary>
        /// Creates a standard settings-style foldout group bound to a SerializedProperty's isExpanded field.
        /// </summary>
        public static OM_FoldoutGroup CreateSettingsGroup(SerializedProperty mainProperty, string label, string color, VisualElement parent = null)
        {
            var group = new OM_FoldoutGroup(mainProperty.isExpanded, label, value =>
            {
                mainProperty.isExpanded = value;
                mainProperty.serializedObject.ApplyModifiedProperties();
            }).SetColor(TryGetColorByName(color));
            
            parent?.Add(group);
            return group;
        }

        /// <summary>
        /// A dictionary of predefined color keywords for use in foldout styling.
        /// </summary>
        private static readonly Dictionary<string, Color> ColorsDictionary = new Dictionary<string, Color>()
        {
            { "Red", Color.red },
            { "Green", Color.green },
            { "Blue", Color.blue },
            { "Editor", new Color(1f, 0.15f, 0.69f) },
            { "Sound", new Color(0.77f, 1f, 0.74f) },
            { "Sounds", new Color(0.77f, 1f, 0.74f) },
            { "Events", new Color(0.22f, 1f, 0.85f) },
            { "Settings", new Color(1f, 0.28f, 0.4f) },
            { "Data", new Color(1f, 0.28f, 0.4f) },
            { "Info", new Color(0.3f, 0.84f, 0.12f) },
            { "Clip", new Color(0.58f, 0.3f, 0.8f) },
            { "Target", new Color(0.09f, 0.52f, 0.88f) },
            { "Interpolation", Color.yellow },
            { "Clip_Item", new Color(0.49f, 0.37f, 1f) },
            { "Behaviour", new Color(0.36f, 1f, 0.14f) },
            { "BehaviourGroup", new Color(0.04f, 0.44f, 1f) },
            { "Position", new Color(1f, 0.93f, 0.29f) },
            { "Actions", new Color(1f, 0.93f, 0.29f) },
            { "Action", new Color(0.29f, 0.94f, 1f) },
        };

        /// <summary>
        /// Tries to resolve a color from a keyword string.
        /// </summary>
        public static Color? TryGetColorByName(string colorName)
        {
            if (ColorsDictionary.TryGetValue(colorName, out var color))
            {
                return color;
            }

            return null;
        }
    }
}
