using Acryl.Engine;
using Acryl.Engine.Graphics;
using Acryl.Engine.Graphics.Core;
using Acryl.Engine.Stores;

namespace Acryl.Scenes
{
    public class GameplayScene : Scene
    {
        private CachedRenderTarget Background;

        [LoadAsync]
        private void Load(TextureStore store)
        {
            Background = new CachedRenderTarget();

            var t = store.Get(
                "https://images.unsplash.com/photo-1519638399535-1b036603ac77?ixlib=rb-1.2.1&w=1000&q=80");
            var s = new Sprite(t);
            
            s.Scale = Field.ApplyScale(s);
            
            Add(Background);
            
            Background.Add(s);
        }
    }
}