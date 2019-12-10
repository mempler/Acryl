using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Acryl.Engine.Graphics.Extension;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl.Engine.Stores
{
    public class TextureStore : ResourceStore<Texture2D>
    {
        private readonly Dictionary<string, Texture2D> _cachedTextures
            = new Dictionary<string, Texture2D>();

        [DependencyResolved]
        private FileResourceStore FileStore { get; set; }
        
        [DependencyResolved]
        private DllResourceStore DllResource { get; set; }
        
        [DependencyResolved]
        private OnlineStore OnlineStore { get; set; }
        
        [DependencyResolved]
        private GraphicsDevice Device { get; set; }
        
        public override Texture2D Get(string key)
        {
            if (_cachedTextures.TryGetValue(key, out var t))
                return t;

            Stream data;
            if (key.StartsWith("file://"))
                data = FileStore.GetStream(key);
            else if (key.StartsWith("http://") || key.StartsWith("https://"))
                data = OnlineStore.GetStream(key);
            else
                data = DllResource.GetStream(key);
            
            if (data == null)
                throw new FileNotFoundException($"{key} Not found!");

            Texture2D tex;
            if (!key.EndsWith(".svg"))
            {
                tex = Texture2D.FromStream(Device, data);
                _cachedTextures[key] = tex;
            
                data.Dispose();
                return tex;
            }

            tex = Device.SvgToTexture2D(data, 0, 0);
            _cachedTextures[key] = tex;
            data.Dispose();
            return tex;
        }

        public override Task<Texture2D> GetAsync(string key) => Task.Run(() => Get(key));

        public override void Dispose()
        {
            foreach (var tex in _cachedTextures)
            {
                tex.Value.Dispose();
            }
        }
    }
}