using System.Linq;
using Acryl.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl.Rulesets.osu.Layers
{
    // Hit Circle Playfield
    public class CircleLayer : Layer
    {
        public CircleLayer()
        {

        }

        protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            var hitCircles = Beatmap.HitObjects
                                    .Where(obj => obj.Kind == HitObjectKind.Circle)
                                    .ToList();
            
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            // We render them backward to fix some Z Axis issues.
            for (var i = hitCircles.Count - 1; i > 0; i--) {
                hitCircles[i].DrawFrame(spriteBatch, gameTime);
            }
            spriteBatch.End();
        }

        protected override void Update(GameTime gameTime)
        {
            var objects = Beatmap.HitObjects
                                 .Where(obj => Beatmap.CurrentElapsed - 5000f < obj.Timing &&
                                               obj.Kind == HitObjectKind.Circle)
                                 .ToList();
  
            for (var i = objects.Count - 1; i > 0; i--) {
                objects[i].UpdateFrame(gameTime);
            }
        }
    }
}