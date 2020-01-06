using System;
using System.IO;
using System.Threading.Tasks;

namespace Acryl.Engine.Stores
{
    public interface IResourceStore<T> : IDependencyContainer, IDisposable
    {
        T Get(string key);
        Task<T> GetAsync(string key);
        
        Stream GetStream(string key);
    }

    public abstract class ResourceStore<T> : DependencyContainer, IResourceStore<T>
    {
        public abstract T Get(string key);
        public abstract Task<T> GetAsync(string key);
        public virtual Stream GetStream(string key) => throw new NotImplementedException();
        
        public abstract void Dispose();
    }
}