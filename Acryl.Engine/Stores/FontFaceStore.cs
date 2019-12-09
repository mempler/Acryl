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
        
        
        private readonly Dictionary<string, Face> _cachedFonts = new Dictionary<string, Face>();
        
        public override Face Get(string key)
        {
            if (_cachedFonts.TryGetValue(key, out var f))
                return f;
            
            var file = DllResourceStore.Get(key + ".ttf");
            if (file == null)
                return null;
            
            var face = Library.NewMemoryFace(file, 0);
            _cachedFonts[key] = face;
            return face;
        }
        public override Task<Face> GetAsync(string key) => Task.Run(() => Get(key));

        public override void Dispose()
        {
            foreach (var f in _cachedFonts)
            {
                f.Value.Dispose();
            }
        }
    }
}