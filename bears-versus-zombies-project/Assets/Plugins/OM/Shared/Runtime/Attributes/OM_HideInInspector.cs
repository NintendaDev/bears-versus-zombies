using System;
using UnityEngine;

namespace OM
{
    /// <summary>
    /// used to hide a field in the inspector.
    /// it should be implemented for each editor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class OM_HideInInspector : PropertyAttribute
    {
        public bool HandleOtherAttributes { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="OM_HideInInspector"/> class.
        /// </summary>
        /// <param name="handleOtherAttributes"></param>
        public OM_HideInInspector(bool handleOtherAttributes = true)
        {
            HandleOtherAttributes = handleOtherAttributes;
        }
    }
}