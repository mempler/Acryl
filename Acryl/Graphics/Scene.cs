using Microsoft.Xna.Framework.Graphics;

namespace Acryl.Graphics
{
    public abstract class Scene : Drawable
    {
        public bool IsActive { get; private set; } = false;

        public virtual void Begin(SpriteBatch batch)
        {
            batch.Begin();
        }

        public virtual void End(SpriteBatch batch)
        {
            batch.End();
        }
        
        public virtual void SwitchTo(Scene scene)
        {
            scene.IsActive = true;
            IsActive = false;
        }
    }
}