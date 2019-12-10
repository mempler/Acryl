using System;
using Acryl.Engine;
using Acryl.Engine.Graphics;
using Acryl.Engine.Graphics.ImGui;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl.ImGuiLayouts
{
    public class CachedRenderTargetLayout : ImGuiLayout
    {
        private readonly CachedRenderTarget _target;

        public CachedRenderTargetLayout(CachedRenderTarget target)
        {
            _target = target;
        }
        
        protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            ImGui.Begin("CachedRenderTarget");

            var enableBlur = _target.Blur;
            ImGui.Checkbox("Enable Blur", ref enableBlur);
            if (enableBlur != _target.Blur)
                _target.Blur = enableBlur;
            
            var blurStrength = _target.BlurStrength;
            ImGui.SliderFloat("Blur Amount", ref blurStrength, 2, 10);
            if (blurStrength != _target.BlurStrength)
                _target.BlurStrength = blurStrength;
            
            ImGui.Text($"FPS: {Math.Round(ImGui.GetIO().Framerate, 2)}, {Math.Round(1000f / ImGui.GetIO().Framerate, 2)}");
            
            ImGui.End();
        }
    }
}