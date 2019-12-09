using System;
using Acryl.Engine;
using Acryl.Engine.Graphics.Core;
using Acryl.Engine.Graphics.MonoGame.ImGui;
using Acryl.Engine.Graphics.MonoGame.ImGui.Layouts;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl
{
    public class AcrylGame : GameBase
    {
        [DependencyResolved]
        private GraphicsDevice Device { get; set; }

        private MonoImGui ImGui;
        private Text2D Text;
        
        [LoadAsync]
        private async void Load(GraphicsDevice device)
        {
            Text = new Text2D("Hello World!") {Size = 16};
            Add(Text);
            
            ImGui = new MonoImGui();
            Add(ImGui);
            
            ImGui.Add(new ImGuiDebugger());
        }
    }
}