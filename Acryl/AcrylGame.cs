using System;
using Acryl.Engine;
using Acryl.Engine.Graphics.Core;
using Acryl.Engine.Graphics.MonoGame.ImGui;
using Acryl.Engine.Graphics.MonoGame.ImGui.Layouts;
using Acryl.Engine.Stores;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl
{
    public class AcrylGame : GameBase
    {
        [DependencyResolved]
        private GraphicsDevice Device { get; set; }

        private Sprite _sprite;
        private MonoImGui ImGui;
        private Text2D Text;
        
        [LoadAsync]
        private async void Load(TextureStore store)
        {
            ImGui = new MonoImGui();
            Add(ImGui);
            ImGui.Add(new ImGuiDebugger());
            
            Text = new Text2D("Hello World!") {Size = 16};
            Add(Text);
            
            var tex = await store.GetAsync("Resources/Textures/AcrylSmall.png");
            _sprite = new Sprite(tex);
            _sprite.Scale  = new Vector2(.2f, .2f);
            _sprite.Origin = Origin.Center;
            _sprite.PositionOrigin = Origin.Center;
            Add(_sprite);
        }
    }
}