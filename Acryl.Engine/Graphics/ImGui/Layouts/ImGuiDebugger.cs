using Acryl.Engine.Graphics.Core;
using Acryl.Engine.Graphics.ImGui.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl.Engine.Graphics.ImGui.Layouts
{
    public class ImGuiDebugger : ImGuiLayout
    {
        [DependencyResolved]
        private GameBase Game { get; set; }
        
        [DependencyResolved]
        private MonoImGui MonoImGui { get; set; }

        private ImGuiChildrenVisualizer ChildrenVisualizer
            = new ImGuiChildrenVisualizer();
        
        private ImGuiRootWindow RootWindow
            = new ImGuiRootWindow();
        
        private ImGuiGame GameWindow
            = new ImGuiGame();
        
        private ImGuiConsole Console
            = new ImGuiConsole();
        
        private ImGuiGameInformation GameInformation
            = new ImGuiGameInformation();

        [LoadAsync]
        private void Load()
        {
            Add(RootWindow);
            Add(ChildrenVisualizer);
            Add(GameWindow);
            Add(Console);
            Add(GameInformation);
        }
    }
}