using System;
using System.Drawing;

namespace Acryl.Engine.Graphics.Extension
{
    public static class ColorExtension
    {
        public static Color Lerp(this Color _1, Color _2, float _3)
        {
            var rf = _1.R + _3 * (_2.R - _1.R);
            var gf = _1.G + _3 * (_2.G - _1.G);
            var bf = _1.B + _3 * (_2.B - _1.B);
            var af = _1.A + _3 * (_2.A - _1.A);

            var r = (int) MathF.Round(rf);
            var g = (int) MathF.Round(gf);
            var b = (int) MathF.Round(bf);
            var a = (int) MathF.Round(af);

            return Color.FromArgb(r, g, b, a);
        }
    }
}