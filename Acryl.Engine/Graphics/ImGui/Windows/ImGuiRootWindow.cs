using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = System.Numerics.Vector2;

namespace Acryl.Engine.Graphics.ImGui.Windows
{
    using ImGui = ImGuiNET.ImGui;
    // A replacement for DockSpaceOverViewport
    public class ImGuiRootWindow : ImGuiWindow
    {
        protected internal override ImGuiWindowFlags WindowFlags
            => ImGuiWindowFlags.None       |
               ImGuiWindowFlags.NoResize   |
               ImGuiWindowFlags.NoMove     |
               ImGuiWindowFlags.NoTitleBar |
               ImGuiWindowFlags.DockNodeHost;
        
        protected internal override string Name => "Root";
        
        [DependencyResolved]
        private SpriteBatch Batch { get; set; }

        protected override void BeforeBegin(GameTime gameTime)
        {
            ImGui.SetNextWindowPos(new Vector2(0,0));
            ImGui.SetNextWindowSize(new Vector2(
                Batch.GraphicsDevice.PresentationParameters.BackBufferWidth,
                Batch.GraphicsDevice.PresentationParameters.BackBufferHeight));
        }

        protected override void Draw(GameTime gameTime)
        {
            var dockspaceID = ImGui.GetID("Im_DockSpace");
            
            if (ImGui.BeginMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Open..", "Ctrl+O")) { /* Do stuff */ }
                    if (ImGui.MenuItem("Save", "Ctrl+S"))   { /* Do stuff */ }
                    ImGui.EndMenu();
                }
                ImGui.EndMenuBar();
            }

            ImGui.DockSpace(dockspaceID, Vector2.Zero, ImGuiDockNodeFlags.None | ImGuiDockNodeFlags.PassthruCentralNode);
        }
    }
}