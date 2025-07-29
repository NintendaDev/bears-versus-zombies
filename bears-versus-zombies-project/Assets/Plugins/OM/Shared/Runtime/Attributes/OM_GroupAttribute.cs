using System;
using UnityEngine;

namespace OM
{
    /// <summary>
    /// An attribute to group properties in the inspector.
    /// it allows you to specify a group name, color, and an optional toggle name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class OM_GroupAttribute : PropertyAttribute
    {
        public string GroupName { get; }
        public string GroupColor { get; }
        public string ToggleName { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="OM_GroupAttribute"/> class
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="groupColor"></param>
        /// <param name="toggleName"></param>
        public OM_GroupAttribute(string groupName, string groupColor, string toggleName = null)
        {
            GroupName = groupName;
            GroupColor = groupColor;
            ToggleName = toggleName;
        }
    }
}