using Acryl.Graphics.Elements;
using Acryl.Rulesets.osu.Layers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl.Rulesets.osu
{
    public class OsuRuleset : Ruleset
    {
        public override int Id => 0;
        public override Sprite Icon => null;
        public override string Name => "osu";

        public OsuRuleset()
        {
            Stack.Push(new SliderLayer());
            Stack.Push(new CircleLayer());
            Stack.Push(new UiLayer());
        }
    }
}