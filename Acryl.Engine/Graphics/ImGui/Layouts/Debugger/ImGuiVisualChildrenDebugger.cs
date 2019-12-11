using System;
using System.Collections.Generic;
using Acryl.Engine.Graphics.Core;
using Acryl.Engine.Utility;

namespace Acryl.Engine.Graphics.ImGui.Layouts.Debugger
{
    using ImGui = ImGuiNET.ImGui;
    
    public class ImGuiVisualChildrenDebugger
    {

        private void DrawFromParent(IChildrenContainer<Drawable> parent, int xz = 0)
        {
            var childContainerType = typeof(IChildrenContainer<Drawable>);
            var childContainerListType = typeof(List<IChildrenContainer<Drawable>>);
            
            foreach (var child in parent.Children)
            {
                var childType = child.GetType();
                if (!ImGui.TreeNode(childType.Name))
                    continue;
                    
                ImGui.Indent();
                var childProps = childType.GetProperties();

                const float itemWidth = 200f;
                //ImGui.PushItemWidth(itemWidth);
                    
                ImGui.Columns(2, xz.ToString(), false);
                var x = 0;
                foreach (var prop in childProps)
                {
                    var propStr = $"{x++ + xz} {prop.Name}: {prop.GetValue(child)}";

                    if (prop.Name == "Child")
                        continue;
                    
                    if (childContainerType.IsAssignableFrom(prop.PropertyType))
                    {
                        DrawFromParent(child, xz++);
                        ImGui.NextColumn();
                    }
                    
                    ImGui.Text(prop.Name);
                    ImGui.NextColumn();
                    ImGui.Text($"{prop.GetValue(child)}");
                }
                            
                ImGui.TreePop();
            }
        }
        
        public void Draw(GameBase game)
        {
            if (!ImGui.CollapsingHeader("Children"))
                return;
            
            lock (game.Children)
            {
                DrawFromParent(game);
            }
        }
    }
}