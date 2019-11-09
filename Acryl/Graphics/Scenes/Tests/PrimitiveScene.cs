using Acryl.Graphics;
using Acryl.Graphics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl.Scenes.Tests
{
    public class PrimitiveScene : Scene
    {
        private Box _box;
        private int _i = 0;
        
        public PrimitiveScene()
        {
            Add( _box = new Box(100, 100, Color.White) );
            _box.Origin = Origin.Center;
           // _box.Position += Vector2.One * 100f;
        }
        
        protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (_i > 1000) {
                _box.Rotation++;
                _i = 0;
            }
            _i++;
        }

        protected override void Update(GameTime gameTime)
        {
            
        }
    }
}