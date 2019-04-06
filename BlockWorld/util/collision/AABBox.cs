using OpenTK;
using System;

namespace BlockWorld.util.collision
{
    public class AABBox
    {
        public Vector3 Position;
        public Vector3 Size;

        public float MinX => Position.X;
        public float MaxX => Position.X + Size.X;
        public float MinY => Position.Y;
        public float MaxY => Position.Y + Size.Y;
        public float MinZ => Position.Z;
        public float MaxZ => Position.Z + Size.Z;

        public Vector3 Center
        {
            get => Position + Size / 2;
            set => Position = value - Size / 2;
        }

        public Vector3 CenterXZ
        {
            get => Position + new Vector3(Size.X, 0.0f, Size.Z) / 2;
            set => Position = new Vector3(value.X - Size.X / 2, value.Y, value.Z - Size.Z / 2);
        }

        public AABBox(Vector3 position, Vector3 size)
        {
            Position = position;
            Size = size;
        }

        public AABBox(Vector3 pos)
        {
            Position = pos;
            Size = new Vector3(1, 1, 1);
        }

        public bool Inside(Vector3 point)
        {
            return Position.X <= point.X && point.X <= Position.X + Size.X &&
                Position.Y <= point.Y && point.Y <= Position.Y + Size.Y &&
                Position.Z <= point.Z && point.Z <= Position.Z + Size.Z;
        }

        public AABBox ExpandFromCenter(float ratio)
        {
            return ExpandFromCenter(Vector3.One * ratio);
        }

        public AABBox ExpandFromCenter(Vector3 ratios)
        {
            return new AABBox(Position + Size * (Vector3.One - ratios) / 2, Size * ratios);
        }

        public AABBox Expand(Vector3 amount)
        {
            return new AABBox(new Vector3(
                amount.X < 0 ? Position.X + amount.X : Position.X,
                amount.Y < 0 ? Position.Y + amount.Y : Position.Y,
                amount.Z < 0 ? Position.Z + amount.Z : Position.Z
                ),
                new Vector3(
                amount.X > 0 ? Size.X + amount.X : Size.X - amount.X,
                amount.Y > 0 ? Size.Y + amount.Y : Size.Y - amount.Y,
                amount.Z > 0 ? Size.Z + amount.Z : Size.Z - amount.Z
                ));
        }

        public AABBox Intersection(AABBox box2)
        {
            float xAxis = this.MaxX - box2.MinX;
            float xAxis2 = box2.MaxX - this.MinX;

            float yAxis = this.MaxY - box2.MinY;
            float yAxis2 = box2.MaxY - this.MinY;

            float zAxis = this.MaxZ - box2.MinZ;
            float zAxis2 = box2.MaxZ - this.MinZ;

            return new AABBox(new Vector3(
                    Math.Max(this.Position.X, box2.Position.X),
                    Math.Max(this.Position.Y, box2.Position.Y),
                    Math.Max(this.Position.Z, box2.Position.Z)),
                new Vector3(
                    Math.Min(xAxis, xAxis2),
                    Math.Min(yAxis, yAxis2),
                    Math.Min(zAxis, zAxis2))
               );
        }


        /*
         * Shortest Direction this has to move to unintersect
         * 
         * */
        [Obsolete("Unused delete soon")]
        public Vector3 SmallestOffset(AABBox box2)
        {
            float xAxis = box2.MinX - this.MaxX;
            float xAxis2 = box2.MaxX - this.MinX;
            float xOffset = Math.Abs(xAxis) < xAxis2 ? xAxis : xAxis2;
            float xOffsetAbs = Math.Abs(xOffset);

            float yAxis = box2.MinY - this.MaxY;
            float yAxis2 = box2.MaxY - this.MinY;
            float yOffset = Math.Abs(yAxis) < yAxis2 ? yAxis : yAxis2;
            float yOffsetAbs = Math.Abs(yOffset);

            float zAxis = box2.MinZ - this.MaxZ;
            float zAxis2 = box2.MaxZ - this.MinZ;
            float zOffset = Math.Abs(zAxis) < zAxis2 ? zAxis : zAxis2;
            float zOffsetAbs = Math.Abs(zOffset);

            if (xOffsetAbs > Size.X && yOffsetAbs > Size.Y && zOffsetAbs > Size.Z)
            {
                return Vector3.Zero;
            }
            else if (xOffsetAbs < yOffsetAbs && xOffsetAbs < zOffsetAbs)
            {
                return Vector3.UnitX * xOffset;
            }
            else if (zOffsetAbs < yOffsetAbs)
            {
                return Vector3.UnitZ * zOffset;
            }
            else
            {
                return Vector3.UnitY * yOffset;
            }
        }

        public bool XIntersection(AABBox box2)
        {
            return this.MaxX > box2.MinX && box2.MaxX > this.MinX;
        }

        public bool YIntersection(AABBox box2)
        {
            return this.MaxY > box2.MinY && box2.MaxY > this.MinY;
        }

        public bool ZIntersection(AABBox box2)
        {
            return this.MaxZ > box2.MinZ && box2.MaxZ > this.MinZ;
        }

        public bool XYIntersection(AABBox box2)
        {
            return XIntersection(box2) && YIntersection(box2);
        }

        public bool XZIntersection(AABBox box2)
        {
            return XIntersection(box2) && ZIntersection(box2);
        }

        public bool YZIntersection(AABBox box2)
        {
            return YIntersection(box2) && ZIntersection(box2);
        }

        public bool Intersects(AABBox box2)
        {
            return XIntersection(box2) && YIntersection(box2) && ZIntersection(box2);
        }

        public float YOffset(AABBox box2, float dy)
        {
            if (XZIntersection(box2))
            {
                if (dy < 0 && box2.MinY >= this.MaxY)
                {
                    float offset = this.MaxY - box2.MinY;
                    if (offset > dy)
                    {
                        dy = offset;
                    }
                }
                else if (dy > 0 && box2.MaxY <= this.MinY)
                {
                    float offset = this.MinY - box2.MaxY;
                    if (offset < dy)
                    {
                        dy = offset;
                    }
                }
            }

            return dy;
        }

        public float ZOffset(AABBox box2, float dz)
        {
            if (XYIntersection(box2))
            {
                if (dz < 0 && box2.MinZ >= this.MaxZ)
                {
                    float offset = this.MaxZ - box2.MinZ;
                    if (offset > dz)
                    {
                        dz = offset;
                    }
                }
                else if (dz > 0 && box2.MaxZ <= this.MinZ)
                {
                    float offset = this.MinZ - box2.MaxZ;
                    if (offset < dz)
                    {
                        dz = offset;
                    }
                }
            }

            return dz;
        }

        public float XOffset(AABBox box2, float dx)
        {
            if (YZIntersection(box2))
            {
                if (dx < 0 && box2.MinX >= this.MaxX)
                {
                    float offset = this.MaxX - box2.MinX;
                    if (offset > dx)
                    {
                        dx = offset;
                    }
                }
                else if (dx > 0 && box2.MaxX <= this.MinX)
                {
                    float offset = this.MinX - box2.MaxX;
                    if (offset < dx)
                    {
                        dx = offset;
                    }
                }
            }

            return dx;
        }
    }
}
