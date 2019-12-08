using Acryl.Graphics.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl.Graphics.Scenes
{
    public class StartupScene : Scene
    {
        public Text2D warningHeaderText;
        public Text2D warningBodyText;

        public bool startedFading;
        public StartupScene()
        {
            warningHeaderText = new Text2D("ALPHA WARNING!");
            warningBodyText = new Text2D("This is a fairly new Game! Expect that shit will work Better than this game!");

            warningBodyText.PositionOffset = new Vector2(0, 50);
            warningBodyText.Origin = Origin.Center;
            warningBodyText.PositionOrigin = Origin.Center;

            warningHeaderText.PositionOffset = new Vector2(0, 25);
            warningHeaderText.Origin = Origin.Center;
            warningHeaderText.PositionOrigin = Origin.Center;
        }

        protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!startedFading)
            {
                warningHeaderText.Alpha = 0;
                warningBodyText.Alpha = 0;

                startedFading = true;

                warningHeaderText.MoveTo(PositionOffset - new Vector2(0, 50f), 500);
                warningBodyText.MoveTo(PositionOffset - new Vector2(0, 50f), 500);
                
                warningHeaderText.FadeTo(1, 1000)
                    .OnFinish += x =>
                {
                    warningHeaderText.FadeTo(0, 2000);
                };
                warningBodyText.FadeTo(1, 1000)
                    .OnFinish += x =>
                {
                    warningBodyText.FadeTo(0, 2000).OnFinish += easing => AcrylGame.Game.SwitchScene(new GamePlayScene());
                };
            }
            
            spriteBatch.Begin(SpriteSortMode.Deferred,
                BlendState.NonPremultiplied);
            
            warningHeaderText.DrawFrame(spriteBatch, gameTime);
            warningBodyText.DrawFrame(spriteBatch, gameTime);
            
            spriteBatch.End();
        }

        public override void SwitchTo(Scene scene)
        {
            scene.Alpha = 0;
            scene.FadeTo(1, 500);
        }
    }
}