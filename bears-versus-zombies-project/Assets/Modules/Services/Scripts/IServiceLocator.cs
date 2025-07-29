namespace Modules.Services
{
    public interface IServiceLocator
    {
        public void Add(object service);
        
        public void Add<T>(T service);
        
        public T Get<T>();
    }
}