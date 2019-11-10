using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Acryl.Graphics.Origin;

namespace Acryl.Graphics.Elements
{
    public class Sprite : Drawable, IDisposable
    {
        public Sprite(Texture2D tex)
        {
            Texture = tex;
        }
        
        public Texture2D Texture { get; set; }
        
        protected override void Update(GameTime gameTime)
        {
            
        }
        
        protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!Visible || Texture == null)
                return;
            
            var alpha = Alpha * (Parent?.Alpha ?? 1f);
            
            var origin = new Vector2();

            if ((Origin & Top) != 0)
                origin.Y = 0;
            if ((Origin & Bottom) != 0)
                origin.Y = Texture.Height;
            
            if ((Origin & Left) != 0)
                origin.Y = 0;
            if ((Origin & Right) != 0)
                origin.X = Texture.Width;
            
            if ((Origin & Center) != 0) {
                origin.X = Texture.Width / 2f;
                origin.Y = Texture.Height / 2f;
                
                if ((Origin & Top) != 0)
                    origin.Y = 0;
                
                else if ((Origin & Bottom) != 0)
                    origin.Y = Texture.Height;
            
                if ((Origin & Left) != 0)
                    origin.Y = 0;
                else if ((Origin & Right) != 0)
                    origin.X = Texture.Width / 2f;
            }

            var originScale = AcrylGame.Field.ApplyScale(origin);
            var positionOffset = new Vector2();
            if ((PositionOrigin & Top) != 0)
                positionOffset.X = 0;
            if ((PositionOrigin & Bottom) != 0)
                positionOffset.Y = AcrylGame.Field.Height-originScale.Y;
            
            if ((PositionOrigin & Left) != 0)
                positionOffset.Y = 0;
            if ((PositionOrigin & Right) != 0)
                positionOffset.X = AcrylGame.Field.Width-originScale.X;
            
            if ((PositionOrigin & Center) != 0) {
                positionOffset.X = (AcrylGame.Field.Width-originScale.X) / 2f;
                positionOffset.Y = (AcrylGame.Field.Height-originScale.Y) / 2f;
                
                if ((PositionOrigin & Top) != 0)
                    positionOffset.Y = 0;
                
                else if ((PositionOrigin & Bottom) != 0)
                    positionOffset.Y = AcrylGame.Field.Height-originScale.Y;
            
                if ((PositionOrigin & Left) != 0)
                    positionOffset.Y = 0;
                
                else if ((PositionOrigin & Right) != 0)
                    positionOffset.X = (AcrylGame.Field.Width-originScale.X) / 2f;
            }
            
            spriteBatch.Draw(
                Texture,
                Position + PositionOffset + positionOffset +
                    (Parent?.Position ?? Vector2.Zero) + (Parent?.PositionOffset ?? Vector2.Zero),
                null,
                new Color(Color, Math.Min(alpha, 1f)),
                Rotation + (Parent?.Rotation ?? 0),
                origin,
                Scale * (Parent?.Scale ?? Vector2.One),
                Effects,
                0
                );
        }

        public void Dispose()
        {
            Texture?.Dispose();
        }
        
        public static implicit operator Texture2D(Sprite x)
        {
            return x.Texture;
        }
    }
}