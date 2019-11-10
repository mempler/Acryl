using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl.Graphics.Skin
{
    public enum SkinElementType
    {
        Skinnable
    }
    
    public static class SkinManager
    {
        private static Dictionary<string, Texture2D> SkinnableTextures
            = new Dictionary<string, Texture2D>();
        
        private static Dictionary<string, Effect> SkinnableEffects
            = new Dictionary<string, Effect>();

        
        public static Color[] SkinColors =
        {
            Color.Red,
            Color.Blue,
            Color.Yellow,
            Color.Green
        };

        private static string _skinName;
        public static string SkinName
        {
            get => _skinName;
            set
            {
                SkinnableTextures.Clear(); // Reload Skin
                _skinName = value;
            }
        }

        public static Texture2D GetSkinElement(string element)
        {
            if (SkinnableTextures.TryGetValue(element, out var tex))
                return tex;

            var skinElementPath = Path.Combine(AcrylGame.AcrylDirectory, $"Skins/{SkinName}/{element}.png");
            if (File.Exists(skinElementPath))
            {
                using var fs = File.OpenRead(skinElementPath);
                tex = Texture2D.FromStream(AcrylGame.Game.GraphicsDevice, fs);
            }
            else
                tex = AcrylGame.Game.Content.Load<Texture2D>("SkinnableTextures/" + element);

            SkinnableTextures[element] = tex;
            return tex;
        }

        public static Effect GetEffect(string element)
        {
            if (SkinnableEffects.TryGetValue(element, out var effect))
                return effect;
            
            effect = AcrylGame.Game.Content.Load<Effect>(element);
            SkinnableEffects[element] = effect;

            return effect;
        }
    }
}