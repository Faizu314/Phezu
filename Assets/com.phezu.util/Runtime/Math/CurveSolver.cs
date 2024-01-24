using System;
using UnityEngine;

public static class CurveSolver
{
    public abstract class Curve {
        public abstract float Solve(float x);
    }

    public class StraightLine : Curve {

        readonly private float m;
        readonly private float c;

        public StraightLine(float m, float c) {
            this.m = m;
            this.c = c;
        }

        public override float Solve(float x) {
            return (m * x) + c;
        }
    }

    #region Catenary

    public class CatenaryCurve : Curve {
        private readonly float X;
        private readonly float Y;
        private readonly float p;
        private readonly float g;
        private readonly float b;

        public CatenaryCurve(float X, float Y, float p, float g, float b) {
            this.X = X;
            this.Y = Y;
            this.p = p;
            this.g = g;
            this.b = b;
        }

        public override float Solve(float x) {
            float f1 = X / (p * g);
            float f2 = Y / (p * g);

            return f1 * (float)Math.Cosh((x/f1) + b) - f2;
        }
    }

    //https://www.desmos.com/calculator/w1bwodpfoz
    /// <summary>
    /// Computes the catenary curve with origin between the two points at y = 0.
    /// </summary>
    /// <param name="g">Gravitational constant.</param>
    /// <param name="p">Linear density of rope.</param>
    /// <param name="l">Horizontal distance between two points.</param>
    /// <param name="h1">Height of left point.</param>
    /// <param name="h2">Height of right point.</param>
    /// <param name="L">Length of the rope.</param>
    /// <param name="catenaryTolerance">How accurately the catenary constant will be calculated, must be positive. The Lower the better.</param>
    /// <param name="slackTolerance">How close the length of the rope has to be to the distance between two points for it to be exactly straight. Must be greater than 1.</param>
    public static Curve ComputeCatenary(float g, float p, float l, float h1, float h2, float L, float slackTolerance = 1.005f, float catenaryTolerance = 0.01f, int maxIterations = 100) {
        if (g == 0f)
            return null;
        if (p == 0f)
            return null;
        if (l == 0f)
            return null;
        if (catenaryTolerance <= 0f)
            return null;
        if (slackTolerance <= 1f)
            return ConstructStraightLine(l, h1, h2);
        if (GetSlackRatio(l, h1, h2, L) <= slackTolerance)
            return ConstructStraightLine(l, h1, h2);
        
        float b = (float)Math.Atanh((h2 - h1) / L);
        float X = FindCatenaryConstant(g, p, l, h1, h2, b, catenaryTolerance, maxIterations);
        float Y = FindConstraintConstant(X, p, g, l, b , h2);

        return new CatenaryCurve(X, Y, p, g, b);
    }

    private static float GetSlackRatio(float l, float h1, float h2, float L) {
        Vector2 p1 = new(l / 2f, h2);
        Vector2 p2 = new(-l / 2f, h1);

        return L / Vector2.Distance(p1, p2);
    }

    private static Curve ConstructStraightLine(float l, float h1, float h2) {
        float m = (h2 - h1) / l;
        float c = h2 - (m * (l / 2f));

        return new StraightLine(m, c);
    }

    private static float FindCatenaryConstant(float g, float p, float l, float h1, float h2, float b, float tolerance, int maxIterations) {
        float lowerLimit = tolerance;
        float upperLimit = p * g * l * 2f;
        float mid = (lowerLimit + upperLimit) / 2f;
        float lE = F(g, p, l, h1, h2, b, lowerLimit);
        float uE = F(g, p, l, h1, h2, b, upperLimit);
        float mE = F(g, p, l, h1, h2, b, mid);

        if (lE > 0f)
            return lowerLimit;
        if (uE < 0f)
            return upperLimit;

        int iterations = 0;

        while ((mE - 0f > tolerance || mE < 0f) && (iterations < maxIterations)) {
            if (mE > 0f)
                upperLimit = mid;
            else
                lowerLimit = mid;
            mid = (lowerLimit + upperLimit) / 2f;

            mE = F(g, p, l, h1, h2, b, mid);

            iterations++;
        }

        return mid;
    }

    /// F(X) - X
    private static float F(float g, float p, float l, float h1, float h2, float b, float x) {
        float numerator = (h1 * p * g) - (h2 * p * g);
        float f1 = (p * g * l) / (2f * x);
        float denominator = (float)(Math.Cosh(-f1 + b) - Math.Cosh(f1 + b));

        return (numerator / denominator) - x;
    }

    private static float FindConstraintConstant(float X, float p, float g, float l, float b, float h2) {
        float f1 = (p * g * l) / (2f * X);

        return (X * (float)Math.Cosh(f1 + b)) - (h2 * p * g);
    }

    #endregion

}
