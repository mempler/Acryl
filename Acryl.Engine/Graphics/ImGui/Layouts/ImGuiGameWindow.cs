using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = System.Numerics.Vector2;

namespace Acryl.Engine.Graphics.ImGui.Layouts
{
    public class ImGuiGameWindow : ImGuiLayout
    {
        [DependencyResolved]
        private MonoImGui MonoImGui { get; set; }
        
        [DependencyResolved]
        private PostProcessor PostProcessor { get; set; }
        
        protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (MonoImGui == null)
                return;
            
            ImGuiNET.ImGui.Begin("Game", ImGuiWindowFlags.HorizontalScrollbar);
            var wndSize = ImGuiNET.ImGui.GetWindowSize();

            Size = wndSize;
            var gSize = new Vector2(Field.Width, Field.Height);
            
            ImGuiNET.ImGui.Image(MonoImGui.PostProcessingId,
                gSize,
                Vector2.Zero,
                Vector2.One);
            
            ImGuiNET.ImGui.End();
        }
    }
}