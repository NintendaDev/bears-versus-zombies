using OM.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.Animora.Editor
{
    /// <summary>
    /// A drag handle element used within <see cref="AnimoraActionElement"/>.
    /// Implements <see cref="IOM_DragControlDraggable"/> to delegate drag behavior to the parent component.
    /// </summary>
    public class AnimoraActionElementDragArea : VisualElement, IOM_DragControlDraggable
    {
        /// <summary>
        /// The parent component element that owns this drag area.
        /// </summary>
        public AnimoraActionElement ActionElement { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="AnimoraActionElementDragArea"/>.
        /// </summary>
        /// <param name="actionElement">The parent <see cref="AnimoraActionElement"/> this drag area controls.</param>
        public AnimoraActionElementDragArea(AnimoraActionElement actionElement)
        {
            ActionElement = actionElement;

            style.width = new StyleLength(new Length(30, LengthUnit.Pixel));
            style.height = new StyleLength(new Length(100, LengthUnit.Percent));

            var icon = new VisualElement().SetPickingMode(PickingMode.Ignore);
            icon.style.width = new StyleLength(new Length(60, LengthUnit.Percent));
            icon.style.height = new StyleLength(new Length(60, LengthUnit.Percent));
            icon.style.backgroundImage = EditorGUIUtility.IconContent("align_vertically_center").image as Texture2D;
            icon.AddTo(this);

            style.alignContent = Align.Center;
            style.justifyContent = Justify.Center;
            style.alignItems = Align.Center;
        }

        /// <summary>
        /// Called when dragging starts.
        /// Delegates to the parent component element.
        /// </summary>
        public void StartDrag(Vector2 mousePosition)
        {
            ActionElement.StartDrag(mousePosition);
        }

        /// <summary>
        /// Called during dragging movement.
        /// Delegates to the parent component element.
        /// </summary>
        public void Drag(Vector2 delta, Vector2 mousePosition)
        {
            ActionElement.Drag(delta, mousePosition);
        }

        /// <summary>
        /// Called when dragging ends.
        /// Delegates to the parent component element.
        /// </summary>
        public void EndDrag(Vector2 delta, Vector2 mousePosition)
        {
            ActionElement.EndDrag(delta, mousePosition);
        }
    }
}
