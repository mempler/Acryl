using Acryl.Graphics.Skin;
using Acryl.Rulesets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Acryl.Graphics.Elements.Gameplay
{
    public class SkipButton : Drawable
    {
        private SpriteAnimation SkipAnimation { get; }
        private bool hasBeenClicked = false;

        public SkipButton()
        {
            if (SkipAnimation == null)
                SkipAnimation = SpriteAnimation.FromImageArray("Skip");

            Add(SkipAnimation);
        }

        private bool IsFading;
        protected override void Update(GameTime gameTime)
        {
            if (BeatmapManager.ActiveBeatmap.CurrentElapsed >=
                BeatmapManager.ActiveBeatmap.First.Timing - 2000f)
            {
                //Remove(SkipAnimation);
                if (IsFading)
                    return;
                
                SkipAnimation.FadeTo(0, 500);
                IsFading = true;
                return;
            }

            if (!Visible)
                return;
            
            if (!Keyboard.GetState().IsKeyDown(Keys.Space))
                return;
            
            if (hasBeenClicked)
                return;

            BeatmapManager.ActiveBeatmap.CurrentElapsed = BeatmapManager.ActiveBeatmap.First.Timing - 2000f;
            BeatmapManager.ActiveBeatmap.Song.Position = BeatmapManager.ActiveBeatmap.First.Timing - 2000f;

            hasBeenClicked = true;
        }
    }
}