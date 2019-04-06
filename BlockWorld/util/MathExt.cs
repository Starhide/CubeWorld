using OpenTK;
using System;

namespace BlockWorld.util
{
    public class MathExt
    {

        public static int FloorToInt(double d)
        {
            return (int)Math.Floor(d);
        }

        public static int AbsMod(int a, int m)
        {
            int r = a % m;
            return r < 0 ? r + m : r;
        }

        public static float AbsMod(float a, float m)
        {
            float r = a % m;
            return r < 0 ? r + m : r;
        }

        public static double AbsMod(double a, double m)
        {
            double r = a % m;
            return r < 0 ? r + m : r;
        }

        public static int DisDirMod(int i, int t, int m)
        {
            int q = t - i;
            int w = t - (i + m);
            int e = t + m - i;
            if (Math.Abs(q) < Math.Abs(w) && Math.Abs(q) < Math.Abs(e))
            {
                return q;
            }
            else if (Math.Abs(w) < Math.Abs(e))
            {
                return w;
            }
            else
            {
                return e;
            }
        }

        /**
         * Seems Expensive may need to optimize
         * 
         * Returns the distance and direction from i to t in modulo m
         * */
        public static float DisDirMod(float i, float t, float m)
        {
            float q = t - i;
            float w = t - (i + m);
            float e = t + m - i;
            if (Math.Abs(q) < Math.Abs(w) && Math.Abs(q) < Math.Abs(e))
            {
                return q;
            }
            else if (Math.Abs(w) < Math.Abs(e))
            {
                return w;
            }
            else
            {
                return e;
            }
        }

        public static Vector3 DirectionMod(Vector3 a, Vector3 b, float mx, float my, float mz)
        {
            return new Vector3(
                    DisDirMod(a.X, b.X, mx),
                    DisDirMod(a.Y, b.Y, my),
                    DisDirMod(a.Z, b.Z, mz)
                );
        }

        public static Vector3 DirectionMod(Vector3 a, Vector3 b, Vector3IR s)
        {
            return DirectionMod(a, b, s.X, s.Y, s.Z);
        }

        public static float DistanceMod(Vector3 a, Vector3 b, Vector3IR s)
        {
            return DirectionMod(a, b, s.X, s.Y, s.Z).LengthFast;
        }

        public static Vector3 AbsModVector(Vector3 vector3, Vector3IR size)
        {
            return new Vector3(AbsMod(vector3.X, size.X), AbsMod(vector3.Y, size.Y), AbsMod(vector3.Z, size.Z));
        }
    }
}
