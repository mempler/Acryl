using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = System.Drawing.Rectangle;

namespace Acryl.Extension
{
    public static class Texture2DExtension
    {
        public static Texture2D TexFromBitmap(GraphicsDevice device, Bitmap bitmap)
        {
            var tex = new Texture2D(device, bitmap.Width, bitmap.Height);
            
            var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);

            var bufferSize = data.Height * data.Stride;

            //create data buffer 
            var bytes = new byte[bufferSize];    

            // copy bitmap data into buffer
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            // copy our buffer to the texture
            tex.SetData(bytes);

            // unlock the bitmap data
            bitmap.UnlockBits(data);

            return tex;
        }
        
        public static Texture2D CreateCircle(this GraphicsDevice graphicsDevice, int radius, Color color)
        {
            var texture = new Texture2D(graphicsDevice, radius, radius, false, SurfaceFormat.Color);
            var colorData = new Color[radius*radius];

            var diam = radius / 2f;
            var diamsq = diam * diam;

            for (var x = 0; x < radius; x++)
            {
                for (var y = 0; y < radius; y++)
                {
                    var index = x * radius + y;
                    var pos = new Vector2(x - diam, y - diam);
                    if (pos.LengthSquared() <= diamsq)
                    {
                        colorData[index] = color;
                    }
                    else
                    {
                        colorData[index] = Color.Transparent;
                    }
                }
            }

            texture.SetData(colorData);
            return texture;
        }
    }
}