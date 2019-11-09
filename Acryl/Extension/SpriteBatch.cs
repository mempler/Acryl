using System;
using Acryl.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl.Extension
{
    public static class SpriteBatchExtension
    {
        private static Texture2D _texture;
        private static Texture2D EmptyTexture2D(this SpriteBatch spriteBatch)
        {
            if (_texture != null)
                return _texture;
            
            _texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _texture.SetData(new[] {Color.White});

            return _texture;
        }
        
        public static void DrawLine(this SpriteBatch spriteBatch, Line line, Color color, float thickness = 1f)
        {
            var distance = Vector2.Distance(line.Begin, line.End);
            var angle = (float)Math.Atan2(line.End.Y - line.Begin.Y, line.End.X - line.Begin.X);
            DrawLine(spriteBatch, line.Begin, distance, angle, color, thickness);
        }
        
        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float thickness = 5f)
        {
            var origin = new Vector2(0f, 0.5f);
            var scale = new Vector2(length, thickness);
            spriteBatch.Draw(spriteBatch.EmptyTexture2D(), point, null, color, angle, origin, scale, SpriteEffects.None, 0);
        }

        public static void DrawBox(this SpriteBatch spriteBatch, Rectangle rectangle, Color color, float angle, Vector2 origin)
        {
            var position = new Vector2(rectangle.X, rectangle.Y);
            var scale = new Vector2(rectangle.Width, rectangle.Height);
            
            spriteBatch.Draw(spriteBatch.EmptyTexture2D(), position, null, color, angle, origin, scale, SpriteEffects.None, 0);
        }
    }
}