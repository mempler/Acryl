using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = System.Numerics.Vector2;

namespace Acryl.Engine.Graphics.ImGui.Windows
{
    public class ImGuiGame : ImGuiWindow
    {
        [DependencyResolved]
        private MonoImGui MonoImGui { get; set; }
        
        [DependencyResolved]
        private PostProcessor PostProcessor { get; set; }

        private readonly VirtualField _nf = new VirtualField(0,0);

        protected internal override ImGuiWindowFlags WindowFlags => ImGuiWindowFlags.HorizontalScrollbar;
        protected internal override string Name => "Game";

        protected override void Draw(GameTime gameTime)
        {
            if (MonoImGui == null || PostProcessor?.RenderTexture == null)
                return;
            
            var wndSize = ImGuiNET.ImGui.GetWindowSize();

            Size = wndSize;
            var gSize = new Vector2(Field.Width, Field.Height);

            _nf.Width = wndSize.X;
            _nf.Height = wndSize.Y;
            
            var scl = _nf.ApplyScale(PostProcessor.RenderTexture) * .9f;
            var iSize = gSize * scl;
            
            ImGuiNET.ImGui.SetCursorPos((wndSize - iSize) * 0.5f);
            ImGuiNET.ImGui.Image(MonoImGui.PostProcessingId,
                iSize,
                Vector2.Zero,
                Vector2.One);
        }
    }
}