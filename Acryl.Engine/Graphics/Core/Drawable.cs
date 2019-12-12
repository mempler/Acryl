using System;
using Acryl.Engine.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tweening;

namespace Acryl.Engine.Graphics.Core
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

        internal bool HasTmpAltered = false;
        internal Color TmpColor = Color.White;
        
        public Vector2 Size { get; set; } = Vector2.Zero;
        public Vector2 Scale { get; set; } = Vector2.One;
        public float Rotation { get; set; } = 0;
        public float Alpha = 1f; // between 0 and 1
        
        private bool _visible = true;
        public bool Visible { get => _visible && Alpha > 0f && (Parent?.Visible ?? true); set => _visible = value; }

        public bool DrawChildren = true;
        
        [DependencyResolved]
        protected VirtualField Field { get; set; }
        
        protected Tweener Tweener { get; } = new Tweener();

        internal IntPtr ImTex { get; set; }

        public (Color color, Rectangle destRect, float rotation, Vector2 origin) CalculateFrame(float width, float height)
        {
            var alpha = Alpha * (Parent?.Alpha ?? 1f);
            
            var origin = new Vector2();
            if ((Origin & Origin.Top) != 0)
                origin.Y = 0;
            if ((Origin & Origin.Bottom) != 0)
                origin.Y = height;
            
            if ((Origin & Origin.Left) != 0)
                origin.Y = 0;
            if ((Origin & Origin.Right) != 0)
                origin.X = width;
            
            if ((Origin & Origin.Center) != 0) {
                origin.X = width / 2f;
                origin.Y = height / 2f;
                
                if ((Origin & Origin.Top) != 0)
                    origin.Y = 0;
                
                else if ((Origin & Origin.Bottom) != 0)
                    origin.Y = height;
            
                if ((Origin & Origin.Left) != 0)
                    origin.Y = 0;
                else if ((Origin & Origin.Right) != 0)
                    origin.X = width / 2f;
            }
            
            // ReSharper disable once UseDeconstruction
            var originScale = Field.ApplyScale(origin);
            var positionOffset = new Vector2();
            
            if ((PositionOrigin & Origin.Top) != 0)
                positionOffset.X = 0;
            if ((PositionOrigin & Origin.Bottom) != 0)
                positionOffset.Y = Field.Height-originScale.Y;
            
            if ((PositionOrigin & Origin.Left) != 0)
                positionOffset.Y = 0;
            if ((PositionOrigin & Origin.Right) != 0)
                positionOffset.X = Field.Width-originScale.X;
            
            if ((PositionOrigin & Origin.Center) != 0) {
                positionOffset.X = (Field.Width-originScale.X) / 2f;
                positionOffset.Y = (Field.Height-originScale.Y) / 2f;
                
                if ((PositionOrigin & Origin.Top) != 0)
                    positionOffset.Y = 0;
                
                else if ((PositionOrigin & Origin.Bottom) != 0)
                    positionOffset.Y = Field.Height-originScale.Y;
            
                if ((PositionOrigin & Origin.Left) != 0)
                    positionOffset.Y = 0;
                
                else if ((PositionOrigin & Origin.Right) != 0)
                    positionOffset.X = (Field.Width-originScale.X) / 2f;
            }

            var col = Color;
            if (Parent?.Color != null && !HasTmpAltered)
                col = Color.Lerp(Color, Parent.Color, 1); // inherit Parent Colors.
            else if (HasTmpAltered)
                col = TmpColor;
            
            var realColor = new Color(col, Math.Min(alpha, 1f));
            var tmpPos = Position + PositionOffset + positionOffset + (Parent?.Position ?? Vector2.Zero) +
                         (Parent?.PositionOffset ?? Vector2.Zero);
            
            var realPos = new Vector2(
                MathF.Round(tmpPos.X, MidpointRounding.AwayFromZero), // <------
                MathF.Round(tmpPos.Y, MidpointRounding.AwayFromZero));
            
            var realRotation = (Rotation + (Parent?.Rotation ?? 0)) / ((float)Math.PI * 2);
            var realScale = Scale * (Parent?.Scale ?? Vector2.One);
            var size = Size + (Parent?.Size ?? Vector2.Zero);

            var destRect = new Rectangle(
                (int) realPos.X, (int) realPos.Y,
                (int) MathF.Round(size.X * realScale.X, MidpointRounding.AwayFromZero),
                (int) MathF.Round(size.Y * realScale.Y, MidpointRounding.AwayFromZero));
            
            return (realColor, destRect, realRotation, origin);
        }

        public void DrawFrame(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!Visible) // Don't draw anything that isn't even visible
                return;
            
            Draw(spriteBatch, gameTime);
            if (!DrawChildren)
                return;
            
            lock(Children)
                foreach (var child in Children) // Iterate through Child and it's Children to draw a Frame
                    child.DrawFrame(spriteBatch, gameTime);
        }
        
        public void UpdateFrame(GameTime gameTime)
        {
            Update(gameTime);
            
            Tweener.Update((float) gameTime.ElapsedGameTime.TotalSeconds);

            lock(Children)
                foreach (var child in Children) // Iterate through Child and it's Children to draw a Frame
                    child.UpdateFrame(gameTime);
        }

        protected virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
        }

        protected virtual void Update(GameTime gameTime)
        {
        }

        public Tween<Vector2> MoveTo(Vector2 to, float duration, float delay)
        {
            return Tweener.TweenTo(this,
                e => e.Position, to,
                duration, delay);
        }
        
        public Tween<Vector2> ScaleTo(Vector2 to, float duration, float delay)
        {
            return Tweener.TweenTo(this,
                e => e.Scale, to,
                duration, delay);
        }
        
        public Tween<float> FadeTo(float to, float duration, float delay)
        {
            return Tweener.TweenTo(this,
                e => e.Alpha, to,
                duration, delay);
        }
    }
}