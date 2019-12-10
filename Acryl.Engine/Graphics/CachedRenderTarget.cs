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
        private RenderTarget2D _target2D;
        
        private bool Invalidated { get; set; }

        public void Invalidate()
        {
            Invalidated = true;
        }
        
        public Effect Effect { get; set; }

        [LoadAsync]
        private void Load()
        {
            _customBatch = new SpriteBatch(Device);
            
            _target2D = new RenderTarget2D(Device, (int) Field.Width, (int) Field.Height);
        }

        protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {

            if (Invalidated)
            {
                Device.SetRenderTarget(_target2D);
                _customBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied, effect: Effect);
                base.Draw(_customBatch, gameTime);
                _customBatch.End();
                Device.SetRenderTarget(null);

                Invalidated = false;
            }

            var (color, pos, rotation, scale, origin) = CalculateFrame(Field.Width, Field.Height);
            
            spriteBatch.Draw(
                _target2D,
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