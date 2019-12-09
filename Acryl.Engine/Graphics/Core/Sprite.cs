using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl.Engine.Graphics.Core
{
    public class Sprite : Drawable, IDisposable
    {
        public Vector2 Size => new Vector2(Texture.Width, Texture.Height);
        
        public Sprite(Texture2D tex)
        {
            Texture = tex;
        }
        
        public Texture2D Texture { get; set; }
        
        protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!Visible || Texture == null)
                return;

            var (color, pos, rotation, scale, origin) = CalculateFrame(Size.X, Size.Y);
            
            spriteBatch.Draw(
                Texture,
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

        public override void Dispose(bool isDisposing)
        {
            Texture?.Dispose();
            base.Dispose(isDisposing);
        }
        
        public static implicit operator Texture2D(Sprite x)
        {
            return x.Texture;
        }
    }
}