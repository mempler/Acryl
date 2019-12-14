using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = System.Numerics.Vector2;

namespace Acryl.Engine.Graphics.Core
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
                (int) Field.Width,
                (int) Field.Height,
                false,
                Device.PresentationParameters.BackBufferFormat,
                DepthFormat.None,
                32,
                RenderTargetUsage.PreserveContents);
            
            Invalidate();
            
            _hasLoaded = true;
        }
        
        public Texture2D CachedTexture { get; private set; }
        protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            DrawChildren = false;
            if (!_hasLoaded)
                return;
            
            if (Invalidated)
            {
                Device.SetRenderTarget(_renderTarget2D);
                _customBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, Effect);
                _customBatch.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Transparent);
            
                lock(Children)
                    foreach (var child in Children) // Iterate through Child and it's Children to draw a Frame
                        child.DrawFrame(_customBatch, new GameTime());
            
                _customBatch.End();
                Device.SetRenderTarget(null);
            
                CachedTexture =
                    Blur ?
                        _blur.PerformGaussianBlur(_renderTarget2D) :
                        _renderTarget2D;
                
                Size = new Vector2(CachedTexture.Width, CachedTexture.Height);
                
                Invalidated = false;
            }
            
            var (color, destRect, rotation, origin) = CalculateFrame(Field.Width, Field.Height);
            
            spriteBatch.Draw(CachedTexture,
                new Rectangle(destRect.X, destRect.Y, destRect.Width, destRect.Height), 
                null,
                new Color(color.R, color.G, color.B, color.A),
                rotation,
                new Microsoft.Xna.Framework.Vector2(origin.X, origin.Y), 
                SpriteEffects.None,
                0);
        }
    }
}