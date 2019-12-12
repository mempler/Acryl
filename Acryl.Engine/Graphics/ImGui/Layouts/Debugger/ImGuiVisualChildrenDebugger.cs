using System.Collections.Generic;
using Acryl.Engine.Graphics.Core;
using Acryl.Engine.Utility;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl.Engine.Graphics.ImGui.Layouts.Debugger
{
    using ImGui = ImGuiNET.ImGui;
    
    public class ImGuiVisualChildrenDebugger
    {
        private void DrawFromParent(GameBase game, ImGuiRenderer renderer, IChildrenContainer<Drawable> parent)
        {
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
                        child.TmpColor = Color.HotPink;
                        child.HasTmpAltered = true;
                    }
                    ImGui.BeginTooltip();

                    ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35.0f);
                    foreach (var prop in childProps)
                    {
                        if (prop.Name == "Children" || prop.Name == "Parent" || prop.Name == "Child" || prop.Name == "RootParent"
                            || prop.Name == "IsRootParent")
                            continue;

                        if (prop.PropertyType == typeof(Color))
                        {
                            var vCol = (Color) prop.GetValue(child);
                            var vec = ImGui.ColorConvertU32ToFloat4(vCol.PackedValue);
                            
                            ImGui.TextUnformatted($"{prop.Name}:"); ImGui.SameLine(150);
                            ImGui.ColorEdit4(prop.Name, ref vec, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.NoLabel);
                            continue;
                        }
                        
                        if (prop.PropertyType == typeof(Texture2D))
                        {
                            var vTex = (Texture2D) prop.GetValue(child);
                            var iTex = renderer.BindTexture(vTex); // Unbind and Rebinding them is slow, TODO: cache!
                            
                            ImGui.TextUnformatted($"{prop.Name}:"); ImGui.SameLine(150);
                            ImGui.Image(iTex, new System.Numerics.Vector2(300, 150), 
                                System.Numerics.Vector2.Zero, System.Numerics.Vector2.One,
                                System.Numerics.Vector4.One, System.Numerics.Vector4.One);
                            
                            //renderer.UnbindTexture(iTex);
                            continue;
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
                        DrawFromParent(game, renderer, child);
                    }
                }

                ImGui.TreePop();
            }
        }
        
        public void Draw(GameBase game, ImGuiRenderer renderer)
        {
            if (!ImGui.CollapsingHeader("Children"))
                return;
            
            lock (game.Children)
            {
                DrawFromParent(game, renderer, game);
            }
        }
    }
}