using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Acryl.Graphics.Origin;

namespace Acryl.Graphics.Elements
{
    public class Text2D : Drawable
    {
        private readonly string _s;
        public SpriteFont Font;

        public Text2D(string s, SpriteFont font = null)
        {
            _s = s;

            Font = font ?? AcrylGame.DefaultFont;
        }

        protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        { 
            if (!Visible || Font == null)
                return;
            
            var alpha = Alpha * (Parent?.Alpha ?? 1f);

            var measuredString = Font.MeasureString(_s);
            
            var origin = new Vector2();
            
            if ((Origin & Top) != 0)
                origin.Y = 0;
            if ((Origin & Bottom) != 0)
                origin.Y = measuredString.Y;
            
            if ((Origin & Left) != 0)
                origin.Y = 0;
            if ((Origin & Right) != 0)
                origin.X = measuredString.X;
            
            if ((Origin & Center) != 0) {
                origin.X = measuredString.X / 2f;
                origin.Y = measuredString.Y / 2f;
                
                if ((Origin & Top) != 0)
                    origin.Y = 0;
                
                else if ((Origin & Bottom) != 0)
                    origin.Y = measuredString.Y;
            
                if ((Origin & Left) != 0)
                    origin.Y = 0;
                else if ((Origin & Right) != 0)
                    origin.X = measuredString.X / 2f;
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
            
           
            
            spriteBatch.DrawString(Font,
                _s,
                Position + PositionOffset + positionOffset + 
                (Parent?.Position ?? Vector2.Zero) + (Parent?.PositionOffset ?? Vector2.Zero),
                new Color(Color, Math.Min(alpha, 1f)),
                Rotation + (Parent?.Rotation ?? 0),
                origin,
                Scale * (Parent?.Scale ?? Vector2.One),
                Effects,
                0);
        }
    }
}