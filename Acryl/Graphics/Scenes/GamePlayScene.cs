using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Acryl.Extension.Discord;
using Acryl.Graphics.Elements;
using Acryl.Rulesets;
using Acryl.Rulesets.osu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl.Graphics.Scenes
{
    public class GamePlayScene : Scene
    {
        private static Beatmap Beatmap => BeatmapManager.ActiveBeatmap;
        private FpsCounterDisplay fpsCounter = new FpsCounterDisplay();

        public static List<Ruleset> Rulesets = new List<Ruleset>
        {
            new OsuRuleset()
        };

        private const double Start = 0; // Skip 30 seconds

        public GamePlayScene()
        {
            BeatmapManager.ActiveBeatmap = Beatmap.ReadBeatmap(
                    Path.Combine(AcrylGame.AcrylDirectory,
                        "BeatMaps/0/0.osu"
                    )
                );
            
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
            Rulesets[Beatmap.RulesetId].DrawFrame(spriteBatch, gameTime);

            // Draw everything else ontop of the Ruleset Layer.
            spriteBatch.Begin(SpriteSortMode.Deferred,
                BlendState.NonPremultiplied);

            fpsCounter.DrawFrame(spriteBatch, gameTime);
            
            spriteBatch.End();
        }
        
        protected override void Update(GameTime gameTime)
        {
            if (!Beatmap.Song.IsPlaying) {
                Beatmap.Song.Volume = .1f;
                Beatmap.Song.Play();
            }
            
            Beatmap.CurrentElapsed += gameTime.ElapsedGameTime.TotalMilliseconds;
            
            Beatmap.CurrentTimingPoint = Beatmap.TimingPoints.LastOrDefault(x => x.Offset > Beatmap.CurrentElapsed);

            var songPosition = Beatmap.Song.Position;
            // Audio Resynchronization if we're going too far away. > cause by Lag.
            if (Beatmap.CurrentElapsed > songPosition + 100f ||
                Beatmap.CurrentElapsed < songPosition - 100f)
                Beatmap.Song.Position = Beatmap.CurrentElapsed;

            Rulesets[Beatmap.RulesetId].UpdateFrame(gameTime);
            
            fpsCounter.UpdateFrame(gameTime);
        }
    }
}