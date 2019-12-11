using System.IO;
using Acryl.Engine;
using Acryl.Engine.Graphics.ImGui;
using Acryl.Engine.Graphics.ImGui.Layouts;
using Acryl.Engine.Stores;
using Acryl.Graphics;
using Acryl.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl
{
    public class AcrylGame : GameBase
    {
        [DependencyResolved]
        private GraphicsDevice Device { get; set; }
        
        private MonoImGui ImGui;

        public static string AcrylDirectory = Path.Combine(
            //Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.Personal)).FullName,
            ".acryl"
        );
        
        [LoadAsync]
        private void Load(TextureStore store)
        {
            ImGui = new MonoImGui();
            Add(ImGui); // Load dependencies
            ImGui.Add(new ImGuiDebugger());
            Remove(ImGui); // Remove from children.
            Dependencies.Add(ImGui);
            Dependencies.Add(new SkinManager());

#if !DEBUG
            Remove(ActiveScene);
            ActiveScene = new StartupScene();
            Add(ActiveScene);

            SwitchScene(new GameplayScene(), 2, 4);
#else
            Remove(ActiveScene);
            ActiveScene = new GameplayScene();
            Add(ActiveScene);
#endif

        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            
            SpriteBatch.Begin(
                SpriteSortMode.Deferred, 
                BlendState.AlphaBlend, 
                SamplerState.PointClamp,
                DepthStencilState.None, 
                RasterizerState.CullCounterClockwise, 
                null, 
                Matrix.Identity);
            
            ImGui?.UpdateFrame(gameTime); // Always draw ImGui on top of Everything!
            ImGui?.DrawFrame(SpriteBatch, gameTime);

            SpriteBatch.End();
        }
    }
}