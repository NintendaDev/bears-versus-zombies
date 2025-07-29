using UnityEngine;

namespace OM
{
    /// <summary>
    /// MinMaxSliderAttribute is used to create a slider in the inspector that allows you to select a range of values.
    /// </summary>
    public class OM_MinMaxSliderAttribute : PropertyAttribute
    {
        public readonly float Min;
        public readonly float Max;

        
        public OM_MinMaxSliderAttribute(float min, float max)
        {
            this.Min = min;
            this.Max = max;
        }
    }
}