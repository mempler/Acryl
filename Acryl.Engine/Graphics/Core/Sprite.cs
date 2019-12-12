using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;

namespace Acryl.Engine.Graphics.Core
{
    public class Sprite : Drawable, IDisposable
    {
        public Sprite(Texture2D tex)
        {
            Texture = tex;
        }

        private Texture2D _tex;

        public Texture2D Texture
        {
            get => _tex;
            set
            {
                _tex = value;
                Size = new Vector2(_tex.Width, _tex.Height);
            }
        }
        
        protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!Visible || Texture == null)
                return;
            
            var (color, destRect, rotation, origin) = CalculateFrame(Field.Width, Field.Height);
            
            spriteBatch.Draw(Texture,
                destRect,
                null,
                color,
                rotation,
                origin,
                SpriteEffects.None,
                0);
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