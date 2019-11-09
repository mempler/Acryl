using Acryl.Graphics.Elements;
using Acryl.Rulesets.osu.Layers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl.Rulesets.osu
{
    public class OsuRuleset : Ruleset
    {
        public override int Id => 0;
        public override Sprite Icon => null;
        public override string Name => "osu";

        public OsuRuleset()
        {
            Stack.Push(new PlayLayer());
            Stack.Push(new UiLayer());
        }
        
        protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            
        }
    }
}