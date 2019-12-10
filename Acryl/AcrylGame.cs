using Acryl.Engine;
using Acryl.Engine.Graphics;
using Acryl.Engine.Graphics.Core;
using Acryl.Engine.Graphics.ImGui;
using Acryl.Engine.Graphics.ImGui.Layouts;
using Acryl.Engine.Stores;
using Acryl.ImGuiLayouts;
using Acryl.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl
{
    public class AcrylGame : GameBase
    {
        [DependencyResolved]
        private GraphicsDevice Device { get; set; }
        
        private MonoImGui ImGui;

        [LoadAsync]
        private void Load(TextureStore store)
        {
            ImGui = new MonoImGui();
            Add(ImGui); // Load dependencies
            //ImGui.Add(new ImGuiDebugger());
            Remove(ImGui); // Remove from children.
            Dependencies.Add(ImGui);

            /*
            Text = new Text2D("Hello World!") {Size = 16};
            Add(Text);
            
            var tex = await store.GetAsync("Resources/Textures/AcrylSmall.png");
            _sprite = new Sprite(tex)
            {
                Scale = new Vector2(.2f, .2f),
                Origin = Origin.Center,
                PositionOrigin = Origin.Center,
            };
            
            Add(_sprite);
            
            _sprite
                .MoveTo(new Vector2(-100, 0), 5, 0)
                .Easing(EasingFunctions.ExponentialInOut);
            
            _sprite
                .ScaleTo(new Vector2(.1f, .1f), 2, 0)
                .Easing(EasingFunctions.BounceOut);
            */

            Remove(ActiveScene);
            ActiveScene = new StartupScene();
            Add(ActiveScene);
            
            SwitchScene(new GameplayScene(), 2, 4);
            /*
            ActiveScene = new StartupScene();
            Add(ActiveScene);

            ActiveScene.FadeTo(0, 1, 5)
                .OnEnd(x =>
                {
                    Remove(ActiveScene);
                    
                    ActiveScene = new GameplayScene();
                    
                    Add(ActiveScene);
                });
            */

            //.OnEnd(s =>);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            
            SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied);
            
            ImGui?.UpdateFrame(gameTime); // Always draw ImGui on top of Everything!
            ImGui?.DrawFrame(SpriteBatch, gameTime);

            SpriteBatch.End();
        }
    }
}