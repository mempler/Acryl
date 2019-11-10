using System.Linq;
using Acryl.Graphics;
using Acryl.Rulesets.osu.HitObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl.Rulesets.osu.Layers
{
    public class SliderLayer : Layer
    {
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
                    DepthFormat.None,
                    32,
                    RenderTargetUsage.DiscardContents);
            
            var sliders = Beatmap.HitObjects.Where(obj => obj.Kind == HitObjectKind.Slider).ToList();
            
            AcrylGame.Game.GraphicsDevice.SetRenderTarget(_sliderTarget);
            AcrylGame.Game.GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            for (var i = sliders.Count - 1; i > 0; i--) {
                sliders[i].DrawFrame(spriteBatch, gameTime);
            }
            spriteBatch.End();
            AcrylGame.Game.GraphicsDevice.SetRenderTarget(null);
            
            HitSlider.BorderShader.Parameters["BorderColor"].SetValue(Color.White.ToVector4());
            HitSlider.BorderShader.Parameters["BorderWidth"].SetValue(.003f);
            
            spriteBatch.Begin(SpriteSortMode.Immediate,
                BlendState.NonPremultiplied,
                effect: HitSlider.BorderShader,
                samplerState: SamplerState.AnisotropicWrap);
            spriteBatch.Draw(_sliderTarget, new Rectangle(0, 0, 1280, 720), Color.White);
            spriteBatch.End();
        }
        
        protected override void Update(GameTime gameTime)
        {
            var objects = Beatmap.HitObjects
                                 .Where(obj => Beatmap.CurrentElapsed - 5000f < obj.Timing &&
                                               obj.Kind == HitObjectKind.Slider)
                                 .ToList();
  
            for (var i = objects.Count - 1; i > 0; i--) {
                objects[i].UpdateFrame(gameTime);
            }
        }
    }
}