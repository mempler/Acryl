using System;
using Acryl.Engine.Graphics.ImGui.Layouts.Debugger;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl.Engine.Graphics.ImGui.Layouts
{
    using ImGui = ImGuiNET.ImGui;
    public class ImGuiDebugger : ImGuiLayout
    {
        [DependencyResolved]
        private GameBase Game { get; set; }
        
        [DependencyResolved]
        private MonoImGui MonoImGui { get; set; }

        private ImGuiVisualChildrenDebugger ChildrenDebugger
            = new ImGuiVisualChildrenDebugger();
        
        protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (Game == null || !Visible)
                return;
            
            ImGui.Begin("Debug");

            ImGui.Text($"FPS: {Math.Round(ImGui.GetIO().Framerate, 2)}, {Math.Round(1000f / ImGui.GetIO().Framerate, 2)}");
            
            ChildrenDebugger.Draw(Game, MonoImGui.Renderer);
            
            ImGui.End();
        }
    }
}