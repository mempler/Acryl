using System;
using Acryl.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl.Graphics
{
    [Flags]
    public enum Origin
    {
        None   = 0,
        Left   = 1 << 1,
        Top    = 1 << 2,
        Right  = 1 << 3,
        Bottom = 1 << 4,
        Center = 1 << 5
    }
    
    public abstract class Drawable : ChildrenContainer<Drawable>
    {
        public Vector2 Position { get; set; } = Vector2.Zero;
        public Vector2 PositionOffset { get; set; } = Vector2.Zero;
        public Color Color { get; set; } = Color.White;
        
        public Origin Origin { get; set; } = Origin.Top | Origin.Left;
        public Origin PositionOrigin { get; set; } = Origin.Top | Origin.Left;
        
        public Vector2 Scale { get; set; } = Vector2.One;

        public float Rotation { get; set; } = 0;

        public SpriteEffects Effects { get; set; } = SpriteEffects.None;

        private bool _visible = true;
        public bool Visible { get => _visible && Alpha > 0f && (Parent?.Visible ?? true); set => _visible = value; }

        public float Alpha = 1f; // between 0 and 1

        public bool Freeze;

        public void DrawFrame(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!Visible) // Don't draw anything that isn't even visible
                return;

            Draw(spriteBatch, gameTime);
            if (!Visible) // Don't draw anything that isn't even visible
                return;

            foreach (var child in Children) // Iterate through Child and it's Children to Draw a Frame
                child.DrawFrame(spriteBatch, gameTime);
        }
        
        public void UpdateFrame(GameTime gameTime)
        {
            Update(gameTime);
            foreach (var child in Children) // Iterate through Child and it's Children to Draw a Frame
                child.UpdateFrame(gameTime);
        }

        protected virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            
        }

        protected virtual void Update(GameTime gameTime)
        {
            
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="position">Position which should we move to</param>
        /// <param name="duration">Duration between tweening, in MS</param>
        /// <param name="easingType"></param>
        public void MoveTo(Vector2 position, float duration, EasingType easingType = EasingType.None)
        {
            Easing.ApplyEasing(eas =>
                {
                    eas.Freeze = Freeze;
                    Position = eas.Current;
                },
                Position, position,
                duration, easingType);
        }

        public void ScaleTo(float scale, float duration, EasingType easingType = EasingType.None)
            => ScaleTo(new Vector2(scale), duration, easingType);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scale">Scale which we should scale to.</param>
        /// <param name="duration">Duration between tweening, in MS</param>
        /// <param name="easingType"></param>
        public void ScaleTo(Vector2 scale, float duration, EasingType easingType = EasingType.None)
        {
            var easing = 
            Easing.ApplyEasing(eas =>
                {
                    eas.Freeze = Freeze;
                    Scale = eas.Current;
                },
                Scale, scale,
                duration, easingType);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="alpha">Alpha we should fade to..</param>
        /// <param name="duration">Duration between tweening, in MS</param>
        /// <param name="easingType"></param>
        public void FadeTo(float alpha, float duration, EasingType easingType = EasingType.None)
        {
            Easing.ApplyEasing(eas =>
                {
                    eas.Freeze = Freeze;
                    Alpha = (float) eas.Current;
                },
                Alpha, alpha,
                duration, easingType);
        }
    }
}