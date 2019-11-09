using System.Collections.Generic;

namespace Acryl.Helpers
{
    public class ChildrenContainer<T> where T : ChildrenContainer<T>
    {
        protected readonly List<T> Children = new List<T>();

        public T Parent;

        public void Add(T child)
        {
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
    }
}