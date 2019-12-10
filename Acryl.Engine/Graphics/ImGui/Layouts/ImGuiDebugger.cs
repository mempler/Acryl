using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl.Engine.Graphics.ImGui.Layouts
{
    using ImGui = ImGuiNET.ImGui;
    public class ImGuiDebugger : ImGuiLayout
    {
        protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            ImGui.Begin("Debug");
            
            ImGui.Text($"FPS: {Math.Round(ImGui.GetIO().Framerate, 2)}, {Math.Round(1000f / ImGui.GetIO().Framerate, 2)}");
            
            ImGui.End();
        }
    }
}