using Acryl.Engine.Graphics.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl.Engine.Graphics.MonoGame.ImGui
{
    public class MonoImGui : Drawable
    {
        private ImGuiRenderer _renderer;

        private bool _isLoaded;

        [LoadAsync]
        private void Load(GraphicsDevice device, GameBase game)
        {
            _renderer = new ImGuiRenderer(game);
            _renderer.RebuildFontAtlas();
            _isLoaded = true;
            
            DrawChildren = false;
        }

        protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!Visible || !_isLoaded)
                return;
            
            _renderer.BeforeLayout(gameTime);

            foreach (var child in Children) // Draw inside ImGui's Batch.
            {
                child.DrawFrame(spriteBatch, gameTime);
            }
            
            _renderer.AfterLayout();
            
            base.Draw(spriteBatch, gameTime);
        }
    }
}