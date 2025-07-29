using System.Collections.Generic;
using OM.TimelineCreator.Runtime;
using UnityEngine;

namespace OM.Animora.Runtime
{
    /// <summary>
    /// Manages a collection of <see cref="AnimoraAction"/> instances associated with an <see cref="AnimoraClip"/>.
    /// Responsible for initializing, validating, adding, removing, and executing actions based on
    /// the Animora timeline lifecycle events.
    /// </summary>
    [System.Serializable]
    public class AnimoraActionsManager
    {
        /// <summary>
        /// Event triggered whenever the list of actions is modified (added or removed).
        /// Useful for editor UI updates.
        /// </summary>
        public System.Action OnActionsChanged;

        /// <summary>
        /// The list containing all AnimoraAction instances managed by this manager.
        /// Marked with [SerializeReference] to support polymorphism serialization in Unity.
        /// </summary>
        [SerializeReference] private List<AnimoraAction> actions;

        /// <summary>
        /// Reference to the AnimoraClip that owns this ActionsManager.
        /// Set during initialization.
        /// </summary>
        private AnimoraClip _clip;
        /// <summary>
        /// Reference to the AnimoraPlayer associated with the owning clip.
        /// Set during initialization.
        /// </summary>
        private AnimoraPlayer _player;

        /// <summary>
        /// Validates all managed actions. Called when the owning clip or player is validated.
        /// </summary>
        /// <param name="player">The associated AnimoraPlayer.</param>
        /// <param name="clip">The owning AnimoraClip.</param>
        public void OnValidate(AnimoraPlayer player, AnimoraClip clip)
        {
            if (actions == null) return; // No actions to validate
            // Iterate through each action and call its OnValidate method
            foreach (var component in actions)
            {
                component?.OnValidate(player, clip); // Null check for safety
            }
        }

        /// <summary>
        /// Checks the list of actions for null entries and removes them.
        /// Ensures the list integrity, especially after potential serialization issues or manual modifications.
        /// </summary>
        /// <param name="player">The associated AnimoraPlayer (potentially for context, though not used in current implementation).</param>
        public void CheckActions(AnimoraPlayer player)
        {
            actions ??= new List<AnimoraAction>(); // Initialize list if null
            // Iterate backwards to safely remove elements
            for (var i = actions.Count - 1; i >= 0; i--)
            {
                if (actions[i] == null)
                {
                    actions.RemoveAt(i); // Remove null entries
                }
            }
        }

        /// <summary>
        /// Initializes the ActionsManager and all its contained actions.
        /// Sets references to the owning clip and player, and calls InitializeComponent on each action.
        /// </summary>
        /// <param name="clip">The owning AnimoraClip.</param>
        /// <param name="player">The associated AnimoraPlayer.</param>
        public void Initialize(AnimoraClip clip, AnimoraPlayer player)
        {
            _clip = clip;
            _player = player;

            actions ??= new List<AnimoraAction>(); // Initialize list if null

            // Initialize each action with references to the manager, player, and clip
            foreach (var component in actions)
            {
                // Null check just in case CheckActions hasn't run or there was an issue
                component?.InitializeComponent(player, this, clip);
            }
        }

        /// <summary>
        /// Gets the complete list of managed AnimoraAction components.
        /// </summary>
        /// <returns>The list of AnimoraActions.</returns>
        public List<AnimoraAction> GetComponents()
        {
            // Initialize list if null, though Initialize should ideally be called first.
            actions ??= new List<AnimoraAction>();
            return actions;
        }

        /// <summary>
        /// Gets an enumerable collection of actions that belong to a specific <see cref="AnimoraActionGroup"/>.
        /// </summary>
        /// <param name="group">The component group to filter actions by.</param>
        /// <returns>An enumerable collection of matching AnimoraActions.</returns>
        public IEnumerable<AnimoraAction> GetComponentsBasedOnType(AnimoraActionGroup group)
        {
            // Initialize list if null for safety.
            actions ??= new List<AnimoraAction>();
            // Use Linq's FindAll to filter actions based on their ComponentGroup property.
            return actions.FindAll(action => action != null && action.ActionGroup == group);
        }

        /// <summary>
        /// Adds a new AnimoraAction to the manager's list.
        /// Records an Undo operation and triggers the OnActionsChanged event.
        /// Typically used by editor tools.
        /// </summary>
        /// <param name="componentToAdd">The AnimoraAction instance to add.</param>
        /// <param name="player">The IOM_TimelinePlayer interface (usually AnimoraPlayer) used for recording Undo.</param>
        public void AddComponent(AnimoraAction componentToAdd, IOM_TimelinePlayer<AnimoraClip> player)
        {
            // Initialize list if null.
            actions ??= new List<AnimoraAction>();
            player.RecordUndo("Add Action"); // Record Undo for editor support
            actions.Add(componentToAdd); // Add the action to the list
            OnActionsChanged?.Invoke(); // Notify listeners that the list has changed
        }

        /// <summary>
        /// Adds a new AnimoraAction to the manager's list directly, without recording Undo or invoking events.
        /// Useful for internal setup or programmatic addition where Undo/events are not needed.
        /// </summary>
        /// <param name="componentToAdd">The AnimoraAction instance to add.</param>
        /// <param name="player">The IOM_TimelinePlayer interface (context, not used for Undo here).</param>
        public void AddActionDirect(AnimoraAction componentToAdd, IOM_TimelinePlayer<AnimoraClip> player)
        {
            if (actions == null) actions = new List<AnimoraAction>(); // Initialize list if null
            actions.Add(componentToAdd); // Add the action directly
        }

        /// <summary>
        /// Removes an AnimoraAction from the manager's list.
        /// Records an Undo operation and triggers the OnActionsChanged event.
        /// Typically used by editor tools.
        /// </summary>
        /// <param name="componentToRemove">The AnimoraAction instance to remove.</param>
        /// <param name="player">The IOM_TimelinePlayer interface (usually AnimoraPlayer) used for recording Undo.</param>
        public void RemoveComponent(AnimoraAction componentToRemove, IOM_TimelinePlayer<AnimoraClip> player)
        {
            // Do nothing if the list is null or the component isn't found.
            if (actions == null || !actions.Contains(componentToRemove)) return;

            player.RecordUndo("Remove Action"); // Record Undo for editor support
            actions.Remove(componentToRemove); // Remove the action from the list
            OnActionsChanged?.Invoke(); // Notify listeners that the list has changed
        }

        #region Runner Methods - These methods iterate and call corresponding methods on relevant actions

        /// <summary>
        /// Calls <see cref="AnimoraAction.ResetBeforePlay"/> on all managed actions.
        /// Used to prepare actions before the timeline starts playing.
        /// </summary>
        /// <param name="isPreviewing">Whether playback is starting in editor preview mode.</param>
        /// <param name="playDirection">The initial playback direction.</param>
        public void ResetBeforePlay(bool isPreviewing, OM_PlayDirection playDirection)
        {
            if (actions == null) return;
            foreach (var action in actions)
            {
                // Pass context (_player, _clip) set during Initialize
                action?.ResetBeforePlay(isPreviewing, playDirection, _player, _clip);
            }
        }

        /// <summary>
        /// Calls <see cref="AnimoraAction.OnPreviewChanged{TTarget}"/> on all managed actions that can be previewed.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target components.</typeparam>
        /// <param name="animoraPlayer">The associated AnimoraPlayer.</param>
        /// <param name="isOn">True if preview is starting, false if stopping.</param>
        /// <param name="targets">The collection of target components.</param>
        /// <param name="clip">The owning AnimoraClip.</param>
        public void OnPreviewChanged<TTarget>(AnimoraPlayer animoraPlayer, bool isOn, IEnumerable<TTarget> targets,
                                              AnimoraClip clip) where TTarget : Component
        {
            if (actions == null) return;
            foreach (var animoraAction in actions)
            {
                // Skip actions that cannot be previewed or are null
                if (animoraAction == null || animoraAction.CanBePreviewed(animoraPlayer) == false) continue;
                animoraAction.OnPreviewChanged(animoraPlayer, isOn, targets, clip);
            }
        }

        /// <summary>
        /// Triggers actions belonging to the <see cref="AnimoraActionGroup.OnStartPlaying"/> group.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target components.</typeparam>
        /// <param name="targets">The collection of target components.</param>
        /// <param name="clip">The owning AnimoraClip.</param>
        /// <param name="player">The associated AnimoraPlayer.</param>
        /// <param name="isPreviewing">Indicates if running during editor preview.</param>
        public void StartPlaying<TTarget>(IEnumerable<TTarget> targets, AnimoraClip clip, AnimoraPlayer player, bool isPreviewing)
            where TTarget : Component
        {
            if (actions == null) return;
            foreach (var action in actions)
            {
                // Check if the action can be played in this group and context
                if (action == null || action.CanBePlayed(AnimoraActionGroup.OnStartPlaying, clip, player) == false) continue;
                action.TriggerAction(targets, player, clip); // Execute the action
            }
        }

        /// <summary>
        /// Triggers actions belonging to the <see cref="AnimoraActionGroup.OnCompletePlaying"/> group.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target components.</typeparam>
        /// <param name="targets">The collection of target components.</param>
        /// <param name="clip">The owning AnimoraClip.</param>
        /// <param name="player">The associated AnimoraPlayer.</param>
        public void CompletePlaying<TTarget>(IEnumerable<TTarget> targets, AnimoraClip clip, AnimoraPlayer player) where TTarget :
            Component
        {
            if (actions == null) return;
            foreach (var action in actions)
            {
                 // Check if the action can be played in this group and context
                if (action == null || action.CanBePlayed(AnimoraActionGroup.OnCompletePlaying, clip, player) == false) continue;
                action.TriggerAction(targets, player, clip); // Execute the action
            }
        }

        /// <summary>
        /// Triggers actions belonging to the <see cref="AnimoraActionGroup.OnStartLoop"/> group.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target components.</typeparam>
        /// <param name="targets">The collection of target components.</param>
        /// <param name="clip">The owning AnimoraClip.</param>
        /// <param name="player">The associated AnimoraPlayer.</param>
        /// <param name="isPreviewing">Indicates if running during editor preview.</param>
        public void StartLoop<TTarget>(IEnumerable<TTarget> targets, AnimoraClip clip, AnimoraPlayer player, bool isPreviewing)
            where TTarget : Component
        {
            if (actions == null) return;
            foreach (var action in actions)
            {
                 // Check if the action can be played in this group and context
                if (action == null || action.CanBePlayed(AnimoraActionGroup.OnStartLoop, clip, player) == false) continue;
                action.TriggerAction(targets, player, clip); // Execute the action
            }
        }

        /// <summary>
        /// Triggers actions belonging to the <see cref="AnimoraActionGroup.OnCompleteLoop"/> group.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target components.</typeparam>
        /// <param name="targets">The collection of target components.</param>
        /// <param name="clip">The owning AnimoraClip.</param>
        /// <param name="player">The associated AnimoraPlayer.</param>
        public void CompleteLoop<TTarget>(IEnumerable<TTarget> targets, AnimoraClip clip, AnimoraPlayer player) where TTarget :
            Component
        {
            if (actions == null) return;
            foreach (var action in actions)
            {
                // Check if the action can be played in this group and context
                if (action == null || action.CanBePlayed(AnimoraActionGroup.OnCompleteLoop, clip, player) == false) continue;
                action.TriggerAction(targets, player, clip); // Execute the action
            }
        }

        /// <summary>
        /// Triggers actions belonging to the <see cref="AnimoraActionGroup.OnExitClip"/> group.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target components.</typeparam>
        /// <param name="targets">The collection of target components.</param>
        /// <param name="clip">The owning AnimoraClip.</param>
        /// <param name="player">The associated AnimoraPlayer.</param>
        public void OnExit<TTarget>(IEnumerable<TTarget> targets, AnimoraClip clip, AnimoraPlayer player) where TTarget : Component
        {
            if (actions == null) return;
            foreach (var action in actions)
            {
                 // Check if the action can be played in this group and context
                if (action == null || action.CanBePlayed(AnimoraActionGroup.OnExitClip, clip, player) == false) continue;
                action.TriggerAction(targets, player, clip); // Execute the action
            }
        }

        /// <summary>
        /// Triggers actions belonging to the <see cref="AnimoraActionGroup.OnEnterClip"/> group.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target components.</typeparam>
        /// <param name="targets">The collection of target components.</param>
        /// <param name="clip">The owning AnimoraClip.</param>
        /// <param name="player">The associated AnimoraPlayer.</param>
        public void OnEnter<TTarget>(IEnumerable<TTarget> targets, AnimoraClip clip, AnimoraPlayer player) where TTarget : Component
        {
            if (actions == null) return;
            foreach (var action in actions)
            {
                // Check if the action can be played in this group and context
                if (action == null || action.CanBePlayed(AnimoraActionGroup.OnEnterClip, clip, player) == false) continue;
                action.TriggerAction(targets, player, clip); // Execute the action
            }
        }

        #endregion
    }
}