using Acryl.Extension;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Acryl.Graphics.Origin;

namespace Acryl.Graphics.Primitives
{
    public class Box : Drawable
    {
        public float Width { get; set; }
        public float Height { get; set; }
        
        private static GraphicsDevice GraphicsDevice => AcrylGame.Game.GraphicsDevice;
        private static GraphicsDeviceManager GraphicsDeviceManager => AcrylGame.Game.GraphicsDeviceManager;

        public Box(int width, int height, Color color)
        {
            Width = width;
            Height = height;
            Color = color;
        }

        protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            var origin = new Vector2();

            if ((Origin & Top) != 0)
                origin.Y = 0;
            if ((Origin & Bottom) != 0)
                origin.Y = Height;
            
            if ((Origin & Left) != 0)
                origin.Y = 0;
            if ((Origin & Right) != 0)
                origin.X = Width;
            
            if ((Origin & Center) != 0) {
                origin.X = Width / 2f;
                origin.Y = Height / 2f;
                
                if ((Origin & Top) != 0)
                    origin.Y = 0;
                
                else if ((Origin & Bottom) != 0)
                    origin.Y = Height;
            
                if ((Origin & Left) != 0)
                    origin.Y = 0;
                else if ((Origin & Right) != 0)
                    origin.X = Width / 2f;
            }
            
            spriteBatch.DrawBox(new Rectangle((int) Position.X, (int) Position.Y,(int) Width, (int) Height),
                Color, Rotation / 360f, origin);
        }

        protected override void Update(GameTime gameTime)
        {
            
        }
    }
}