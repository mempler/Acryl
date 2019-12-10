using Acryl.Engine.Graphics.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl.Engine.Graphics
{
    public class CachedRenderTarget : Drawable
    {
        [DependencyResolved]
        private GraphicsDevice Device { get; set; }
        
        private SpriteBatch _customBatch;

        private RenderTarget2D _renderTarget2D;
        private GaussianBlur _blur;

        public float BlurStrength
        {
            get => _blur.Amount;
            set
            {
                _blur.ComputeKernel(7, value);
                Invalidate();
            }
        }
        
        private bool Invalidated;
        public void Invalidate()
        {
            Invalidated = true;
        }
        
        public Effect Effect { get; set; }
        
        private bool IsBlurEnabled { get; set; }
        public bool Blur
        {
            get => IsBlurEnabled;
            set
            {
                IsBlurEnabled = value;
                Invalidate();
            }
        }

        private bool _hasLoaded;

        [LoadAsync]
        private void Load()
        {
            _customBatch = new SpriteBatch(Device);
            _blur = new GaussianBlur(1, _customBatch) {Parent = this};

            AsyncLoadingPipeline.LoadForObject(_blur, this);
            
            _renderTarget2D = new RenderTarget2D(
                Device,
                Device.PresentationParameters.BackBufferWidth,
                Device.PresentationParameters.BackBufferHeight,
                false,
                Device.PresentationParameters.BackBufferFormat,
                DepthFormat.None,
                32,
                RenderTargetUsage.PreserveContents);
            
            Invalidate();
            
            _hasLoaded = true;
        }
        
        private Texture2D _cachedTexture;
        protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            DrawChildren = false;
            if (!_hasLoaded)
                return;
            
            if (Invalidated)
            {
                Device.SetRenderTarget(_renderTarget2D);
                _customBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, effect: Effect);
                _customBatch.GraphicsDevice.Clear(Color.Transparent);
            
                lock(Children)
                    foreach (var child in Children) // Iterate through Child and it's Children to draw a Frame
                        child.DrawFrame(_customBatch, new GameTime());
            
                _customBatch.End();
                Device.SetRenderTarget(null);
            
                _cachedTexture =
                    Blur ?
                        _blur.PerformGaussianBlur(_renderTarget2D) :
                        _renderTarget2D;
            }


            var (color, pos, rotation, scale, origin) = CalculateFrame(Field.Width, Field.Height);
            
            spriteBatch.Draw(
                _cachedTexture,
                pos,
                null,
                color,
                rotation,
                origin,
                scale,
                SpriteEffects.None,
                0
            );
        }
    }
}