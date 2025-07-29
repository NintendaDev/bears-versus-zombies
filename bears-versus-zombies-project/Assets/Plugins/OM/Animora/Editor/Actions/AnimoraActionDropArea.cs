using OM.Animora.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.Animora.Editor
{
    /// <summary>
    /// A custom <see cref="VisualElement"/> representing a drop area for adding actions to an <see cref="AnimoraActionGroup"/>.
    /// Supports context menu, keyboard shortcuts, and a "+" button for interaction.
    /// </summary>
    public class AnimoraActionDropArea : VisualElement
    {
        /// <summary>
        /// The associated <see cref="AnimoraActionGroup"/> this drop area modifies.
        /// </summary>
        public readonly AnimoraActionGroup ActionGroup;

        private readonly AnimoraActionManagerDrawer _drawer;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimoraActionDropArea"/> class.
        /// </summary>
        /// <param name="actionGroup">The component group this drop area belongs to.</param>
        /// <param name="drawer">Reference to the parent drawer for invoking popups.</param>
        public AnimoraActionDropArea(AnimoraActionGroup actionGroup, AnimoraActionManagerDrawer drawer)
        {
            ActionGroup = actionGroup;
            _drawer = drawer;

            this.AddToClassList("drop-area");

            // Right-click context menu
            this.RegisterCallback<ContextClickEvent>(e =>
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Add Action"), false, () => {
                    drawer.ShowSearchPopup(actionGroup, this);
                });
                menu.ShowAsContext();
            });

            // Space key shortcut
            this.RegisterCallback<KeyDownEvent>(e =>
            {
                if (e.keyCode == KeyCode.Space)
                {
                    drawer.ShowSearchPopup(actionGroup, this);
                    e.StopPropagation();
                }
            });

            // "+" button
            var addButton = new Button(() =>
            {
                drawer.ShowSearchPopup(actionGroup, this);
            })
            {
                text = "+"
            };
            addButton.AddToClassList("drop-area-add-button");
            Add(addButton);

            focusable = true;
        }
    }
}
