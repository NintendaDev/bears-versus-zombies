namespace OM.Animora.Runtime
{
    /// <summary>
    /// Defines the mode of operation for actions that modify a value,
    /// typically used in "Set" actions (e.g., Set Position, Set Rotation).
    /// Determines whether the action should directly set the target value or
    /// add to the existing value.
    /// </summary>
    public enum AnimoraSetType
    {
        /// <summary>
        /// The action directly sets the target property to the specified value,
        /// overwriting any previous value.
        /// </summary>
        Set,

        /// <summary>
        /// The action adds the specified value to the target property's current value.
        /// Requires the target property type to support addition.
        /// </summary>
        Add,
    }
}