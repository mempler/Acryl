using System;
using Acryl.Engine.Graphics.Core;
using Acryl.Engine.Graphics.ImGui.Layouts.Debugger;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = System.Numerics.Vector2;

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
        
        private ImGuiRootWindow RootWindow
            = new ImGuiRootWindow();
        
        private ImGuiGameWindow GameWindow
            = new ImGuiGameWindow();

        [LoadAsync]
        private void Load()
        {
            AsyncLoadingPipeline.LoadForObject(RootWindow, this);
            AsyncLoadingPipeline.LoadForObject<Drawable>(RootWindow, this);
            AsyncLoadingPipeline.LoadForObject(GameWindow, this);
            AsyncLoadingPipeline.LoadForObject<Drawable>(GameWindow, this);
        }
        
        protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (Game == null || !Visible)
                return;
            
            RootWindow.DrawFrame(spriteBatch, gameTime);
            ChildrenDebugger.Draw(Game, MonoImGui.Renderer);
            GameWindow.DrawFrame(spriteBatch, gameTime);
            
            //ImGui.DockSpaceOverViewport(null, ImGuiDockNodeFlags.None, null);
            //var childDebuggerId = ImGui.GetID("Children Debugger");
            //ImGui.Begin("Debug", ImGuiWindowFlags.MenuBar);
            //ImGui.DockSpace(childDebuggerId, Vector2.Zero, ImGuiDockNodeFlags.None | ImGuiDockNodeFlags.PassthruCentralNode);

            //ImGui.Text($"FPS: {Math.Round(ImGui.GetIO().Framerate, 2)}, {Math.Round(1000f / ImGui.GetIO().Framerate, 2)}");

            //ChildrenDebugger.Draw(Game, MonoImGui.Renderer);

            //ImGui.End();
        }
    }
}