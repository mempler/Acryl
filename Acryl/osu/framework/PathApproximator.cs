// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.
// https://github.com/ppy/osu-framework/blob/master/osu.Framework/MathUtils/PathApproximator.cs

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Acryl.Engine.osu.framework
{
    /// <summary>
    /// Helper methods to approximate a path by interpolating a sequence of control points.
    /// </summary>
    public static class PathApproximator
    {
        private const float BezierTolerance = 0.25f;

        /// <summary>
        /// The amount of pieces to calculate for each control point quadruplet.
        /// </summary>
        private const int CatmullDetail = 50;

        private const float CircularArcTolerance = 0.1f;

        /// <summary>
        /// Creates a piecewise-linear approximation of a bezier curve, by adaptively repeatedly subdividing
        /// the control points until their approximation error vanishes below a given threshold.
        /// </summary>
        /// <returns>A list of vectors representing the piecewise-linear approximation.</returns>
        public static List<Vector2> ApproximateBezier(ReadOnlySpan<Vector2> controlPoints)
        {
            var output = new List<Vector2>();
            var count = controlPoints.Length;

            if (count == 0)
                return output;

            var subdivisionBuffer1 = new Vector2[count];
            var subdivisionBuffer2 = new Vector2[count * 2 - 1];

            var toFlatten = new Stack<Vector2[]>();
            var freeBuffers = new Stack<Vector2[]>();

            // "toFlatten" contains all the curves which are not yet approximated well enough.
            // We use a stack to emulate recursion without the risk of running into a stack overflow.
            // (More specifically, we iteratively and adaptively refine our curve with a
            // <a href="https://en.wikipedia.org/wiki/Depth-first_search">Depth-first search</a>
            // over the tree resulting from the subdivisions we make.)
            toFlatten.Push(controlPoints.ToArray());

            var leftChild = subdivisionBuffer2;

            while (toFlatten.Count > 0)
            {
                var parent = toFlatten.Pop();

                if (BezierIsFlatEnough(parent))
                {
                    // If the control points we currently operate on are sufficiently "flat", we use
                    // an extension to De Casteljau's algorithm to obtain a piecewise-linear approximation
                    // of the bezier curve represented by our control points, consisting of the same amount
                    // of points as there are control points.
                    BezierApproximate(parent, output, subdivisionBuffer1, subdivisionBuffer2, count);

                    freeBuffers.Push(parent);
                    continue;
                }

                // If we do not yet have a sufficiently "flat" (in other words, detailed) approximation we keep
                // subdividing the curve we are currently operating on.
                var rightChild = freeBuffers.Count > 0 ? freeBuffers.Pop() : new Vector2[count];
                BezierSubdivide(parent, leftChild, rightChild, subdivisionBuffer1, count);

                // We re-use the buffer of the parent for one of the children, so that we save one allocation per iteration.
                for (var i = 0; i < count; ++i)
                    parent[i] = leftChild[i];

                toFlatten.Push(rightChild);
                toFlatten.Push(parent);
            }

            output.Add(controlPoints[count - 1]);
            return output;
        }

        /// <summary>
        /// Creates a piecewise-linear approximation of a Catmull-Rom spline.
        /// </summary>
        /// <returns>A list of vectors representing the piecewise-linear approximation.</returns>
        public static List<Vector2> ApproximateCatmull(ReadOnlySpan<Vector2> controlPoints)
        {
            var result = new List<Vector2>((controlPoints.Length - 1) * CatmullDetail * 2);

            for (var i = 0; i < controlPoints.Length - 1; i++)
            {
                var v1 = i > 0 ? controlPoints[i - 1] : controlPoints[i];
                var v2 = controlPoints[i];
                var v3 = i < controlPoints.Length - 1 ? controlPoints[i + 1] : v2 + v2 - v1;
                var v4 = i < controlPoints.Length - 2 ? controlPoints[i + 2] : v3 + v3 - v2;

                for (var c = 0; c < CatmullDetail; c++)
                {
                    result.Add(CatmullFindPoint(ref v1, ref v2, ref v3, ref v4, (float)c / CatmullDetail));
                    result.Add(CatmullFindPoint(ref v1, ref v2, ref v3, ref v4, (float)(c + 1) / CatmullDetail));
                }
            }

            return result;
        }

        /// <summary>
        /// Creates a piecewise-linear approximation of a circular arc curve.
        /// </summary>
        /// <returns>A list of vectors representing the piecewise-linear approximation.</returns>
        public static List<Vector2> ApproximateCircularArc(ReadOnlySpan<Vector2> controlPoints)
        {
            var a = controlPoints[0];
            var b = controlPoints[1];
            var c = controlPoints[2];

            var aSq = (b - c).LengthSquared();
            var bSq = (a - c).LengthSquared();
            var cSq = (a - b).LengthSquared();

            // If we have a degenerate triangle where a side-length is almost zero, then give up and fall
            // back to a more numerically stable method.
            if (Precision.AlmostEquals(aSq, 0) || Precision.AlmostEquals(bSq, 0) || Precision.AlmostEquals(cSq, 0))
                return new List<Vector2>();

            var s = aSq * (bSq + cSq - aSq);
            var t = bSq * (aSq + cSq - bSq);
            var u = cSq * (aSq + bSq - cSq);

            var sum = s + t + u;

            // If we have a degenerate triangle with an almost-zero size, then give up and fall
            // back to a more numerically stable method.
            if (Precision.AlmostEquals(sum, 0))
                return new List<Vector2>();

            var centre = (s * a + t * b + u * c) / sum;
            var dA = a - centre;
            var dC = c - centre;

            var r = dA.Length();

            var thetaStart = Math.Atan2(dA.Y, dA.X);
            var thetaEnd = Math.Atan2(dC.Y, dC.X);

            while (thetaEnd < thetaStart)
                thetaEnd += 2 * Math.PI;

            double dir = 1;
            var thetaRange = thetaEnd - thetaStart;

            // Decide in which direction to draw the circle, depending on which side of
            // AC B lies.
            var orthoAtoC = c - a;
            orthoAtoC = new Vector2(orthoAtoC.Y, -orthoAtoC.X);

            if (Vector2.Dot(orthoAtoC, b - a) < 0)
            {
                dir = -dir;
                thetaRange = 2 * Math.PI - thetaRange;
            }

            // We select the amount of points for the approximation by requiring the discrete curvature
            // to be smaller than the provided tolerance. The exact angle required to meet the tolerance
            // is: 2 * Math.Acos(1 - TOLERANCE / r)
            // The special case is required for extremely short sliders where the radius is smaller than
            // the tolerance. This is a pathological rather than a realistic case.
            var amountPoints = 2 * r <= CircularArcTolerance ? 2 : Math.Max(2, (int)Math.Ceiling(thetaRange / (2 * Math.Acos(1 - CircularArcTolerance / r))));

            var output = new List<Vector2>(amountPoints);

            for (var i = 0; i < amountPoints; ++i)
            {
                var fract = (double)i / (amountPoints - 1);
                var theta = thetaStart + dir * fract * thetaRange;
                var o = new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta)) * r;
                output.Add(centre + o);
            }

            return output;
        }

        /// <summary>
        /// Creates a piecewise-linear approximation of a linear curve.
        /// Basically, returns the input.
        /// </summary>
        /// <returns>A list of vectors representing the piecewise-linear approximation.</returns>
        public static List<Vector2> ApproximateLinear(ReadOnlySpan<Vector2> controlPoints)
        {
            var result = new List<Vector2>(controlPoints.Length);

            foreach (var c in controlPoints)
                result.Add(c);

            return result;
        }

        /// <summary>
        /// Creates a piecewise-linear approximation of a lagrange polynomial.
        /// </summary>
        /// <returns>A list of vectors representing the piecewise-linear approximation.</returns>
        public static List<Vector2> ApproximateLagrangePolynomial(ReadOnlySpan<Vector2> controlPoints)
        {
            // TODO: add some smarter logic here, chebyshev nodes?
            const int numSteps = 51;

            var result = new List<Vector2>(numSteps);

            var weights = Interpolation.BarycentricWeights(controlPoints);

            var minX = controlPoints[0].X;
            var maxX = controlPoints[0].X;

            for (var i = 1; i < controlPoints.Length; i++)
            {
                minX = Math.Min(minX, controlPoints[i].X);
                maxX = Math.Max(maxX, controlPoints[i].X);
            }

            var dx = maxX - minX;

            for (var i = 0; i < numSteps; i++)
            {
                var x = minX + dx / (numSteps - 1) * i;
                var y = (float)Interpolation.BarycentricLagrange(controlPoints, weights, x);
                result.Add(new Vector2(x, y));
            }

            return result;
        }

        /// <summary>
        /// Make sure the 2nd order derivative (approximated using finite elements) is within tolerable bounds.
        /// NOTE: The 2nd order derivative of a 2d curve represents its curvature, so intuitively this function
        ///       checks (as the name suggests) whether our approximation is _locally_ "flat". More curvy parts
        ///       need to have a denser approximation to be more "flat".
        /// </summary>
        /// <param name="controlPoints">The control points to check for flatness.</param>
        /// <returns>Whether the control points are flat enough.</returns>
        private static bool BezierIsFlatEnough(IReadOnlyList<Vector2> controlPoints)
        {
            for (var i = 1; i < controlPoints.Count - 1; i++)
            {
                if ((controlPoints[i - 1] - 2 * controlPoints[i] + controlPoints[i + 1]).LengthSquared() > BezierTolerance * BezierTolerance * 4)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Subdivides n control points representing a bezier curve into 2 sets of n control points, each
        /// describing a bezier curve equivalent to a half of the original curve. Effectively this splits
        /// the original curve into 2 curves which result in the original curve when pieced back together.
        /// </summary>
        /// <param name="controlPoints">The control points to split.</param>
        /// <param name="l">Output: The control points corresponding to the left half of the curve.</param>
        /// <param name="r">Output: The control points corresponding to the right half of the curve.</param>
        /// <param name="subdivisionBuffer">The first buffer containing the current subdivision state.</param>
        /// <param name="count">The number of control points in the original list.</param>
        private static void BezierSubdivide(Vector2[] controlPoints, Vector2[] l, Vector2[] r, Vector2[] subdivisionBuffer, int count)
        {
            var midpoints = subdivisionBuffer;

            for (var i = 0; i < count; ++i)
                midpoints[i] = controlPoints[i];

            for (var i = 0; i < count; i++)
            {
                l[i] = midpoints[0];
                r[count - i - 1] = midpoints[count - i - 1];

                for (var j = 0; j < count - i - 1; j++)
                    midpoints[j] = (midpoints[j] + midpoints[j + 1]) / 2;
            }
        }

        /// <summary>
        /// This uses <a href="https://en.wikipedia.org/wiki/De_Casteljau%27s_algorithm">De Casteljau's algorithm</a> to obtain an optimal
        /// piecewise-linear approximation of the bezier curve with the same amount of points as there are control points.
        /// </summary>
        /// <param name="controlPoints">The control points describing the bezier curve to be approximated.</param>
        /// <param name="output">The points representing the resulting piecewise-linear approximation.</param>
        /// <param name="count">The number of control points in the original list.</param>
        /// <param name="subdivisionBuffer1">The first buffer containing the current subdivision state.</param>
        /// <param name="subdivisionBuffer2">The second buffer containing the current subdivision state.</param>
        private static void BezierApproximate(Vector2[] controlPoints, List<Vector2> output, Vector2[] subdivisionBuffer1, Vector2[] subdivisionBuffer2, int count)
        {
            var l = subdivisionBuffer2;
            var r = subdivisionBuffer1;

            BezierSubdivide(controlPoints, l, r, subdivisionBuffer1, count);

            for (var i = 0; i < count - 1; ++i)
                l[count + i] = r[i + 1];

            output.Add(controlPoints[0]);

            for (var i = 1; i < count - 1; ++i)
            {
                var index = 2 * i;
                var p = 0.25f * (l[index - 1] + 2 * l[index] + l[index + 1]);
                output.Add(p);
            }
        }

        /// <summary>
        /// Finds a point on the spline at the position of a parameter.
        /// </summary>
        /// <param name="vec1">The first vector.</param>
        /// <param name="vec2">The second vector.</param>
        /// <param name="vec3">The third vector.</param>
        /// <param name="vec4">The fourth vector.</param>
        /// <param name="t">The parameter at which to find the point on the spline, in the range [0, 1].</param>
        /// <returns>The point on the spline at <paramref name="t"/>.</returns>
        private static Vector2 CatmullFindPoint(ref Vector2 vec1, ref Vector2 vec2, ref Vector2 vec3, ref Vector2 vec4, float t)
        {
            var t2 = t * t;
            var t3 = t * t2;

            Vector2 result;
            result.X = 0.5f * (2f * vec2.X + (-vec1.X + vec3.X) * t + (2f * vec1.X - 5f * vec2.X + 4f * vec3.X - vec4.X) * t2 + (-vec1.X + 3f * vec2.X - 3f * vec3.X + vec4.X) * t3);
            result.Y = 0.5f * (2f * vec2.Y + (-vec1.Y + vec3.Y) * t + (2f * vec1.Y - 5f * vec2.Y + 4f * vec3.Y - vec4.Y) * t2 + (-vec1.Y + 3f * vec2.Y - 3f * vec3.Y + vec4.Y) * t3);

            return result;
        }
    }
}
