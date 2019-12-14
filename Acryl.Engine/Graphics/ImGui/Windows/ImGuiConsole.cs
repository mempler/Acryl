using System;
using System.Collections.Generic;
using System.IO;
using ImGuiNET;
using Microsoft.Xna.Framework;

namespace Acryl.Engine.Graphics.ImGui.Windows
{
    public class ImGuiConsole : ImGuiWindow
    {
        protected internal override ImGuiWindowFlags WindowFlags => ImGuiWindowFlags.None;
        protected internal override string Name => "Console";
        
        private readonly MemoryStream ConsoleStream = new MemoryStream();
        private StreamWriter ConsoleWriter;
        private StreamWriter RealConsoleWriter;
        private StreamReader ConsoleReader;
        private List<string> lines = new List<string>();

        [LoadAsync]
        private void Load()
        {
            ConsoleWriter = new StreamWriter(ConsoleStream)
            {
                AutoFlush = true
            };
            RealConsoleWriter = new StreamWriter(Console.OpenStandardOutput())
            {
                AutoFlush = true
            };
            Console.SetOut(ConsoleWriter);
            
            ConsoleReader = new StreamReader(ConsoleWriter.BaseStream);
            
            Console.WriteLine("It's only a game");
            Console.WriteLine("why you have to be mad ?");
        }

        protected override void Update(GameTime gameTime)
        {
            if (lines.Count > 512 * 32) {
                lines.Clear();
            }
            
            ConsoleStream.Position = 0;
            var ln = string.Empty;
            while (ln != null)
            {
                ln = ConsoleReader.ReadLine();
                
                if (ln != null)
                    RealConsoleWriter.WriteLine(ln);
                
                if (ln != null)
                    lines.Add(ln);
                
                if (ln == null)
                    ConsoleStream.SetLength(0);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            if (ConsoleReader == null)
                return;
            
            ImGuiNET.ImGui.TextUnformatted(string.Join("\n", lines) ?? string.Empty);
            ImGuiNET.ImGui.SetScrollHereY(1);
        }
        
        public override void Dispose(bool isDisposing)
        {
            ConsoleStream?.Dispose();
            RealConsoleWriter?.Dispose();
            ConsoleReader?.Dispose();
            ConsoleWriter?.Dispose();
            
            base.Dispose(isDisposing);
        }
    }
}