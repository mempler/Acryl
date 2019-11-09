using System;
using Microsoft.Xna.Framework;

namespace Acryl.Helpers
{
    public class Line
    {
        public Vector2 Begin;
        public Vector2 End;
        public bool ForceEnd = false;
        public bool Straight = false;

        public float rho => (End - Begin).Length();
        public float theta => (float)Math.Atan2(End.Y - Begin.Y, End.X - Begin.X);

        public float DistanceSquaredToPoint(Vector2 p)
            => Vector2.DistanceSquared(p, ClosestPointTo(p));

        
        public float DistanceToPoint(Vector2 p)
            => Vector2.Distance(p, ClosestPointTo(p));


        // http://geomalgorithms.com/a02-_lines.html
        public Vector2 ClosestPointTo(Vector2 p)
        {
            Vector2 v = End - Begin;
            var w = p - Begin;

            var c1 = Vector2.Dot(w, v);
            if (c1 <= 0)
                return Begin;
            
            var c2 = Vector2.Dot(v, v);
            if (c2 <= c1)
                return End;
            
            var b = c1 / c2;
            var pB = Begin + b * v;

            return pB;
        }
        
        internal Matrix WorldMatrix()
        {
            var rotate = Matrix.CreateRotationZ(theta);
            var translate = Matrix.CreateTranslation(Begin.X, Begin.Y, 0);
            return rotate * translate;
        }
        
        internal Matrix EndWorldMatrix()
            => Matrix.CreateRotationZ(theta) * Matrix.CreateTranslation(End.X, End.Y, 0);


        public Line(Vector2 begin, Vector2 end)
        {
            Begin = begin;
            End = end;
        }
    }
}