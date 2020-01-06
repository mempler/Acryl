using Acryl.Engine.Graphics.Core;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl.Engine.Graphics.ImGui
{
    public abstract class ImGuiWindow : Drawable
    {
        protected abstract internal ImGuiWindowFlags WindowFlags { get; }
        protected abstract internal string Name { get; }

        protected virtual void BeforeBegin(GameTime gameTime)
        {
            
        }
        
        protected abstract void Draw(GameTime gameTime);

        protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            BeforeBegin(gameTime);
            
            ImGuiNET.ImGui.Begin(Name, WindowFlags);

            Draw(gameTime);
            
            ImGuiNET.ImGui.End();
        }
        
    }
}