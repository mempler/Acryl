using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SharpFont;

namespace Acryl.Engine.Stores
{
    public class FontFaceStore : ResourceStore<Face>
    {
        [DependencyResolved]
        private Library Library { get; set; }
        
        [DependencyResolved]
        private DllResourceStore DllResourceStore { get; set; }
        
        public override Face Get(string key)
        {
            var file = DllResourceStore.Get(key + ".ttf");
            if (file == null)
                return null;
            
            var face = Library.NewMemoryFace(file, 0);
            return face;
        }
        public override Task<Face> GetAsync(string key) => Task.Run(() => Get(key));
        
        public override void Dispose()
        {
        }
    }
}