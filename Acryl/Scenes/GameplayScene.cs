using Acryl.Engine;
using Acryl.Engine.Graphics;
using Acryl.Engine.Graphics.Core;
using Acryl.Engine.Graphics.ImGui;
using Acryl.Engine.Stores;
using Acryl.ImGuiLayouts;
using Microsoft.Xna.Framework;

namespace Acryl.Scenes
{
    public class GameplayScene : Scene
    { 
        private CachedRenderTarget Background;
        
        [LoadAsync]
        private void Load(TextureStore store, MonoImGui imGui)
        {
            Background = new CachedRenderTarget
            {
                PositionOrigin = Origin.Center,
                Origin = Origin.Center
            };
            
            var t = store.Get(
                "https://images.unsplash.com/photo-1519638399535-1b036603ac77?ixlib=rb-1.2.1&w=1000&q=80");
            
            var s = new Sprite(t)
            {
                PositionOrigin = Origin.Center,
                Origin = Origin.Center
            };

            s.Scale = Field.ApplyScale(s);
            
            Add(Background);
            
            Background.Add(s);
            Background.BlurStrength = 2f;
            Background.Invalidate();
            //Background.Scale = Field.ApplyScale(s);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}