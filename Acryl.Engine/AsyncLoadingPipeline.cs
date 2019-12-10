using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Acryl.Engine
{
    public class DependencyNotFoundException : Exception
    {
        public override string Message { get; }

        public DependencyNotFoundException(bool isProp, string argumentName, Type type)
        {
            var propKind = isProp ? "Property" : "Argument";
            Message =  $"{propKind} {argumentName} could not be Resolved! in type {type}";
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    [MeansImplicitUse]
    public class DependencyResolvedAttribute : Attribute { }

    public interface IDependencyContainer
    {
        IDependencyContainer Parent { get;  }
        object ResolveDependency(Type t, string hint = "", bool check = true);
    }
    
    public class DependencyContainer : IDependencyContainer
    {
        private readonly List<(Type type, string hint, object dependency)> _dependencies
            = new List<(Type, string, object)>();

        public IDependencyContainer Parent { get; internal set; }
        public IDependencyContainer RootParent
        {
            get
            {
                var lastNonNullParent = Parent;
                var parent = lastNonNullParent;

                do
                {
                    parent = parent?.Parent;
                    if (parent != null)
                        lastNonNullParent = parent;
                } while (parent?.Parent != null);

                return lastNonNullParent;
            }
        }

        public bool IsRootParent => Parent == null;
        
        protected internal void Add(object dependency, string hint = "")
        {
            _dependencies.Add((dependency.GetType(), hint, dependency));
        }
        
        public void Add<T>(T dependency, string hint = "")
        {
            if (hint == "")
                hint = dependency.GetType().Name;
            
            if (dependency is DependencyContainer container &&
                this != dependency as DependencyContainer)
            {
                container.Parent = this;
                DependencyInjector.InjectIntoObject(dependency, this);
            }

            _dependencies.Add((typeof(T), hint, dependency));
        }

        public object ResolveDependency(Type t, string hint = "", bool check = false)
        {
            if (!IsRootParent && !check)
                return RootParent?.ResolveDependency(t, hint, true);
            
            foreach (var (depType, rHint, dependency) in _dependencies.Where(dep => dep.dependency != null))
            {
                if (typeof(DependencyContainer).IsSubclassOf(dependency.GetType()))
                {
                    object correctDep;
                    if ((correctDep = ((DependencyContainer) dependency).ResolveDependency(t, hint)) != null) // Maybe our Dep is in one of the Children
                        return correctDep;
                }
                
                if (t == depType && rHint.Contains(hint, StringComparison.CurrentCultureIgnoreCase)) // Not in one our children ? well, lets check our own Dictionary.
                    return dependency;

                if (t == depType)
                    return dependency;
            }

            return null; // Still not found ? screw it, lets return null!
        }
    }
    
    public static class DependencyInjector
    {
        public static void InjectIntoMethod(DependencyContainer obj, MethodInfo method)
            => InjectIntoMethod(obj, obj, method); // looks weird, but works.

        public static Task InjectIntoMethod(object obj, DependencyContainer container, MethodInfo method)
        {
            var args = new List<object>();
            foreach (var arg in method.GetParameters())
            {
                var resolved = container.ResolveDependency(arg.ParameterType, arg.Name);

                if (resolved == null)
                    throw new DependencyNotFoundException(false, arg.Name, obj.GetType());
                
                args.Add(resolved);
            }

            return (Task) method.Invoke(obj, args.ToArray());
        }
        
        public static void InjectIntoObject(DependencyContainer obj)
            => InjectIntoObject(obj, obj);
        
        public static void InjectIntoObject(object obj, DependencyContainer container)
        {
            foreach (var prop in obj
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(x => x.GetCustomAttributes(typeof(DependencyResolvedAttribute), false).Length > 0))
            {
                var resolved = container.ResolveDependency(prop.PropertyType, prop.Name);
                
                if (resolved == null)
                    throw new DependencyNotFoundException(true, prop.Name, obj.GetType());
                
                prop.SetValue(obj, resolved);
            }
        }
        
        public static void InjectIntoObject(Type t, object obj, DependencyContainer container)
        {
            foreach (var prop in t
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(x => x.GetCustomAttributes(typeof(DependencyResolvedAttribute), false).Length > 0))
            {
                var resolved = container.ResolveDependency(prop.PropertyType, prop.Name);
                
                if (resolved == null)
                    throw new DependencyNotFoundException(true, prop.Name, obj.GetType());
                
                prop.SetValue(obj, resolved);
            }
        }
    }
    
    [AttributeUsage(AttributeTargets.Method)]
    [MeansImplicitUse]
    public class LoadAsyncAttribute : Attribute { }
    internal static class AsyncLoadingPipeline
    {
        internal static Task LoadForObject(DependencyContainer obj)
            => LoadForObject(obj, obj);

        public static Task LoadForObject<T>(T obj, DependencyContainer container)
            => LoadForObject(typeof(T), obj, container);
        
        internal static async Task LoadForObject(Type type, object obj, DependencyContainer container)
        {
            DependencyInjector.InjectIntoObject(type, obj, container);
            
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => m.GetCustomAttributes(typeof(LoadAsyncAttribute), false).Length > 0)
                .ToList();
            
            if (methods.Count > 1)
                throw new NotSupportedException("Please do NOT use [LoadAsync] multiple times!");

            foreach (var method in methods)
            {
                if (!method.IsPrivate)
                    throw new NotSupportedException("[LoadAsync] MUST be private!");
                
                if (method.IsAbstract)
                    throw new NotSupportedException("[LoadAsync] CANNOT be abstract!");
                
                var x = DependencyInjector.InjectIntoMethod(obj, container, method);
                if (x != null) // this allos it to be Sync.
                    await x;
            }
        }
    }
}