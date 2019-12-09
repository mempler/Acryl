using System;
using System.Collections.Generic;
using System.Linq;
using Acryl.Engine.Graphics.Core;

namespace Acryl.Engine.Utility
{
    public interface IChildrenContainer<T> where T : IChildrenContainer<T>
    {
        List<T> Children { get; }
        T Child { get; }
        T Parent { get; }

        void Add(T child);
        void Remove(T child);
    }
    
    public class ChildrenContainer<T> : DependencyContainer, IDisposable, IChildrenContainer<T> where T : ChildrenContainer<T>
    {
        private readonly List<T> _children = new List<T>();
        public List<T> Children => _children;
        
        public T Child
        {
            get
            {
                lock(Children)
                    return Children.Count == 1 ?
                        Children.FirstOrDefault() :
                        null;
            }
            set
            {
                lock(Children)
                    if (Children.Count <= 0)
                        Children.Add(value);
            }
        }

        public T Parent { get; private set; }

        public void Add(T child)
        { 
            if (child is DependencyContainer container)
            {
                container.Parent = this;
            }
            
            AsyncLoadingPipeline.LoadForObject(child, this).Wait(); // Lets load for Drawable first.
            AsyncLoadingPipeline.LoadForObject(child.GetType(), child, this).Wait();
            
            child.Parent = (T) this;
            lock (Children)
                Children.Add(child);
        }
        
        public void Remove(T child)
        {
            child.Parent = null;
            lock (Children)
                Children.Remove(child);
        }

        public virtual void Dispose(bool isDisposing)
        {
            lock (Children)
                foreach (var child in Children)
                    child?.Dispose(isDisposing);
        }
        
        public void Dispose()
        {
            Dispose(true);
        }

        ~ChildrenContainer()
        {
            Dispose(false);
        }
    }
}