using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.Editor
{
    /// <summary>
    /// A utility class that provides fluent extensions and helpers for working with Unity UIElements.
    /// Includes styling, creation, binding, and event handling shortcuts.
    /// </summary>
    public static class OM_UIElementsUtil
    {
        /// <summary>
        /// Draws a serialized property inside a parent element and optionally subscribes to value changes.
        /// </summary>
        public static PropertyField DrawProperty(this SerializedProperty property, VisualElement parent, Action<SerializedProperty, SerializedPropertyChangeEvent> onPropertyChanged = null)
        {
            var propertyCopy = property.Copy();
            var propertyField = new PropertyField(propertyCopy);
            propertyField.Bind(property.serializedObject);
            parent.Add(propertyField);

            propertyField.RegisterValueChangeCallback(e => onPropertyChanged?.Invoke(propertyCopy, e));
            return propertyField;
        }

        /// <summary>
        /// Adds one or more class names to a VisualElement.
        /// </summary>
        public static T AddClassNames<T>(this T element, params string[] classNames) where T : VisualElement
        {
            foreach (var className in classNames)
            {
                if (!string.IsNullOrEmpty(className))
                    element.AddToClassList(className);
            }
            return element;
        }

        /// <summary>
        /// Adds the current element to a parent container.
        /// </summary>
        public static T AddTo<T>(this T element, VisualElement parent) where T : VisualElement
        {
            parent?.Add(element);
            return element;
        }

        /// <summary>
        /// Sets the background image of an element from a Unity built-in icon.
        /// </summary>
        public static T SetBackgroundFromIconContent<T>(this T element, string iconName) where T : VisualElement
        {
            var iconContent = EditorGUIUtility.IconContent(iconName);
            element.style.backgroundImage = iconContent?.image as Texture2D;
            return element;
        }

        /// <summary>
        /// Adds a single style sheet from Resources.
        /// </summary>
        public static T AddStyleSheet<T>(this T element, string styleSheetPath) where T : VisualElement
        {
            var styleSheet = Resources.Load<StyleSheet>(styleSheetPath);
            if (styleSheet != null) element.styleSheets.Add(styleSheet);
            return element;
        }

        /// <summary>
        /// Adds multiple style sheets from Resources.
        /// </summary>
        public static T AddStyleSheet<T>(this T element, params string[] stylesheets) where T : VisualElement
        {
            foreach (var path in stylesheets)
            {
                var sheet = Resources.Load<StyleSheet>(path);
                if (sheet != null) element.styleSheets.Add(sheet);
            }
            return element;
        }

        /// <summary>
        /// Sets the name of the element.
        /// </summary>
        public static T SetName<T>(this T element, string name) where T : VisualElement
        {
            if (!string.IsNullOrEmpty(name))
                element.name = name;
            return element;
        }

        /// <summary>
        /// Sets the tooltip for the element.
        /// </summary>
        public static T SetTooltip<T>(this T element, string tooltip) where T : VisualElement
        {
            element.tooltip = tooltip;
            return element;
        }

        /// <summary>
        /// Sets the picking mode for the element.
        /// </summary>
        public static T SetPickingMode<T>(this T element, PickingMode pickingMode) where T : VisualElement
        {
            element.pickingMode = pickingMode;
            return element;
        }

        /// <summary>
        /// Assigns the element to an out variable in a fluent chain.
        /// </summary>
        public static T AssignTo<T>(this T element, out T target) where T : VisualElement
        {
            target = element;
            return element;
        }

        /// <summary>
        /// Sets the text for text-based elements.
        /// </summary>
        public static T SetText<T>(this T element, string text) where T : TextElement
        {
            element.text = text;
            return element;
        }

        /// <summary>
        /// Adds a click event callback to a Button.
        /// </summary>
        public static T OnClicked<T>(this T element, Action onClick) where T : Button
        {
            element.clicked += onClick;
            return element;
        }

        /// <summary>
        /// Creates and returns a button added to the current visual element.
        /// </summary>
        public static Button CreateButton(this VisualElement target, string text, Action onClick)
        {
            return new Button().SetText(text).OnClicked(onClick);
        }

        /// <summary>
        /// Creates a generic VisualElement with optional name, class, and parent.
        /// </summary>
        public static VisualElement CreateVisualElement(this VisualElement target, string name = null, PickingMode pickingMode = PickingMode.Position, string className = null, VisualElement parent = null)
        {
            return new VisualElement().SetName(name).SetPickingMode(pickingMode).AddTo(parent).AddClassNames(className);
        }

        /// <summary>
        /// Creates a new instance of a given VisualElement type and optionally configures it.
        /// </summary>
        public static T Create<T>(this VisualElement target, string name = null, PickingMode pickingMode = PickingMode.Position, string className = null, VisualElement parent = null)
            where T : VisualElement, new()
        {
            return new T().SetName(name).SetPickingMode(pickingMode).AddTo(parent).AddClassNames(className);
        }

        // ------------- Styling Extensions -------------

        public static T SetBorderRadius<T>(this T target, float radius) where T : VisualElement
        {
            target.style.borderTopLeftRadius = radius;
            target.style.borderTopRightRadius = radius;
            target.style.borderBottomRightRadius = radius;
            target.style.borderBottomLeftRadius = radius;
            return target;
        }

        public static T SetBorderRadius<T>(this T target, float topRight, float bottomRight, float bottomLeft, float topLeft) where T : VisualElement
        {
            target.style.borderTopRightRadius = topRight;
            target.style.borderBottomRightRadius = bottomRight;
            target.style.borderBottomLeftRadius = bottomLeft;
            target.style.borderTopLeftRadius = topLeft;
            return target;
        }

        public static T SetFlexDirection<T>(this T target, FlexDirection direction) where T : VisualElement
        {
            target.style.flexDirection = direction;
            return target;
        }

        public static T SetJustifyContent<T>(this T target, Justify justify) where T : VisualElement
        {
            target.style.justifyContent = justify;
            return target;
        }

        public static T SetPadding<T>(this T target, float padding) where T : VisualElement
        {
            return target.SetPadding(padding, padding, padding, padding);
        }

        public static T SetPadding<T>(this T target, float top, float right, float bottom, float left) where T : VisualElement
        {
            target.style.paddingTop = top;
            target.style.paddingRight = right;
            target.style.paddingBottom = bottom;
            target.style.paddingLeft = left;
            return target;
        }

        public static T SetMargin<T>(this T target, float margin) where T : VisualElement
        {
            return target.SetMargin(margin, margin, margin, margin);
        }

        public static T SetMargin<T>(this T target, float top, float right, float bottom, float left) where T : VisualElement
        {
            target.style.marginTop = top;
            target.style.marginRight = right;
            target.style.marginBottom = bottom;
            target.style.marginLeft = left;
            return target;
        }

        public static T SetBackgroundColor<T>(this T target, Color color) where T : VisualElement
        {
            target.style.backgroundColor = color;
            return target;
        }

        public static T SetOverflow<T>(this T target, Overflow overflow) where T : VisualElement
        {
            target.style.overflow = overflow;
            return target;
        }

        public static T SetSize<T>(this T target, float width, float height) where T : VisualElement
        {
            target.style.width = width;
            target.style.height = height;
            return target;
        }

        public static T SetBorderColor<T>(this T target, Color topColor, Color rightColor, Color bottomColor, Color leftColor) where T : VisualElement
        {
            target.style.borderTopColor = topColor;
            target.style.borderRightColor = rightColor;
            target.style.borderBottomColor = bottomColor;
            target.style.borderLeftColor = leftColor;
            return target;
        }

        public static T SetBorderColor<T>(this T target, Color color) where T : VisualElement
        {
            return target.SetBorderColor(color, color, color, color);
        }

        /// <summary>
        /// Sets top, right, bottom, and left offsets.
        /// </summary>
        public static T SetTrbl<T>(this T target, float top, float right, float bottom, float left) where T : VisualElement
        {
            target.style.top = top;
            target.style.right = right;
            target.style.bottom = bottom;
            target.style.left = left;
            return target;
        }

        /// <summary>
        /// Sets all four offsets to the same value.
        /// </summary>
        public static T SetTrbl<T>(this T target, float value) where T : VisualElement
        {
            return target.SetTrbl(value, value, value, value);
        }

        /// <summary>
        /// Sets border width on all sides.
        /// </summary>
        public static T SetBorderSize<T>(this T target, float size) where T : VisualElement
        {
            target.style.borderTopWidth = size;
            target.style.borderRightWidth = size;
            target.style.borderBottomWidth = size;
            target.style.borderLeftWidth = size;
            return target;
        }

        /// <summary>
        /// Sets the display style (Flex or None).
        /// </summary>
        public static T SetDisplay<T>(this T target, DisplayStyle display) where T : VisualElement
        {
            target.style.display = display;
            return target;
        }

        /// <summary>
        /// Sets visibility using a boolean flag (true = Flex, false = None).
        /// </summary>
        public static T SetDisplay<T>(this T target, bool value) where T : VisualElement
        {
            return target.SetDisplay(value ? DisplayStyle.Flex : DisplayStyle.None);
        }
    }
}
