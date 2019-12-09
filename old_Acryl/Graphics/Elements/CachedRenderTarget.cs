using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl.Graphics.Elements
{
    public class CachedRenderTarget : Drawable, IDisposable
    {
        public IEnumerable<Sprite> Sprites { get; set; } = new List<Sprite>();
        
        /* Blur Settings */
        public bool Blur = false;
        public float BlurStrength = 1;
        public int BlurRadius = 7;
        
        private bool Invalidated;
        private RenderTarget2D _renderTarget2D;
        private GaussianBlur _blur;

        public CachedRenderTarget()
        {
            _renderTarget2D = new RenderTarget2D(
                AcrylGame.Game.GraphicsDevice,
                AcrylGame.Game.GraphicsDevice.PresentationParameters.BackBufferWidth,
                AcrylGame.Game.GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                AcrylGame.Game.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.None,
                32,
                RenderTargetUsage.PreserveContents);
        }
        
        public void Invalidate()
        {
            Invalidated = true;

            if (Blur)
            {
                if (_blur == null)
                    _blur = new GaussianBlur(0);
                
                
                _blur.ComputeKernel(BlurRadius, BlurStrength);
            }
        }

        private Texture2D _cachedTexture;
        protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (_cachedTexture == null && !Invalidated)
                Invalidate();
            
            if (Invalidated)
            {
                spriteBatch.GraphicsDevice.SetRenderTarget(_renderTarget2D);

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
                spriteBatch.GraphicsDevice.Clear(Color.Transparent);
                
                foreach (var sprite in Sprites)
                    sprite?.DrawFrame(spriteBatch, gameTime);
                
                spriteBatch.End();
                spriteBatch.GraphicsDevice.SetRenderTarget(null);

                _cachedTexture = Blur ?
                    _blur.PerformGaussianBlur(_renderTarget2D) :
                    _renderTarget2D;

                Invalidated = false;
            }

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            spriteBatch.Draw(_cachedTexture, new Rectangle(0,0, 1280, 720),
                Color.White);
            spriteBatch.End();
        }

        public void Dispose()
        {
            _renderTarget2D?.Dispose();
        }
    }
}