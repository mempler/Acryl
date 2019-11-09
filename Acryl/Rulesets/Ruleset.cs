using Acryl.Graphics;
using Acryl.Graphics.Elements;

namespace Acryl.Rulesets
{
    public abstract class Ruleset : Drawable
    {
        public abstract int Id { get; }
        public abstract Sprite Icon { get; }
        public abstract string Name { get; }

        public ILayerStack Stack { get; }

        public Ruleset()
        {
            Stack = new LayerStack();
        }
    }
}