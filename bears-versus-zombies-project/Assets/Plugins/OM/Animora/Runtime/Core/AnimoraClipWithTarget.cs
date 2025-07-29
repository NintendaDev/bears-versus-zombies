using System;
using System.Collections.Generic;
using System.Linq;
using OM.TimelineCreator.Runtime;
using UnityEngine;
using Object = UnityEngine.Object;

namespace OM.Animora.Runtime
{
    /// <summary>
    /// An abstract base class for Animora clips that operate on specific target Components.
    /// It extends <see cref="AnimoraClip"/> by adding management for target components (<see cref="AnimoraTargets{T}"/>)
    /// and a collection of actions (<see cref="AnimoraActionsManager"/>) that are executed during the clip's lifecycle events.
    /// This class simplifies creating clips that need to interact with GameObjects/Components in the scene.
    /// </summary>
    /// <typeparam name="T">The specific type of <see cref="System.ComponentModel.Component"/> that this clip targets (e.g., Transform, Light, Renderer).</typeparam>
    [System.Serializable] // Ensures this class and its derived classes can be serialized by Unity.
    public abstract class AnimoraClipWithTarget<T> : AnimoraClip, IAnimoraComponentsManagerOwner // Inherits base clip functionality and owns an ActionsManager
        where T : Component // Constraint: T must be a Unity Component or derived from it.
    {
        /// <summary>
        /// Marks the end of a property group in the custom inspector drawer.
        /// Likely used by <see cref="OM.TimelineCreator.Editor.Drawers.OM_ClipBaseDrawer"/>.
        /// </summary>
        [OM_EndGroup] // Attribute possibly used by a custom editor/drawer to structure the inspector.

        [SerializeField]
        [Tooltip("The target components this clip will affect.")] // Add tooltip for clarity in Inspector
        protected AnimoraTargets<T> targets;

        /// <summary>
        /// Manages the collection of <see cref="AnimoraAction"/> instances associated with this clip.
        /// These actions are executed at different points in the clip's lifecycle.
        /// This field is serialized by Unity.
        /// </summary>
        [SerializeField]
        [Tooltip("Actions to be executed by this clip during its lifecycle.")] // Add tooltip
        protected AnimoraActionsManager actionsManager;

        /// <summary>
        /// Public property providing access to the target component manager (<see cref="AnimoraTargets{T}"/>).
        /// </summary>
        public AnimoraTargets<T> Targets => targets;

        /// <summary>
        /// Public property providing access to the action manager (<see cref="AnimoraActionsManager"/>).
        /// Implements the <see cref="IAnimoraComponentsManagerOwner"/> interface.
        /// Lazily initializes the actionsManager if it's null upon first access.
        /// </summary>
        public AnimoraActionsManager ActionsManager => actionsManager ??= new AnimoraActionsManager();

        /// <summary>
        /// Overrides the base <see cref="AnimoraClip.OnValidate"/> method.
        /// Calls the base validation and then validates the associated <see cref="AnimoraActionsManager"/>.
        /// Typically called when inspector values change in the Unity Editor.
        /// </summary>
        /// <param name="player">The <see cref="AnimoraPlayer"/> context.</param>
        public override void OnValidate(AnimoraPlayer player)
        {
            // Call the validation logic from the base AnimoraClip class.
            base.OnValidate(player);
            // Validate the actions managed by the ActionsManager, passing player and this clip for context.
            // The null-conditional operator (?.) prevents errors if actionsManager hasn't been initialized yet.
            ActionsManager?.OnValidate(player, this);
        }

        /// <summary>
        /// Overrides the base <see cref="AnimoraClip.ResetBeforePlay"/> method.
        /// Calls the base reset logic and then resets the associated <see cref="AnimoraActionsManager"/>.
        /// </summary>
        /// <param name="isPreviewing">Indicates if the reset is for preview mode.</param>
        /// <param name="playDirection">The intended playback direction.</param>
        public override void ResetBeforePlay(bool isPreviewing, OM_PlayDirection playDirection)
        {
            // Call the reset logic from the base AnimoraClip class.
            base.ResetBeforePlay(isPreviewing, playDirection);
            // Reset the state of all actions managed by the ActionsManager.
            ActionsManager?.ResetBeforePlay(isPreviewing, playDirection);
        }

        /// <summary>
        /// Overrides the base <see cref="AnimoraClip.OnStartPlaying"/> method.
        /// Calls the base start logic, initializes the <see cref="AnimoraActionsManager"/>,
        /// and triggers the <see cref="AnimoraActionGroup.OnStartPlaying"/> actions.
        /// </summary>
        /// <param name="player">The <see cref="AnimoraPlayer"/> starting playback.</param>
        /// <param name="isPreviewing">Indicates if playback is starting in preview mode.</param>
        /// <param name="playDirection">The initial playback direction.</param>
        public override void OnStartPlaying(AnimoraPlayer player, bool isPreviewing, OM_PlayDirection playDirection)
        {
            // Call the start playing logic from the base AnimoraClip class.
            base.OnStartPlaying(player, isPreviewing, playDirection);
            // Initialize the ActionsManager with the current player and this clip instance.
            // This ensures actions have the necessary context.
            ActionsManager.Initialize(this, player);
            // Trigger the 'OnStartPlaying' actions within the ActionsManager, targeting the components retrieved from 'targets'.
            ActionsManager.StartPlaying(Targets.GetTargets(), this, player, isPreviewing);
        }

        /// <summary>
        /// Overrides the base <see cref="AnimoraClip.OnCompletePlaying"/> method.
        /// Calls the base completion logic and triggers the <see cref="AnimoraActionGroup.OnCompletePlaying"/> actions.
        /// </summary>
        public override void OnCompletePlaying()
        {
            // Call the completion logic from the base AnimoraClip class.
            base.OnCompletePlaying();
            // Trigger the 'OnCompletePlaying' actions, targeting the associated components.
            // Pass the current player reference (obtained from the base class).
            ActionsManager.CompletePlaying(Targets.GetTargets(), this, Player);
        }

        /// <summary>
        /// Overrides the base <see cref="AnimoraClip.OnStartLoop"/> method.
        /// Calls the base loop start logic and triggers the <see cref="AnimoraActionGroup.OnStartLoop"/> actions.
        /// </summary>
        /// <param name="isPreviewing">Indicates if the loop is starting in preview mode.</param>
        /// <param name="playDirection">The playback direction for this loop.</param>
        public override void OnStartLoop(bool isPreviewing, OM_PlayDirection playDirection)
        {
            // Call the loop start logic from the base AnimoraClip class.
            base.OnStartLoop(isPreviewing, playDirection);
            // Trigger the 'OnStartLoop' actions, targeting the associated components.
            ActionsManager.StartLoop(Targets.GetTargets(), this, Player, isPreviewing);
        }

        /// <summary>
        /// Overrides the base <see cref="AnimoraClip.OnCompleteLoop"/> method.
        /// Calls the base loop completion logic and triggers the <see cref="AnimoraActionGroup.OnCompleteLoop"/> actions.
        /// </summary>
        public override void OnCompleteLoop()
        {
            // Call the loop completion logic from the base AnimoraClip class.
            base.OnCompleteLoop();
            // Trigger the 'OnCompleteLoop' actions, targeting the associated components.
            ActionsManager.CompleteLoop(Targets.GetTargets(), this, Player);
        }

        /// <summary>
        /// Overrides the base <see cref="AnimoraClip.Enter"/> method.
        /// Calls the base enter logic and triggers the <see cref="AnimoraActionGroup.OnEnterClip"/> actions.
        /// </summary>
        public override void Enter()
        {
            // Call the enter logic from the base AnimoraClip class.
            base.Enter();
            // Trigger the 'OnEnterClip' actions, targeting the associated components.
            ActionsManager.OnEnter(Targets.GetTargets(), this, Player);
        }

        /// <summary>
        /// Overrides the base <see cref="AnimoraClip.Exit"/> method.
        /// Calls the base exit logic and triggers the <see cref="AnimoraActionGroup.OnExitClip"/> actions.
        /// </summary>
        public override void Exit()
        {
            // Call the exit logic from the base AnimoraClip class.
            base.Exit();
            // Trigger the 'OnExitClip' actions, targeting the associated components.
            ActionsManager.OnExit(Targets.GetTargets(), this, Player);
        }

        /// <summary>
        /// Overrides the base <see cref="AnimoraClip.OnPreviewChanged"/> method.
        /// Calls the base preview change logic and notifies the <see cref="AnimoraActionsManager"/> about the change.
        /// </summary>
        /// <param name="animoraPlayer">The player instance associated with the preview state change.</param>
        /// <param name="isOn">True if preview mode is being activated, false if deactivated.</param>
        public override void OnPreviewChanged(AnimoraPlayer animoraPlayer, bool isOn)
        {
            // Call the preview changed logic from the base AnimoraClip class.
            base.OnPreviewChanged(animoraPlayer, isOn);
            // Notify the ActionsManager about the preview state change, passing relevant context.
            ActionsManager.OnPreviewChanged(animoraPlayer, isOn, Targets.GetTargets(), this);
        }

        /// <summary>
        /// Implements the abstract <see cref="AnimoraClip.GetTargetType"/> method.
        /// Returns the specific <see cref="Component"/> type <typeparamref name="T"/> that this clip targets.
        /// </summary>
        /// <returns>The <see cref="Type"/> of the target component.</returns>
        public override Type GetTargetType()
        {
            // Return the actual System.Type object for the generic parameter T.
            return typeof(T);
        }

        /// <summary>
        /// Implements the abstract <see cref="AnimoraClip.GetTargets"/> method.
        /// Retrieves the list of target components from the <see cref="targets"/> manager
        /// and casts them to the base <see cref="Component"/> type.
        /// </summary>
        /// <returns>A list of target components as the base <see cref="Component"/> type.</returns>
        public override List<Component> GetTargets()
        {
            // Get the list of specific target components (type T).
            // Use LINQ Cast<Component>() to convert each element to the base Component type.
            // Use ToList() to convert the resulting IEnumerable back to a List<Component>.
            // Handle potential null reference if 'targets' is not initialized.
            return Targets?.GetTargets()?.Cast<Component>().ToList() ?? new List<Component>();
        }

        /// <summary>
        /// Overrides the base <see cref="AnimoraClip.OnDrop"/> method.
        /// Handles the event when compatible objects (Components or GameObjects) are dragged
        /// and dropped onto this clip in the timeline editor. Delegates the handling to the
        /// <see cref="AnimoraTargets{T}.OnDrop"/> method.
        /// </summary>
        /// <param name="objectReferences">The array of objects dropped onto the clip.</param>
        /// <param name="player">The timeline player instance (provides context like RecordUndo).</param>
        /// <returns>True if the drop operation was successfully handled, false otherwise.</returns>
        public override bool OnDrop(Object[] objectReferences, IOM_TimelinePlayer<AnimoraClip> player)
        {
            // Delegate the drop handling logic to the AnimoraTargets instance.
            // Handle potential null reference if 'targets' is not initialized.
            return this.Targets?.OnDrop(objectReferences, player) ?? false;
        }

        /// <summary>
        /// Checks if the clip has any configuration errors that would prevent playback.
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public override bool HasError(out string error)
        {
            // Call the base error checking logic.
            var hasError = base.HasError(out error);
            // If no error is found, check the targets for errors.
            
            if (targets == null)
            {
                error = "Targets is not set";
                return true;
            }
            
            if (!hasError)
            {
                // Check if the targets manager has any errors and set the error message accordingly.
                hasError = targets.HasError(out error);
            }

            return hasError; // Return true if any error was found, false otherwise.
        }
    }
}