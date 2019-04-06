using System;

namespace BlockWorld.util
{
    public class Vector3IRModded : IEquatable<Vector3IRModded>
    {
        public int X { get; }
        public int Y { get; }
        public int Z { get; }

        public Vector3IRModded(int x, int y, int z, Vector3IR mod) :
            this(x, y, z, mod.X, mod.Y, mod.Z)
        { }

        public Vector3IRModded(int x, int y, int z, int mx, int my, int mz)
        {
            X = MathExt.AbsMod(x, mx);
            Y = MathExt.AbsMod(y, my);
            Z = MathExt.AbsMod(z, mz);
        }

        public bool Equals(Vector3IRModded other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Vector3IRModded);
        }

        public override int GetHashCode()
        {
            return (((X * 31) ^ Y) * 61) ^ Z;
        }

        public override string ToString()
        {
            return "(" + X + ", " + Y + ", " + Z + ")";
        }

        public static bool operator ==(Vector3IRModded c1, Vector3IRModded c2)
        {
            if (object.ReferenceEquals(c1, c2))
            { return true; }
            if (c1 is null || c2 is null)
            { return false; }
            return c1.X == c2.X && c1.Y == c2.Y && c1.Z == c2.Z;
        }

        public static bool operator !=(Vector3IRModded c1, Vector3IRModded c2)
        {
            return !(c1 == c2);
        }
    }
}
