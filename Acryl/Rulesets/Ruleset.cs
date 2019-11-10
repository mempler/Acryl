using Acryl.Graphics;
using Acryl.Graphics.Elements;
using Acryl.Rulesets.Layers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl.Rulesets
{
    public abstract class Ruleset : Drawable
    {
        public abstract int Id { get; }
        public abstract Sprite Icon { get; }
        public abstract string Name { get; }

        public ILayerStack Stack { get; }

        public Ruleset()
        {
            Stack = new LayerStack();
            
            Stack.Push(new BackgroundLayer());
        }

        protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Stack.Draw(spriteBatch, gameTime);
        }
        
        protected override void Update(GameTime gameTime)
        {
            Stack.Update(gameTime);
        }
    }
}