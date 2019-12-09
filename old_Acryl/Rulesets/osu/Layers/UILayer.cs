using Acryl.Graphics;
using Acryl.Graphics.Elements.Gameplay;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl.Rulesets.osu.Layers
{
    public class UiLayer : Layer
    {
        private SkipButton SkipButton = new SkipButton();

        public UiLayer()
        {
            SkipButton.PositionOrigin = Origin.Center;
            SkipButton.Origin = Origin.Center;
        }
        
        protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            SkipButton.DrawFrame(spriteBatch, gameTime);
            
            spriteBatch.End();
        }

        protected override void Update(GameTime gameTime)
        {
            SkipButton.UpdateFrame(gameTime);
        }
    }
}