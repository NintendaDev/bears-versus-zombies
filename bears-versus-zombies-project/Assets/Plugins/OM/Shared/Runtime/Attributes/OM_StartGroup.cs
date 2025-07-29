using System;

namespace OM
{
    /// <summary>
    /// StartGroupAttribute is used to create a group in the inspector.
    /// It allows you to specify a group name and an optional color.
    /// should be implemented for each custom editor separately.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class OM_StartGroup : Attribute
    {
        public string GroupName { get; }
        public string GroupColor { get; }

        public OM_StartGroup(string groupName, string groupColor = null)
        {
            GroupName = groupName;
            GroupColor = groupColor ?? groupName;
        }
    }
    
    /// <summary>
    /// EndGroupAttribute is used to create a group in the inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class OM_EndGroup : Attribute
    {
        
    }
}