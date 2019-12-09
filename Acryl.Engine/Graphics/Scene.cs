using Acryl.Engine.Graphics.Core;
using MonoGame.Extended.Tweening;

namespace Acryl.Engine.Graphics
{
    public class Scene : Drawable
    {
        public virtual Tween SwitchTo(Scene newScene)
        {
            return
                Tweener.TweenTo(this, s => s.Alpha, 0, .5f)
                    .OnEnd(s => Tweener.TweenTo(newScene, s2 => s2.Alpha, 1, .5f).Easing(EasingFunctions.Linear))
                    .Easing(EasingFunctions.Linear);
        }
    }
}