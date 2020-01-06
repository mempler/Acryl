using System.IO;
using Acryl.Engine;
using Acryl.Engine.Graphics.ImGui;
using Acryl.Engine.Graphics.ImGui.Layouts;
using Acryl.Engine.Stores;
using Acryl.Graphics;
using Acryl.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Acryl
{
    public class AcrylGame : GameBase
    {
        private ImGuiDebugger _debugger = new ImGuiDebugger();
        [DependencyResolved]
        private GraphicsDevice Device { get; set; }
        
        public static string AcrylDirectory = Path.Combine(
            //Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.Personal)).FullName,
            ".acryl"
        );
        
        [LoadAsync]
        private void Load(TextureStore store)
        {
            // Attach Debugger
            ImGui.Add(new ImGuiDebugger());
            //Remove(ImGui); // Remove from children.
            
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

        private bool showDebug;
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