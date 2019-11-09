using System;
using Acryl.Graphics;
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
            var (width, height) = original;
            
            var scaleX = Width / width;
            var scaleY = Height / height;
           
            return new Vector2(scaleX, scaleY);
        }
    }
}