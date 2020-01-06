using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl.Engine.Stores
{
    public class EffectStore : ResourceStore<Effect>
    {
        [DependencyResolved]
        private DllResourceStore DllResourceStore { get; set; }
        
        [DependencyResolved]
        private GraphicsDevice Device { get; set; }
        
        private Dictionary<string, Effect> _effectCache
            = new Dictionary<string, Effect>();

        public override Effect Get(string key)
        {
            if (_effectCache.TryGetValue(key, out var e))
                return e;
            
            var f = DllResourceStore.Get(key.Replace(".fx", "") + ".xnb");
            e = new Effect(Device, f);

            _effectCache[key] = e;
            
            return e;
        }

        public override async Task<Effect> GetAsync(string key)
        {
            if (_effectCache.TryGetValue(key, out var e))
                return e;
            
            var f = await DllResourceStore.GetAsync(key.Replace(".fx", "") + ".xnb");
            e = new Effect(Device, f);

            _effectCache[key] = e;
            
            return e;
        }

        public override void Dispose()
        {
            foreach (var e in _effectCache)
            {
                e.Value.Dispose();
            }
        }
    }
}