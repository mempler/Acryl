using System;
using System.IO;
using System.Linq;
using Acryl.Extension.Discord;
using Acryl.Graphics.Elements;
using Acryl.Graphics.Elements.Gameplay;
using Acryl.Rulesets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl.Graphics.Scenes
{
    public class GamePlayScene : Scene
    {
        public Beatmap Beatmap => BeatmapManager.ActiveBeatmap;
        private FpsCounterDisplay fpsCounter = new FpsCounterDisplay();
        private SkipButton SkipButton = new SkipButton();
        
        private const double Start = 0; // Skip 30 seconds

        // we use our own Begin and End function!
        public override void Begin(SpriteBatch batch)
        {
            //base.Begin(batch);
        }
        public override void End(SpriteBatch batch)
        {
            //base.End(batch);
        }

        public GamePlayScene()
        {
            BeatmapManager.ActiveBeatmap = Beatmap.ReadBeatmap(
                    Path.Combine(AcrylGame.AcrylDirectory,
                        "BeatMaps/0/0.osu"
                    )
                );

            foreach (var obj in Beatmap.HitObjects)
            {
                obj.Parent = this;
            }
            
            AcrylGame.Discord.GetActivityManager()
                   .UpdateActivity(new Activity
                   {
                       State = "Playing BeatMap 0",
                       Assets = new ActivityAssets
                       {
                           LargeText = "Acryl",
                           LargeImage = "acryllogo_small"
                       },
                       Timestamps = new ActivityTimestamps
                       {
                           End = (int) Math.Round(
                                            DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds +
                                            Beatmap.Last.Timing / 1000f
                                        )
                       }
                   }, result =>
                   {
                       Console.WriteLine("Code: {0}", result);
                   });
        }
        
        protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            var sliders = Beatmap.HitObjects.Where(obj => obj.Kind == HitObjectKind.Slider).ToList();
            var other = Beatmap.HitObjects.Where(obj => obj.Kind != HitObjectKind.Slider).ToList();
            
            // Sliders are special, they need to be drawn differently.
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            // We render them backward to fix some Z Axis issues.
            for (var i = sliders.Count - 1; i > 0; i--) {
                sliders[i].DrawFrame(spriteBatch, gameTime);
            }
            spriteBatch.End();
            
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            Beatmap.Background?.DrawFrame(spriteBatch, gameTime);
            if (Beatmap.Background != null)
                Beatmap.Background.Alpha = .4f;

            // We render them backward to fix some Z Axis issues.
            for (var i = other.Count - 1; i > 0; i--) {
                other[i].DrawFrame(spriteBatch, gameTime);
            }
            
            SkipButton.DrawFrame(spriteBatch, gameTime);
            fpsCounter.DrawFrame(spriteBatch, gameTime);
            spriteBatch.End();
        }
        
        protected override void Update(GameTime gameTime)
        {
            if (!Beatmap.Song.IsPlaying) {
                Beatmap.Song.Volume = .1f;
                Beatmap.Song.Play();
            }

            if (!Beatmap.FreezeBeatmap)
                Beatmap.CurrentElapsed += gameTime.ElapsedGameTime.TotalMilliseconds;
            
            Beatmap.CurrentTimingPoint = Beatmap.TimingPoints.LastOrDefault(x => x.Offset > Beatmap.CurrentElapsed);

            if (Beatmap.CurrentElapsed > Beatmap.Song.Position + 100f ||
                Beatmap.CurrentElapsed < Beatmap.Song.Position - 100f)
                Beatmap.Song.Position = Beatmap.CurrentElapsed;
            
            for (var i = Beatmap.HitObjects.Count - 1; i > 0; i--) {
                Beatmap.HitObjects[i].UpdateFrame(gameTime);
            }
            
            fpsCounter.UpdateFrame(gameTime);
            SkipButton.UpdateFrame(gameTime);
        }
    }
}