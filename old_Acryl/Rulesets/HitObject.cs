namespace Acryl.Rulesets
{
    public enum HitObjectKind
    {
        Circle,
        Slider,
        Spinner,
    }
    
    public abstract class HitObject : Drawable
    {
        public HitObjectKind Kind;
        public TimingPoint TimingPoint;
        public int Timing;
        public int Index = 0;
    }
}