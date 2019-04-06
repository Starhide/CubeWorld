using System;

namespace BlockWorld.util
{
    /// <summary>
    /// An readonly 3 integer vector.
    /// </summary>
    public class Vector3IR : IEquatable<Vector3IR>
    {
        public int X { get; }
        public int Y { get; }
        public int Z { get; }

        public Vector3IR(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public bool Equals(Vector3IR other)
        {
            if (other is null)
                return false;
            if (object.ReferenceEquals(this, other))
                return true;

            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Vector3IR);
        }

        public override int GetHashCode()
        {
            return (((X * 31) ^ Y) * 61) ^ Z;
        }

        public override string ToString()
        {
            return "(" + X + ", " + Y + ", " + Z + ")";
        }

        public static bool operator ==(Vector3IR c1, Vector3IR c2)
        {
            if (object.ReferenceEquals(c1, c2))
            { return true; }
            if (c1 is null || c2 is null)
            { return false; }
            return c1.X == c2.X && c1.Y == c2.Y && c1.Z == c2.Z;
        }

        public static bool operator !=(Vector3IR c1, Vector3IR c2)
        {
            return !(c1 == c2);
        }

        public static Vector3IR operator *(Vector3IR v, int s)
        {
            return new Vector3IR(v.X * s, v.Y * s, v.Z * s);
        }
    }
}
