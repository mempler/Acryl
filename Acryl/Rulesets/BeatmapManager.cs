using System;
using Microsoft.Xna.Framework;

namespace Acryl.Rulesets
{
    public class BeatmapManager
    {
        public static Beatmap ActiveBeatmap;

        // Convert Osu!'s virtual field to our Resolution
        public static Vector2 MapPosition(Vector2 virtualField)
        {
            var screenWidth = AcrylGame.Game.GraphicsDevice.Viewport.Width;
            var screenHeight = AcrylGame.Game.GraphicsDevice.Viewport.Height;
            
            var adjustedX = MathF.Round(screenWidth / 1.333333f); // Aspect Ratio
            
            var paddingX = adjustedX / 8;
            var paddingY = screenHeight / 8;

            var newValueX = virtualField.X * (adjustedX - paddingX - paddingX) / 512f + paddingX + (screenWidth - adjustedX) / 2f;
            var newValueY = virtualField.Y * screenHeight / 512f + paddingY;
            return new Vector2(newValueX, newValueY);
        }
    }
}