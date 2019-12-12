using System;
using System.Linq;
using Acryl.Engine.Graphics.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl.Engine.Graphics
{
    public class PostProcessor : Drawable
    {
        [DependencyResolved]
        private GraphicsDevice Device { get; set; }
        
        [DependencyResolved]
        private GameBase Game { get; set; }
        
        private RenderTarget2D _ppTarget2D;

        public bool DrawToScreen { get; set; } = true;
        
        [LoadAsync]
        private void Load()
        {
            _ppTarget2D = new RenderTarget2D(Device,
                Device.PresentationParameters.BackBufferWidth,
                Device.PresentationParameters.BackBufferHeight);
            
            Game.Parent = this;
        }

        public Texture2D RenderTexture;
        public Effect Effect;
        
        protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            DrawChildren = false;
            if (!Visible || Device == null || _ppTarget2D == null)
                return;
            
            Device.SetRenderTarget(_ppTarget2D);
            spriteBatch.Begin();
            
            lock (Game.Children)
                foreach (var child in Game.Children.ToList())
                    child.DrawFrame(spriteBatch, gameTime);
            
            spriteBatch.End();
            RenderTexture = _ppTarget2D;
            
            Device.SetRenderTarget(null);
            if (!DrawToScreen)
                return;
            
            spriteBatch.Begin(effect: Effect);
            var (color, destRect, rotation, origin) = CalculateFrame(Field.Width, Field.Height);
            
            spriteBatch.Draw(RenderTexture,
                destRect,
                null,
                color,
                rotation,
                origin,
                SpriteEffects.None,
                0);
            
            spriteBatch.End();
        }
    }
}