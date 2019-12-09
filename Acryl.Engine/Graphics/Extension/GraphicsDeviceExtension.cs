using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;
using Svg;

namespace Acryl.Engine.Graphics.Extension
{
    public static class GraphicsDeviceExtension
    {
        public static Texture2D GetTexture2DFromBitmap(this GraphicsDevice device, Bitmap bitmap)
        {
            var tex = new Texture2D(device, bitmap.Width, bitmap.Height, false, SurfaceFormat.Color);
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
        
        // https://github.com/mellinoe/ImGui.NET/blob/master/src/ImGui.NET.SampleProgram.XNA/SampleGame.cs
        public static Texture2D CreateTexture(this GraphicsDevice device, int width, int height, Func<int, Microsoft.Xna.Framework.Color> paint)
        {
            //initialize a texture
            var texture = new Texture2D(device, width, height);

            //the array holds the color for each pixel in the texture
            var data = new Microsoft.Xna.Framework.Color[width * height];
            for(var pixel = 0; pixel < data.Length; pixel++)
            {
                //the function applies the color according to the specified pixel
                data[pixel] = paint( pixel );
            }

            //set the color
            texture.SetData( data );

            return texture;
        }
        
        public static Texture2D SvgToTexture2D(this GraphicsDevice device, Stream stream, int width = 500, int height = 500)
        {
            var doc = SvgDocument.Open<SvgDocument>(stream);
            
            doc.Color = new SvgColourServer(Color.White);
            using var btm = doc.Draw(width, height);
            var tex = device.GetTexture2DFromBitmap(btm);
            return tex;
        }
    }
}