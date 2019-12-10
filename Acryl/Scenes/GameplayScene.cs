using System;
using Acryl.Engine;
using Acryl.Engine.Graphics;
using Acryl.Engine.Graphics.Core;
using Acryl.Engine.Graphics.ImGui;
using Acryl.Engine.Stores;
using Acryl.ImGuiLayouts;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl.Scenes
{
    public class GameplayScene : Scene
    {
        private CachedRenderTarget Background;

        [LoadAsync]
        private void Load(TextureStore store, MonoImGui imGui)
        {
            Background = new CachedRenderTarget();

            var t = store.Get(
                "https://images.unsplash.com/photo-1519638399535-1b036603ac77?ixlib=rb-1.2.1&w=1000&q=80");
            var s = new Sprite(t);
            
            s.PositionOrigin = Origin.Center;
            s.Origin = Origin.Center;
            s.Scale = Field.ApplyScale(s);
            
            Add(Background);
            
            Background.Add(s);
            Background.BlurStrength = 2f;
            Background.Invalidate();
            Background.PositionOrigin = Origin.Center;
            Background.Origin = Origin.Center;
            //Background.Scale = Field.ApplyScale(s);
            
            imGui.Add(new CachedRenderTargetLayout(Background));
        }
    }
}