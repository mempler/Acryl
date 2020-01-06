namespace Acryl.Graphics
{
    public abstract class Scene : Drawable
    {
        public bool IsActive { get; private set; } = false;
        
        public virtual void SwitchTo(Scene scene)
        {
            scene.IsActive = true;
            IsActive = false;
        }
    }
}