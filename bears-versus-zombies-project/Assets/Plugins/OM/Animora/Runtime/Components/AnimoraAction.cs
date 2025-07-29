using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace OM.Animora.Runtime
{
    /// <summary>
    /// Defines specific points within the Animora clip lifecycle where actions can be triggered.
    /// Used to group AnimoraActions based on when they should execute.
    /// </summary>
    public enum AnimoraActionGroup
    {
        /// <summary>
        /// Action triggers when the entire AnimoraPlayer starts playing (once per PlayAnimation call).
        /// </summary>
        OnStartPlaying = 0,
        /// <summary>
        /// Action triggers at the beginning of each loop cycle within the timeline.
        /// </summary>
        OnStartLoop = 1,
        /// <summary>
        /// Action triggers when the timeline evaluation enters the time range of the associated AnimoraClip.
        /// </summary>
        OnEnterClip = 2,
        /// <summary>
        /// Action triggers when the timeline evaluation exits the time range of the associated AnimoraClip.
        /// </summary>
        OnExitClip = 3,
        /// <summary>
        /// Action triggers at the end of each loop cycle within the timeline.
        /// </summary>
        OnCompleteLoop = 4,
        /// <summary>
        /// Action triggers when the entire AnimoraPlayer finishes playing (either completes loops or is stopped).
        /// </summary>
        OnCompletePlaying = 5
    }

    /// <summary>
    /// Interface for classes that own an <see cref="AnimoraActionsManager"/>.
    /// This typically applies to AnimoraClips that manage a collection of actions.
    /// </summary>
    public interface IAnimoraComponentsManagerOwner
    {
        /// <summary>
        /// Gets the manager responsible for handling the collection of AnimoraActions.
        /// </summary>
        public AnimoraActionsManager ActionsManager { get; }
    }

    /// <summary>
    /// Specifies the playback direction(s) in which an <see cref="AnimoraAction"/> should be active.
    /// </summary>
    public enum AnimoraActionPlayDirection
    {
        /// <summary>
        /// Action is active only during forward playback.
        /// </summary>
        Forward = 0,
        /// <summary>
        /// Action is active only during backward playback.
        /// </summary>
        Backward = 1,
        /// <summary>
        /// Action is active during both forward and backward playback.
        /// </summary>
        ForwardAndBackward = 2
    }

    /// <summary>
    /// Abstract base class for all actions that can be executed within an Animora timeline clip.
    /// Actions are grouped by <see cref="AnimoraActionGroup"/> and executed at specific lifecycle points.
    /// </summary>
    [System.Serializable]
    public abstract class AnimoraAction
    {
        /// <summary>
        /// The name of the action, primarily used for identification in the editor.
        /// </summary>
        [SerializeField, OM_HideInInspector] // Hidden in inspector, likely set by attributes or editor logic
        private string actionName;

        /// <summary>
        /// The lifecycle point at which this action should be triggered.
        /// </summary>
        [SerializeField, OM_HideInInspector] // Hidden in inspector, likely set by attributes or editor logic
        private AnimoraActionGroup actionGroup;

        /// <summary>
        /// Determines if this action is currently enabled and should be executed.
        /// </summary>
        [SerializeField, OM_HideInInspector] // Hidden in inspector
        private bool isEnabled = true;

        /// <summary>
        /// The execution order of this action within its <see cref="ActionGroup"/>. Lower numbers execute first.
        /// </summary>
        [SerializeField, OM_HideInInspector] // Hidden in inspector
        private int orderIndex = 0;

        /// <summary>
        /// Specifies the playback direction(s) during which this action can be played.
        /// </summary>
        [SerializeField]
        private AnimoraActionPlayDirection playDirection = AnimoraActionPlayDirection.ForwardAndBackward;

        /// <summary>
        /// Gets or sets the name of the action.
        /// </summary>
        public string ActionName
        {
            get => actionName;
            set => actionName = value;
        }

        /// <summary>
        /// Gets or sets the component group (lifecycle point) for this action.
        /// </summary>
        public AnimoraActionGroup ActionGroup
        {
            get => actionGroup;
            set => actionGroup = value;
        }

        /// <summary>
        /// Gets or sets the execution order index of this action.
        /// </summary>
        public int OrderIndex
        {
            get => orderIndex;
            set => orderIndex = value;
        }

        /// <summary>
        /// Gets or sets whether this action is enabled.
        /// </summary>
        public bool IsEnabled
        {
            get => isEnabled;
            set => isEnabled = value;
        }

        /// <summary>
        /// Gets or sets the allowed playback direction(s) for this action.
        /// </summary>
        public AnimoraActionPlayDirection PlayDirection
        {
            get => playDirection;
            set => playDirection = value;
        }

        /// <summary>
        /// Gets the <see cref="AnimoraPlayer"/> instance currently associated with this action. Set during initialization.
        /// </summary>
        public AnimoraPlayer Player { get; private set; }

        /// <summary>
        /// Gets the <see cref="AnimoraActionsManager"/> that owns this action. Set during initialization.
        /// </summary>
        public AnimoraActionsManager ActionsManager { get; private set; }

        /// <summary>
        /// Gets the <see cref="AnimoraClip"/> that this action belongs to. Set during initialization.
        /// </summary>
        public AnimoraClip Clip { get; protected set; }

        /// <summary>
        /// Called when the owning AnimoraClip or AnimoraPlayer is validated in the editor.
        /// Allows the action to perform validation logic.
        /// </summary>
        /// <param name="player">The associated AnimoraPlayer.</param>
        /// <param name="clip">The owning AnimoraClip.</param>
        public virtual void OnValidate(AnimoraPlayer player, AnimoraClip clip)
        {
            // Base implementation does nothing. Override in subclasses for specific validation.
        }

        /// <summary>
        /// Initializes the action, establishing references to the player, manager, and clip.
        /// Called by the AnimoraActionsManager.
        /// </summary>
        /// <param name="player">The associated AnimoraPlayer.</param>
        /// <param name="actionsManager">The owning AnimoraActionsManager.</param>
        /// <param name="clip">The owning AnimoraClip.</param>
        public void InitializeComponent(AnimoraPlayer player, AnimoraActionsManager actionsManager, AnimoraClip clip)
        {
            Player = player;
            ActionsManager = actionsManager;
            Clip = clip;
        }

        /// <summary>
        /// Determines if this action can be played based on its group, enabled state,
        /// current playback direction, and potentially custom logic (like preview state).
        /// </summary>
        /// <param name="fromGroup">The component group currently being executed.</param>
        /// <param name="clip">The owning AnimoraClip, providing context like current play direction.</param>
        /// <param name="player">The associated AnimoraPlayer.</param>
        /// <returns>True if the action can be played, false otherwise.</returns>
        public virtual bool CanBePlayed(AnimoraActionGroup fromGroup, AnimoraClip clip, AnimoraPlayer player)
        {
            // Check direction constraints based on the clip's current play direction
            if (clip.CurrentPlayDirection == OM_PlayDirection.Forward)
            {
                if (playDirection == AnimoraActionPlayDirection.Backward) return false; // Don't play backward-only actions when moving forward
            }
            else if (clip.CurrentPlayDirection == OM_PlayDirection.Backward) // Clip is playing backward
            {
                if (playDirection == AnimoraActionPlayDirection.Forward) return false; // Don't play forward-only actions when moving backward
            }

            // Check if the action should be excluded during editor preview if it cannot be previewed
            if (Application.isPlaying == false && CanBePreviewed(player) == false) return false;

            // Check if the action is enabled and belongs to the group currently being executed
            return isEnabled && fromGroup == actionGroup;
        }

        /// <summary>
        /// Resets the action to its default state. Called in the editor, often to reset values modified by previewing.
        /// This implementation uses reflection to reset fields to their default values.
        /// </summary>
        /// <param name="player">The associated AnimoraPlayer (used for recording undo).</param>
        /// <param name="clip">The owning AnimoraClip.</param>
        public virtual void Reset(AnimoraPlayer player, AnimoraClip clip)
        {
            player.RecordUndo("Reset Action"); // Record Undo operation for editor
            var stopAtBase = typeof(AnimoraAction); // Stop reflection at the base AnimoraAction class
            var type = GetType(); // Get the concrete type of the current action instance

            // Iterate through the type hierarchy up to AnimoraAction
            while (type != null)
            {
                // Get all instance fields (public and non-public) declared in the current type
                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    // Skip fields inherited from AnimoraAction (like componentGroup, orderIndex, etc.) or actionName
                    if (field.Name is nameof(actionGroup) or
                        nameof(orderIndex) or nameof(isEnabled) or
                        nameof(actionName)) continue;

                    // Set the field back to its default value (default for value types, null for reference types)
                    field.SetValue(this, GetDefaultValue(field.FieldType));
                }

                // Stop if we've reached the base class we defined
                if (type == stopAtBase) break;

                // Move up to the base type for the next iteration
                type = type.BaseType;
            }
        }

        /// <summary>
        /// Helper method to get the default value for a given type.
        /// Uses Activator.CreateInstance for value types and returns null for reference types.
        /// </summary>
        /// <param name="type">The type to get the default value for.</param>
        /// <returns>The default value of the type.</returns>
        private object GetDefaultValue(Type type) => type.IsValueType
            ? Activator.CreateInstance(type) // Create default instance for value types (e.g., 0 for int, false for bool)
            : null; // Default for reference types is null

        /// <summary>
        /// Determines if this action can be previewed in the editor timeline.
        /// By default, actions are previewable. Override to prevent previewing.
        /// </summary>
        /// <param name="animoraPlayer">The associated AnimoraPlayer.</param>
        /// <returns>True if the action supports editor preview, false otherwise.</returns>
        public virtual bool CanBePreviewed(AnimoraPlayer animoraPlayer) => true;

        /// <summary>
        /// Called when the editor preview state changes (starts or stops).
        /// Allows the action to react to entering/exiting preview mode, potentially recording initial state for undo.
        /// Generic version to handle different target types.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target component(s).</typeparam>
        /// <param name="animoraPlayer">The associated AnimoraPlayer.</param>
        /// <param name="isOn">True if preview is starting, false if stopping.</param>
        /// <param name="targets">The collection of target components affected by this action.</param>
        /// <param name="clip">The owning AnimoraClip.</param>
        public virtual void OnPreviewChanged<TTarget>(AnimoraPlayer animoraPlayer, bool isOn,
                                                    IEnumerable<TTarget> targets, AnimoraClip clip) where TTarget : Component
        {
            // Base implementation does nothing. Override in subclasses to handle preview state changes.
        }

        /// <summary>
        /// Helper method to safely cast or get a component of type T from a generic Component.
        /// Logs a warning if the cast/retrieval fails. Used by actions needing specific component types.
        /// Returns null if the target is not of the expected type.
        /// </summary>
        /// <typeparam name="T">The desired component type.</typeparam>
        /// <param name="component">The source component.</param>
        /// <returns>The component cast to type T, or null if incompatible.</returns>
        public T TryGetTarget<T>(Component component) where T : Component
        {
            if (component is T t) return t; // Direct cast if possible

            // Try GetComponent if direct cast fails
            if (component.TryGetComponent(out T result)) return result;

            // Log warning if component is not of the expected type
            Debug.LogWarning($"Target {component} is not a {typeof(T)}. Skipping action logic for this target.");
            return null; // Return null if incompatible
        }

        /*
        /// <summary>
        /// Helper method to safely cast or get components of type T from a collection of generic Components.
        /// Logs a warning for any components that fail the cast/retrieval.
        /// Yields only the successfully converted components.
        /// </summary>
        /// <typeparam name="T">The desired component type.</typeparam>
        /// <param name="targets">The source collection of components.</param>
        /// <returns>An enumerable sequence of components cast to type T.</returns>
        public virtual IEnumerable<T> GetTargets<T>(IEnumerable<Component> targets) where T : Component
        {
            foreach (var target in targets)
            {
                if (target is T t) // Direct cast
                {
                    yield return t;
                    continue;
                }
                if (target.TryGetComponent(out T result)) // Try GetComponent
                {
                    yield return result;
                    continue;
                }
                else // Log warning if incompatible
                {
                    Debug.LogWarning($"Target {target} is not a {typeof(T)}. Skipping action logic for this target.");
                }
            }
        }
        */


        /// <summary>
        /// Triggers the action's main logic on a collection of targets.
        /// This method wraps the type-specific OnTriggerAction implementation.
        /// </summary>
        /// <typeparam name="TTarget">The expected type of the target components.</typeparam>
        /// <param name="targets">The collection of target components.</param>
        /// <param name="animoraPlayer">The associated AnimoraPlayer.</param>
        /// <param name="clip">The owning AnimoraClip.</param>
        public void TriggerAction<TTarget>(IEnumerable<TTarget> targets, AnimoraPlayer animoraPlayer, AnimoraClip clip)
            where TTarget : Component
        {
            // Calls the virtual method to be implemented by subclasses
            OnTriggerAction(targets, animoraPlayer, clip);
        }

        /// <summary>
        /// The core virtual method where the action's primary logic is implemented.
        /// This method is called when the action's <see cref="ActionGroup"/> is reached during playback.
        /// Must be overridden by concrete action subclasses.
        /// </summary>
        /// <typeparam name="TTarget">The expected type of the target components.</typeparam>
        /// <param name="targets">The collection of target components.</param>
        /// <param name="animoraPlayer">The associated AnimoraPlayer.</param>
        /// <param name="clip">The owning AnimoraClip.</param>
        public virtual void OnTriggerAction<TTarget>(IEnumerable<TTarget> targets, AnimoraPlayer animoraPlayer,
                                                   AnimoraClip clip) where TTarget : Component
        {
            // Base implementation does nothing. Must be overridden in subclasses.
        }

        /// <summary>
        /// Called once before the AnimoraPlayer starts playing its very first loop.
        /// Allows the action to perform setup tasks based on the initial playback direction,
        /// especially for interpolation setup.
        /// </summary>
        /// <param name="isPreviewing">Indicates if this is happening during editor preview.</param>
        /// <param name="direction">The initial playback direction.</param>
        /// <param name="player">The associated AnimoraPlayer.</param>
        /// <param name="clip">The owning AnimoraClip.</param>
        public virtual void ResetBeforePlay(bool isPreviewing, OM_PlayDirection direction, AnimoraPlayer player,
                                          AnimoraClip clip)
        {
            // Base implementation does nothing. Override for pre-play setup.
        }

        /// <summary>
        /// Factory method to create an instance of a specific AnimoraAction type.
        /// Automatically sets default values based on <see cref="AnimoraCreateAttribute"/> if present.
        /// </summary>
        /// <typeparam name="T">The concrete AnimoraAction type to create.</typeparam>
        /// <param name="isEnabled">Initial enabled state for the action.</param>
        /// <param name="group">Initial component group for the action.</param>
        /// <returns>A new instance of the specified AnimoraAction type.</returns>
        public static T CreateInstance<T>(bool isEnabled = false, AnimoraActionGroup group = AnimoraActionGroup.OnStartPlaying) where T : AnimoraAction
        {
            var type = typeof(T);
            var instance = (T)Activator.CreateInstance(type); // Create a new instance

            // Get custom attribute for default creation settings
            var animoraCreateAttribute = type.GetCustomAttribute<AnimoraCreateAttribute>();

            // Set default name from attribute or type name
            instance.ActionName = animoraCreateAttribute != null ? animoraCreateAttribute.Name : type.Name;
            // Set initial state from parameters
            instance.IsEnabled = isEnabled;
            instance.ActionGroup = group;
            return instance;
        }
    }
}