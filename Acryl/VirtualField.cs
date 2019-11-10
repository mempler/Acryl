using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl
{
    public class VirtualField
    {
        public float Width;
        public float Height;

        public VirtualField(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public Vector2 ApplyPosition(Vector2 original)
        {
            throw new NotImplementedException();
        }

        public Vector2 ApplyScale(Texture2D original)
            => ApplyScale(new Vector2(original.Width, original.Height));
        
        public Vector2 ApplyScale(Vector2 original) // Converts scale to Virtual Scale && applies to Screen Scale
        {
            var fieldAspect = Width / Height;
            var originalAspect = original.X / original.Y;
            
            float scaleFactor;
            if (fieldAspect > originalAspect)
                scaleFactor = Height / original.Y;
            else
                scaleFactor = Width / original.X;

            return new Vector2(scaleFactor);
        }
    }
}