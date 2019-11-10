using Acryl.Graphics;
using Acryl.Graphics.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl.Rulesets.Layers
{
    public class BackgroundLayer : Layer
    {
        private Sprite BackgroundImage => BeatmapManager.ActiveBeatmap.Background;
        
        public BackgroundLayer()
        {
            
        }
        
        protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            Beatmap.Background?.DrawFrame(spriteBatch, gameTime);
            if (Beatmap.Background != null)
                Beatmap.Background.Alpha = .4f;
            spriteBatch.End();
        }
    }
}