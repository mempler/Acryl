using System.Collections.Generic;
using Acryl.Engine.Graphics.Core;
using Acryl.Engine.Utility;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = System.Drawing.Color;

namespace Acryl.Engine.Graphics.ImGui.Windows
{
    using ImGui = ImGuiNET.ImGui;
    
    public class ImGuiChildrenVisualizer : ImGuiWindow
    {
        public void Draw(GameBase game, ImGuiRenderer renderer)
        {
            ImGui.Begin("");

            ImGui.End();
        }

        protected internal override ImGuiWindowFlags WindowFlags => ImGuiWindowFlags.None;
        protected internal override string Name => "Children Debugger";
        
        [DependencyResolved]
        private MonoImGui MonoImGui { get; set; }
                
        [DependencyResolved]
        private GameBase Game { get; set; }
        
        private void DrawFromParent(IChildrenContainer<Drawable> parent)
        {
            if (Game == null || MonoImGui == null)
                return;
            
            var childContainerListType = typeof(List<Drawable>);
            
            foreach (var child in parent.Children)
            {
                var childType = child.GetType();
                var childProps = childType.GetProperties();

                if (!ImGui.TreeNode(childType.Name))
                    continue;
                
                if (ImGui.IsItemHovered())
                {
                    if (!child.HasTmpAltered) {
                        child.TmpColor = Color.GreenYellow;
                        child.HasTmpAltered = true;
                    }
                    ImGui.BeginTooltip();

                    ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35.0f);
                    foreach (var prop in childProps)
                    {
                        if (prop.Name == "Children" || prop.Name == "Parent" || prop.Name == "Child" || prop.Name == "RootParent"
                            || prop.Name == "IsRootParent" || prop.Name == "ImTex" )
                            continue;

                        if (prop.PropertyType == typeof(Color))
                        {
                            var vCol = (Color) prop.GetValue(child);
                            var vec = ImGui.ColorConvertU32ToFloat4((uint) vCol.ToArgb());
                            
                            ImGui.TextUnformatted($"{prop.Name}:"); ImGui.SameLine(150);
                            ImGui.ColorEdit4(prop.Name, ref vec, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.NoLabel);
                            continue;
                        }
                        
                        if (prop.PropertyType == typeof(Texture2D))
                        {
                            var vTex = (Texture2D) prop.GetValue(child);

                            if ((int) child.ImTex <= 0)
                                child.ImTex = MonoImGui.Renderer.BindTexture(vTex);
                            
                            ImGui.TextUnformatted($"{prop.Name}:"); ImGui.SameLine(150);
                            ImGui.Image(child.ImTex, new System.Numerics.Vector2(300, 150), 
                                System.Numerics.Vector2.Zero, System.Numerics.Vector2.One,
                                System.Numerics.Vector4.One, System.Numerics.Vector4.One);
                            
                            //renderer.UnbindTexture(iTex);
                            continue;
                        }

                        if (prop.PropertyType == typeof(RenderTarget2D))
                        {
                            var vTarget = (RenderTarget2D) prop.GetValue(child);

                             if ((int) child.ImTex <= 0)
                                child.ImTex = MonoImGui.Renderer.BindTexture(vTarget);

                            ImGui.TextUnformatted($"{prop.Name}:"); ImGui.SameLine(150);
                            ImGui.Image(child.ImTex, new System.Numerics.Vector2(300, 150), 
                                System.Numerics.Vector2.Zero, System.Numerics.Vector2.One,
                                System.Numerics.Vector4.One, System.Numerics.Vector4.One);
                        }
                        
                        ImGui.TextUnformatted($"{prop.Name}:"); ImGui.SameLine(150);
                        ImGui.TextUnformatted($"{prop.GetValue(child)}");
                    }
                    ImGui.PopTextWrapPos();
                    
                    ImGui.EndTooltip();
                }
                else
                {
                    child.TmpColor = Color.White;
                    child.HasTmpAltered = false;
                }

                foreach (var prop in childProps)
                { 
                    if (prop.Name != "Children")
                        continue;
                    
                    if (childContainerListType.IsAssignableFrom(prop.PropertyType))
                    {
                        DrawFromParent(child);
                    }
                }

                ImGui.TreePop();
            }
        }
        
        protected override void Draw(GameTime gameTime)
        {
            lock (Game.Children)
                DrawFromParent(Game);
        }
    }
}