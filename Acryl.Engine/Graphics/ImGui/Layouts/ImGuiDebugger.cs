using System;
using System.Diagnostics;
using System.Reflection;
using Acryl.Engine.Graphics.Core;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector3 = System.Numerics.Vector3;

namespace Acryl.Engine.Graphics.ImGui.Layouts
{
    using ImGui = ImGuiNET.ImGui;
    public class ImGuiDebugger : ImGuiLayout
    {
        [DependencyResolved]
        private GameBase Game { get; set; }
        
        protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (Game == null || !Visible)
                return;
            
            ImGui.Begin("Debug");
            
            Assembly assembly = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            var version = fvi.FileVersion;
            
            var winSize = ImGui.GetWindowSize();

            ImGui.Text($"Game Version: {assembly.GetName().Version}");
            ImGui.Text($"Game Engine Version: {assembly.GetName().Version}");
            
            ImGui.Text($"FPS: {Math.Round(ImGui.GetIO().Framerate, 2)}, {Math.Round(1000f / ImGui.GetIO().Framerate, 2)}");
            
            if (ImGui.CollapsingHeader("Children"))
            {
                lock (Game.Children)
                {
                    foreach (var child in Game.Children)
                    {
                        var childType = child.GetType();
                        if (ImGui.TreeNode(childType.Name))
                        {
                            ImGui.Indent();
                            var childProps = childType.GetProperties();

                            const float itemWidth = 200f;
                            ImGui.PushItemWidth(itemWidth);
                            var x = 0;
                            foreach (var prop in childProps)
                            {
                                var propStr = $"{x++} {prop.Name}: {prop.GetValue(child)}";

                                #region Color

                                if (prop.PropertyType == typeof(Color))
                                {
                                    var col = (Color) prop.GetValue(child);
                                    var vec = new Vector3(col.R / 255f, col.G / 255f, col.B / 255f);
                                    
                                    ImGui.Text(prop.Name);
                                    ImGui.SameLine(winSize.X - itemWidth - 5f, 0);
    
                                    ImGui.ColorEdit3(propStr, ref vec, ImGuiColorEditFlags.NoLabel);
                                    //ImGui.ColorPicker3(prop.Name, ref vec);
   
                                    var newCol = new Color(vec.X, vec.Y, vec.Z,  0);
                                    if (col != newCol)
                                        prop.SetValue(child, newCol);
                                }

                                #endregion

                                #region Vector2

                                else if (prop.PropertyType == typeof(Vector2))
                                {
                                    var vec = (Vector2) prop.GetValue(child);
                                    var imVec = new System.Numerics.Vector2(vec.X, vec.Y);
                                    
                                    ImGui.Text(prop.Name);
                                    ImGui.SameLine(winSize.X - itemWidth - 5f, 0);
                                    ImGui.DragFloat2(propStr, ref imVec);
                                    
                                    var newVec = new Vector2(imVec.X, imVec.Y);
                                    
                                    if (vec != newVec)
                                        prop.SetValue(child, newVec);
                                }

                                #endregion
                                
                                #region Origin
                                
                                else if (prop.PropertyType == typeof(Origin))
                                {
                                    var origin = (Origin) prop.GetValue(child);
                                    
                                    ImGui.Text(prop.Name);
                                    ImGui.SameLine(winSize.X - itemWidth - 5f, 0);


                                    var newOrigin = Origin.None;
                                    foreach (var val in Enum.GetValues(origin.GetType()))
                                    {
                                        if ((Origin) val == Origin.None)
                                            continue;
                                        
                                        var used = ((int) origin & (int) val) != 0;
                                        ImGui.Checkbox("CB_" + x * (int) val, ref used);
                                        if (used)
                                            newOrigin |= (Origin) val;
                                    }
                                    
                                    prop.SetValue(child, newOrigin);
                                }

                                #endregion

                                else
                                    ImGui.Text(propStr);
                            }
                            
                            ImGui.TreePop();
                        }
                    }
                }
            }

            ImGui.End();
        }
    }
}