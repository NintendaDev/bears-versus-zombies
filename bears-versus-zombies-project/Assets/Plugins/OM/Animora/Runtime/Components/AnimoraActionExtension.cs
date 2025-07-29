namespace OM.Animora.Runtime
{
    /// <summary>
    /// Provides extension methods for the <see cref="AnimoraAction"/> class,
    /// allowing for a fluent configuration syntax when creating or modifying actions.
    /// </summary>
    public static class AnimoraActionExtension
    {
        /// <summary>
        /// Sets the <see cref="AnimoraAction.ActionName"/> for the specified action.
        /// </summary>
        /// <typeparam name="T">The type of the AnimoraAction being modified.</typeparam>
        /// <param name="action">The action instance to modify.</param>
        /// <param name="name">The new name to assign to the action.</param>
        /// <returns>The modified action instance, allowing for method chaining.</returns>
        public static T SetActionName<T>(this T action, string name) where T : AnimoraAction
        {
            action.ActionName = name; // Set the ActionName property
            return action; // Return the same action instance for chaining
        }

        /// <summary>
        /// Sets the <see cref="AnimoraAction.IsEnabled"/> state for the specified action.
        /// </summary>
        /// <typeparam name="T">The type of the AnimoraAction being modified.</typeparam>
        /// <param name="action">The action instance to modify.</param>
        /// <param name="isEnabled">The desired enabled state (true or false).</param>
        /// <returns>The modified action instance, allowing for method chaining.</returns>
        public static T SetIsEnabled<T>(this T action, bool isEnabled) where T : AnimoraAction
        {
            action.IsEnabled = isEnabled; // Set the IsEnabled property
            return action; // Return the same action instance for chaining
        }

        /// <summary>
        /// Sets the <see cref="AnimoraAction.ActionGroup"/> for the specified action.
        /// </summary>
        /// <typeparam name="T">The type of the AnimoraAction being modified.</typeparam>
        /// <param name="action">The action instance to modify.</param>
        /// <param name="group">The <see cref="AnimoraActionGroup"/> to assign to the action.</param>
        /// <returns>The modified action instance, allowing for method chaining.</returns>
        public static T SetComponentGroup<T>(this T action, AnimoraActionGroup group) where T : AnimoraAction
        {
            action.ActionGroup = group; // Set the ComponentGroup property
            return action; // Return the same action instance for chaining
        }

        /// <summary>
        /// Sets the <see cref="AnimoraAction.OrderIndex"/> for the specified action.
        /// </summary>
        /// <typeparam name="T">The type of the AnimoraAction being modified.</typeparam>
        /// <param name="action">The action instance to modify.</param>
        /// <param name="orderIndex">The desired order index (lower values execute first within a group).</param>
        /// <returns>The modified action instance, allowing for method chaining.</returns>
        public static T SetOrderIndex<T>(this T action, int orderIndex) where T : AnimoraAction
        {
            action.OrderIndex = orderIndex; // Set the OrderIndex property
            return action; // Return the same action instance for chaining
        }

        /// <summary>
        /// Sets the <see cref="AnimoraAction.PlayDirection"/> for the specified action.
        /// </summary>
        /// <typeparam name="T">The type of the AnimoraAction being modified.</typeparam>
        /// <param name="action">The action instance to modify.</param>
        /// <param name="playDirection">The <see cref="AnimoraActionPlayDirection"/> defining when the action should play.</param>
        /// <returns>The modified action instance, allowing for method chaining.</returns>
        public static T SetPlayDirection<T>(this T action, AnimoraActionPlayDirection playDirection) where T : AnimoraAction
        {
            action.PlayDirection = playDirection; // Set the PlayDirection property
            return action; // Return the same action instance for chaining
        }
    }
}