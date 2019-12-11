using System.Collections.Generic;
using System.IO;
using Acryl.Engine;
using Acryl.Engine.Stores;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl.Graphics
{
    public class SkinManager : DependencyContainer
    {
        private Dictionary<string, Texture2D> SkinnableTextures
            = new Dictionary<string, Texture2D>();
        
        [DependencyResolved]
        private GraphicsDevice Device { get; set; }
        
        [DependencyResolved]
        private TextureStore TextureStore { get; set; }
        
        public Color[] SkinColors =
        {
            Color.Red,
            Color.Blue,
            Color.Yellow,
            Color.Green
        };
        
        private string _skinName;
        public string SkinName
        {
            get => _skinName;
            set
            {
                SkinnableTextures.Clear(); // Reload Skin
                _skinName = value;
            }
        }

        public Texture2D Get(string element)
        {
            if (SkinnableTextures.TryGetValue(element, out var tex))
                return tex;

            var skinElementPath = Path.Combine(AcrylGame.AcrylDirectory, $"Skins/{SkinName}/{element}.png");
            tex = TextureStore.Get(
                File.Exists(skinElementPath) ?
                    $"file://{skinElementPath}" :
                    $"Resources/SkinnableTextures/{element}.png"
                );

            SkinnableTextures[element] = tex;
            return tex;
        }
    }
}