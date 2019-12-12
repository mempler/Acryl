using Acryl.Engine.Graphics.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl.Engine.Graphics.ImGui
{
    public class MonoImGui : Drawable
    {
        public ImGuiRenderer Renderer { get; private set; }

        private bool _isLoaded;

        [LoadAsync]
        private void Load(GraphicsDevice device, GameBase game)
        {
            Renderer = new ImGuiRenderer(game);
            Renderer.RebuildFontAtlas();
            
            _isLoaded = true;
            DrawChildren = false;
        }

        protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!Visible || !_isLoaded)
                return;
            
            Renderer.BeforeLayout(gameTime);

            lock(Children)
                foreach (var child in Children) // Draw inside ImGui's Batch.
                    child.DrawFrame(spriteBatch, gameTime);

            Renderer.AfterLayout();
        }
    }
}