using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = System.Numerics.Vector2;

namespace Acryl.Engine.Graphics.ImGui.Layouts
{
    using ImGui = ImGuiNET.ImGui;
    // A replacement for DockSpaceOverViewport
    public class ImGuiRootWindow : ImGuiLayout
    {
        protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            var flags = ImGuiWindowFlags.None;
            flags |= ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.DockNodeHost;
            
            ImGui.SetNextWindowPos(new Vector2(0,0));
            ImGui.SetNextWindowSize(new Vector2(
                spriteBatch.GraphicsDevice.PresentationParameters.BackBufferWidth,
                spriteBatch.GraphicsDevice.PresentationParameters.BackBufferHeight));


            var dockspaceID = ImGui.GetID("Im_DockSpace");
            if (ImGui.Begin("Root Window", flags))
            {
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
                ImGui.End();
            }
    
        }
    }
}