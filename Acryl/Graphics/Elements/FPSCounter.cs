using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl.Graphics.Elements
{
    public class FpsCounter
    {
        public double TotalFrames { get; private set; }
        public double TotalSeconds { get; private set; }
        public double AverageFramesPerSecond { get; private set; }
        public double CurrentFramesPerSecond { get; private set; }

        public const int MAXIMUM_SAMPLES = 100;

        private readonly Queue<double> _sampleBuffer = new Queue<double>();

        public void Callback(double delta)
        {
            CurrentFramesPerSecond = 1.0d / delta;
            
            AverageFramesPerSecond = CurrentFramesPerSecond / TotalFrames;

            TotalFrames++;
            TotalSeconds += delta;
        }
    }    
    
    public class FpsCounterDisplay : Drawable
    {
        private readonly FpsCounter _drawFrames = new FpsCounter();
        private readonly FpsCounter _updateFrames = new FpsCounter();

        private string _cachedStringD = string.Empty;
        private string _cachedStringU = string.Empty;

        private double _;
        protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _drawFrames.Callback(gameTime.ElapsedGameTime.TotalMilliseconds);
            
            _ += gameTime.ElapsedGameTime.TotalSeconds;
            if (_ >= .5f)
            {
                _cachedStringD = "FPS: " + Math.Round(_drawFrames.CurrentFramesPerSecond * 1000, 2);
                _cachedStringU = "UPS: " + Math.Round(_updateFrames.CurrentFramesPerSecond * 1000, 2);

                _ = 0;
            }

            spriteBatch.DrawString(AcrylGame.DefaultFont,
                _cachedStringD,
                Vector2.Zero, Color.Aqua);
            
            spriteBatch.DrawString(AcrylGame.DefaultFont,
                _cachedStringU,
                new Vector2(0, 20), Color.Aqua);
        }

        protected override void Update(GameTime gameTime)
        {
            _updateFrames.Callback(gameTime.ElapsedGameTime.TotalMilliseconds);
        }
    }
}