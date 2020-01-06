using System;
using Acryl.Engine.Graphics.Core;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl.Engine.Graphics.ImGui
{
    public class MonoImGui : Drawable
    {
        public ImGuiRenderer Renderer { get; private set; }

        private bool _isLoaded;

        [DependencyResolved]
        private PostProcessor PostProcessor { get; set; }
        
        [LoadAsync]
        private void Load(GraphicsDevice device, GameBase game, PostProcessor postProcessor)
        {
            Renderer = new ImGuiRenderer(game);
            Renderer.RebuildFontAtlas();
            
            _isLoaded = true;
            DrawChildren = false;

            var io = ImGuiNET.ImGui.GetIO();

            io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        }

        public IntPtr PostProcessingId;
        
        protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!Visible || !_isLoaded)
                return;

            PostProcessor.DrawToScreen = false;
            
            PostProcessingId = Renderer.BindTexture(PostProcessor.RenderTexture);
            
            Renderer.BeforeLayout(gameTime);

            lock(Children)
                foreach (var child in Children) // Draw inside ImGui's Batch.
                    child.DrawFrame(spriteBatch, gameTime);

            Renderer.AfterLayout();
            
            Renderer.UnbindTexture(PostProcessingId);
        }
    }
}