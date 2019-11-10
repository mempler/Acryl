using Acryl.Rulesets;

namespace Acryl.Graphics
{
    public abstract class Layer : Drawable
    {
        public Beatmap Beatmap => BeatmapManager.ActiveBeatmap;
        
    }
}