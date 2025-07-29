using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.Editor
{
    /// <summary>
    /// A customizable header bar used in the OM_DefaultEditor.
    /// Displays the title, optional icon, and a menu button.
    /// </summary>
    public class OM_DefaultEditorHeaderBar : VisualElement
    {
        public readonly OM_DefaultEditor DefaultEditor;
        public readonly Label HeaderLabel;
        public readonly VisualElement RightSection;
        public readonly VisualElement LeftSection;
        public readonly Action OnMenuButtonClicked;

        /// <summary>
        /// Constructs the editor header bar with optional icon and title.
        /// </summary>
        /// <param name="defaultEditor">The editor instance this header belongs to.</param>
        /// <param name="onMenuButtonClicked">Callback when the menu button is clicked.</param>
        public OM_DefaultEditorHeaderBar(OM_DefaultEditor defaultEditor, Action onMenuButtonClicked)
        {
            DefaultEditor = defaultEditor;
            OnMenuButtonClicked = onMenuButtonClicked;

            name = "HeaderBar";
            AddToClassList("default-editor-header-bar");

            // Left section
            LeftSection = new VisualElement { name = "LeftSection" };
            LeftSection.AddToClassList("default-editor-header-left-section");
            LeftSection.AddToClassList("default-editor-header-section");
            Add(LeftSection);

            // Icon from attribute
            var iconAttribute = DefaultEditor.target.GetType().GetCustomAttribute<OM_DefaultEditorIconAttribute>();
            if (iconAttribute != null)
            {
                var icon = new VisualElement();
                icon.style.backgroundImage = iconAttribute.GetIcon();
                icon.AddToClassList("default-editor-header-icon");
                LeftSection.Add(icon);
            }

            // Title from attribute or fallback to class name
            var titleAttribute = DefaultEditor.target.GetType().GetCustomAttribute<OM_DefaultEditorTitleAttribute>();
            var title = titleAttribute?.Title ?? DefaultEditor.target.GetType().Name;
            HeaderLabel = new Label(title) { pickingMode = PickingMode.Ignore };
            HeaderLabel.AddToClassList("default-editor-header-title");
            LeftSection.Add(HeaderLabel);

            // Right section
            RightSection = new VisualElement { name = "RightSection" };
            RightSection.AddToClassList("default-editor-header-right-section");
            RightSection.AddToClassList("default-editor-header-section");
            Add(RightSection);

            // Menu button
            var menuButton = new Button(OnMenuButtonClicked)
            {
                style = { backgroundImage = EditorGUIUtility.IconContent("d__Menu").image as Texture2D }
            };
            menuButton.AddToClassList("om-btn");
            menuButton.AddToClassList("default-editor-header-menu-button");
            RightSection.Add(menuButton);
        }
    }

    /// <summary>
    /// A base class for creating modular and stylish custom Unity editors using UI Toolkit.
    /// </summary>
    public class OM_DefaultEditor : UnityEditor.Editor
    {
        public const string DefaultEditorStyleName = "OM_Default";

        public VisualElement Root { get; private set; }
        public VisualElement Header { get; private set; }
        public VisualElement Body { get; private set; }
        public VisualElement Footer { get; private set; }
        public OM_DefaultEditorHeaderBar HeaderBar { get; private set; }

        /// <summary>
        /// Unity callback that builds the inspector UI using UI Toolkit.
        /// </summary>
        public override VisualElement CreateInspectorGUI()
        {
            Root = new VisualElement { name = "Root" };
            Root.AddToClassList("default-editor-root");
            Root.styleSheets.Add(Resources.Load<StyleSheet>(DefaultEditorStyleName));

            DrawRoot();

            return Root;
        }

        /// <summary>
        /// Builds the root layout: header, body, and footer sections.
        /// </summary>
        private void DrawRoot()
        {
            Header = new VisualElement { name = "Header" };
            Header.AddToClassList("default-editor-header");

            Body = new VisualElement { name = "Body" };
            Body.AddToClassList("default-editor-body");

            Footer = new VisualElement { name = "Footer" };
            Footer.AddToClassList("default-editor-footer");

            DrawHeaderBar();
            Root.Add(Header);
            Root.Add(Body);
            Root.Add(Footer);

            DrawRootHeader(Header);
            DrawRootBody(Body);
            DrawRootFooter(Footer);
        }

        /// <summary>
        /// Adds the default header bar to the header section.
        /// </summary>
        protected virtual void DrawHeaderBar()
        {
            HeaderBar = new OM_DefaultEditorHeaderBar(this, OnMenuButtonClicked);
            Root.Add(HeaderBar);
        }

        /// <summary>
        /// Called when the menu button is clicked. Adds default and custom menu options.
        /// </summary>
        private void OnMenuButtonClicked()
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Edit Script"), false, () =>
            {
                var prop = serializedObject.FindProperty("m_Script");
                if (prop != null)
                    AssetDatabase.OpenAsset(prop.objectReferenceValue);
            });

            OnMenuButtonClicked(ref menu);
            menu.ShowAsContext();
        }

        /// <summary>
        /// Allows subclasses to extend the header menu with additional items.
        /// </summary>
        protected virtual void OnMenuButtonClicked(ref GenericMenu genericMenu) { }

        /// <summary>
        /// Override to draw custom content inside the header section.
        /// </summary>
        protected virtual void DrawRootHeader(VisualElement header) { }

        /// <summary>
        /// Override to draw custom content inside the body section.
        /// </summary>
        protected virtual void DrawRootBody(VisualElement body) { }

        /// <summary>
        /// Override to draw custom content inside the footer section.
        /// </summary>
        protected virtual void DrawRootFooter(VisualElement footer) { }

        /// <summary>
        /// Adds a visual element to the header.
        /// </summary>
        public void AddToHeader(VisualElement element) => Header.Add(element);

        /// <summary>
        /// Adds a visual element to the body.
        /// </summary>
        public void AddToBody(VisualElement element) => Body.Add(element);

        /// <summary>
        /// Adds a visual element to the footer.
        /// </summary>
        public void AddToFooter(VisualElement element) => Footer.Add(element);

        /// <summary>
        /// Reconstructs the full editor UI from scratch.
        /// </summary>
        public void RedrawAll()
        {
            Root.Clear();
            DrawRoot();
        }
    }
}
