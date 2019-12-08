using Acryl.Graphics;
using Acryl.Graphics.Elements;

namespace Acryl.Rulesets.Layers
{
    public class BackgroundLayer : Layer
    {
        private static Sprite BackgroundImage => BeatmapManager.ActiveBeatmap.Background;
        
        public BackgroundLayer()
        {
            Add(new CachedRenderTarget
            {
                Sprites = new []{  BackgroundImage  },
                Blur = true,
                BlurStrength = 4f
            });
        }
    }
}