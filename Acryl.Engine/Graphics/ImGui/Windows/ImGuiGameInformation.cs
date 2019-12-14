using System;
using ImGuiNET;
using Microsoft.Xna.Framework;

namespace Acryl.Engine.Graphics.ImGui.Windows
{
    public class ImGuiGameInformation : ImGuiWindow
    {
        protected internal override ImGuiWindowFlags WindowFlags => ImGuiWindowFlags.None;
        protected internal override string Name => "Game Information";

        protected override void Draw(GameTime gameTime)
        {
            ImGuiNET.ImGui.Text($"FPS: {Math.Round(ImGuiNET.ImGui.GetIO().Framerate, 2)}, {Math.Round(1000f / ImGuiNET.ImGui.GetIO().Framerate, 2)}");
        }
    }
}