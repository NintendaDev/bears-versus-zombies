namespace Modules.Types
{
    public abstract class MemorizedValue<T> where T : struct
    {
        private bool _isSet;
        public T? Previous { get; private set; }
        
        public T Current { get; private set; }
        
        public void Update(T value)
        {
            Previous = Current;
            Current = value;
        }

        public bool IsChanged()
        {
            return Previous != null && Current.Equals(Previous) == false;
        }

        public void Reset()
        {
            Previous = null;
            Current = default;
        }
    }
}
