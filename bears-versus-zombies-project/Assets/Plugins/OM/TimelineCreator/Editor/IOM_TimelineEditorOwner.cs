using OM.Editor; // Assuming this contains OM_VisualElementsManager
using OM.TimelineCreator.Runtime; // Contains OM_ClipBase, IOM_TimelinePlayer
// using UnityEditor; // Namespace implicitly needed for UnityEditor.Editor type

namespace OM.TimelineCreator.Editor
{
    /// <summary>
    /// Defines the interface for an object that owns and hosts an OM Timeline Editor instance.
    /// The owner provides essential context and references required by the timeline UI components,
    /// such as access to the visual element management system, the associated Unity Editor instance,
    /// and the runtime data player being edited.
    /// </summary>
    /// <typeparam name="T">The type of the data clip managed by the timeline, derived from <see cref="OM_ClipBase"/>.</typeparam>
    public interface IOM_TimelineEditorOwner<T>
        where T : OM_ClipBase
    {
        /// <summary>
        /// Gets the manager responsible for tracking and potentially providing access to
        /// various VisualElements within the owner's UI hierarchy.
        /// Timeline components use this to register themselves or find other elements.
        /// </summary>
        OM_VisualElementsManager VisualElementsManager { get; }

        /// <summary>
        /// Gets the `UnityEditor.Editor` instance associated with this owner (e.g., the CustomEditor or EditorWindow).
        /// Provides access to editor functionalities like `serializedObject`, `Repaint`, etc.
        /// </summary>
        UnityEditor.Editor Editor { get; }

        /// <summary>
        /// Gets the runtime timeline player instance (<see cref="IOM_TimelinePlayer{T}"/>)
        /// that holds the actual clip data being visualized and edited by the timeline UI.
        /// </summary>
        IOM_TimelinePlayer<T> TimelinePlayer { get; }
    }
}