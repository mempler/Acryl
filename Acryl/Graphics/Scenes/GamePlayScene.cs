using System;
using System.IO;
using System.Linq;
using Acryl.Extension.Discord;
using Acryl.Graphics.Elements;
using Acryl.Graphics.Elements.Gameplay;
using Acryl.Rulesets;
using Acryl.Rulesets.osu.HitObjects;
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
        
        private RenderTarget2D _sliderTarget;
        
        protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (_sliderTarget == null)
                _sliderTarget = new RenderTarget2D(
                    AcrylGame.Game.GraphicsDevice,
                    AcrylGame.Game.GraphicsDevice.PresentationParameters.BackBufferWidth,
                    AcrylGame.Game.GraphicsDevice.PresentationParameters.BackBufferHeight,
                    false,
                    AcrylGame.Game.GraphicsDevice.PresentationParameters.BackBufferFormat,
                    DepthFormat.Depth24);
            var sliders = Beatmap.HitObjects.Where(obj => obj.Kind == HitObjectKind.Slider).ToList();
            var other = Beatmap.HitObjects.Where(obj => obj.Kind != HitObjectKind.Slider).ToList();
            
            AcrylGame.Game.GraphicsDevice.SetRenderTarget(_sliderTarget);
            AcrylGame.Game.GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            for (var i = sliders.Count - 1; i > 0; i--) {
                sliders[i].DrawFrame(spriteBatch, gameTime);
            }
            spriteBatch.End();
            
            AcrylGame.Game.GraphicsDevice.SetRenderTarget(null);
            
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            Beatmap.Background?.DrawFrame(spriteBatch, gameTime);
            if (Beatmap.Background != null)
                Beatmap.Background.Alpha = .4f;
            spriteBatch.End();
            
            HitSlider.BorderShader.Parameters["BorderColor"].SetValue(Color.White.ToVector4());
            HitSlider.BorderShader.Parameters["BorderWidth"].SetValue(.005f);
            
            spriteBatch.Begin(SpriteSortMode.Deferred,
                BlendState.NonPremultiplied,
                effect: HitSlider.BorderShader,
                samplerState: SamplerState.AnisotropicWrap);
            spriteBatch.Draw(_sliderTarget, new Rectangle(0, 0, 1280, 720), Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
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

            var objects = Beatmap.HitObjects.Where(obj =>
                                     Beatmap.CurrentElapsed - 5000f < obj.Timing)
                                 .ToList();
  
            for (var i = objects.Count - 1; i > 0; i--) {
                objects[i].UpdateFrame(gameTime);
            }
            
            fpsCounter.UpdateFrame(gameTime);
            SkipButton.UpdateFrame(gameTime);
        }
    }
}