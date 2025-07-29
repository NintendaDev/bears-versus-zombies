using System.Collections.Generic;

namespace OM.Editor
{
    /// <summary>
    /// Manages a collection of visual editor elements that implement specific listener interfaces.
    /// Provides add/remove/query capabilities and triggers validation or undo/redo events.
    /// </summary>
    public class OM_VisualElementsManager
    {
        /// <summary>
        /// Internal list of managed elements.
        /// </summary>
        private List<IOM_Element> Elements { get; } = new List<IOM_Element>();

        /// <summary>
        /// Adds an element to the manager if not already present.
        /// </summary>
        /// <param name="element">The element to add.</param>
        public void AddElement(IOM_Element element)
        {
            if (Elements.Contains(element)) return;

            Elements.Add(element);
        }

        /// <summary>
        /// Removes an element from the manager if it exists.
        /// </summary>
        /// <param name="element">The element to remove.</param>
        public void RemoveElement(IOM_Element element)
        {
            if (!Elements.Contains(element)) return;

            Elements.Remove(element);
        }

        /// <summary>
        /// Clears all elements from the manager.
        /// </summary>
        public void ClearElements()
        {
            Elements.Clear();
        }

        /// <summary>
        /// Returns all elements of the specified type, skipping null references.
        /// </summary>
        /// <typeparam name="T">The type of elements to retrieve.</typeparam>
        /// <returns>An enumerable of matching elements.</returns>
        public IEnumerable<T> GetElements<T>() where T : IOM_Element
        {
            for (int i = Elements.Count - 1; i >= 0; i--)
            {
                if (Elements[i] == null)
                {
                    RemoveElement(Elements[i]);
                    continue;
                }

                if (Elements[i] is T t)
                {
                    yield return t;
                }
            }
        }

        /// <summary>
        /// Triggers the <see cref="IOM_ValidateListener.OnPlayerValidate"/> method on all validate listeners.
        /// </summary>
        public void TriggerValidate()
        {
            foreach (var element in GetElements<IOM_ValidateListener>())
            {
                element.OnPlayerValidate();
            }
        }

        /// <summary>
        /// Triggers the <see cref="IOM_UndoRedoListener.OnUndoRedoPerformed"/> method on all undo/redo listeners.
        /// </summary>
        public void TriggerUndoRedo()
        {
            foreach (var element in GetElements<IOM_UndoRedoListener>())
            {
                element.OnUndoRedoPerformed();
            }
        }
    }

    /// <summary>
    /// Base interface for all elements managed by OM_VisualElementsManager.
    /// </summary>
    public interface IOM_Element { }

    /// <summary>
    /// Interface for elements that respond to validation events.
    /// </summary>
    public interface IOM_ValidateListener : IOM_Element
    {
        /// <summary>
        /// Called when the player/editor triggers a validation action.
        /// </summary>
        void OnPlayerValidate();
    }

    /// <summary>
    /// Interface for elements that respond to undo/redo actions.
    /// </summary>
    public interface IOM_UndoRedoListener : IOM_Element
    {
        /// <summary>
        /// Called when an undo or redo action is performed.
        /// </summary>
        void OnUndoRedoPerformed();
    }
}
