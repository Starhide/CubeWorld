using BlockWorld.util;
using BlockWorld.util.collision;
using BlockWorld.util.indexers;
using OpenTK;
using System;
using System.Collections.Generic;

namespace BlockWorld.world.entities
{
    internal class Entity
    {
        #region Fields & Properties

        private World world;
        protected float pitch;
        protected float yaw;
        public bool UseGravity;

        public Vector3 Velocity { get; set; }
        public Vector3 Position { get; protected set; }
        public Quaternion Rotation { get; protected set; }
        public AABBox BoundingBox { get; protected set; }

        public Vector3 ChunkPosition => new Vector3(
                     MathExt.AbsMod(MathExt.FloorToInt(Position.X / 16.0f), world.Size.X),
                     MathExt.AbsMod(MathExt.FloorToInt(Position.Y / 16.0f), world.Size.Y),
                     MathExt.AbsMod(MathExt.FloorToInt(Position.Z / 16.0f), world.Size.Z));

        public ChunkIndex ChunkID => new ChunkIndex(Position, world.Size);

        #endregion

        #region Constructors

        public Entity(World world) : this(world, Vector3.Zero, 0, 0) { }

        public Entity(World world, Vector3 pos) : this(world, pos, 0, 0) { }

        public Entity(World world, Vector3 pos, float pitch, float yaw)
        {
            this.world = world;
            Position = pos;
            this.yaw = yaw;
            this.pitch = pitch;
            BoundingBox = new AABBox(Vector3.Zero)
            {
                CenterXZ = Position
            };
            UseGravity = true;
            UpdateRotation();
        }

        #endregion
             
        #region Movement & Rotation

        public Vector3 Forward
        {
            get
            {
                float yawr = MathHelper.DegreesToRadians(Yaw);
                float pitchr = MathHelper.DegreesToRadians(Pitch);
                Vector3 f = new Vector3(
                    (float)(Math.Cos(yawr) * Math.Cos(pitchr)),
                    (float)Math.Sin(pitchr),
                    (float)(Math.Sin(yawr) * Math.Cos(pitchr))
                );
                f.Normalize();
                return f;
            }
        }
        public Vector3 Right { get { Vector3 r = Vector3.Cross(Forward, Vector3.UnitY); r.Normalize(); return r; } }
        public Vector3 Up { get { Vector3 u = Vector3.Cross(Right, Forward); u.Normalize(); return u; } }

        public Vector3 WForward
        {
            get
            {
                float yawr = MathHelper.DegreesToRadians(Yaw);
                Vector3 f = new Vector3(
                    (float)(Math.Cos(yawr)),
                    0,
                    (float)(Math.Sin(yawr))
                );
                f.Normalize();
                return f;
            }
        }
        public Vector3 WRight { get { Vector3 r = Vector3.Cross(Forward, Vector3.UnitY); r.Normalize(); return r; } }


        public float Pitch
        {
            get => pitch;
            set
            {
                pitch = Math.Min(89.99f, Math.Max(-89.99f, value));
                UpdateRotation();
            }
        }

        public float Yaw
        {
            get => yaw;
            set
            {
                yaw = value;
                UpdateRotation();
            }
        }

        public void MoveTo(float x, float y, float z)
        {
            Position = new Vector3(MathExt.AbsMod(x, world.Size.X * 16),
                                   MathExt.AbsMod(y, world.Size.Y * 16),
                                   MathExt.AbsMod(z, world.Size.Z * 16));
            BoundingBox.CenterXZ = Position;
        }

        public void MoveBy(World world, Vector3 vector)
        {
            MoveBy(world, vector.X, vector.Y, vector.Z);
        }

        public void MoveBy(World world, float dx, float dy, float dz)
        {
            List<AABBox> boxes = world.GetCollisionBoxesIntersecting(BoundingBox.Expand(new Vector3(dx, dy, dz)));

            float ox = dx, oy = dy, oz = dz;

            if (dy != 0.0f)
            {
                foreach (AABBox box in boxes)
                {
                    dy = box.YOffset(BoundingBox, dy);
                }

                BoundingBox.Position = MathExt.AbsModVector(BoundingBox.Position + Vector3.UnitY * dy, world.Size * 16);
            }

            if (dx != 0.0f)
            {
                foreach (AABBox box in boxes)
                {
                    dx = box.XOffset(BoundingBox, dx);
                }
                BoundingBox.Position = MathExt.AbsModVector(BoundingBox.Position + Vector3.UnitX * dx, world.Size * 16);
            }

            if (dz != 0.0f)
            {
                foreach (AABBox box in boxes)
                {

                    dz = box.ZOffset(BoundingBox, dz);
                }
                BoundingBox.Position = MathExt.AbsModVector(BoundingBox.Position + Vector3.UnitZ * dz, world.Size * 16);
            }
            Velocity = Velocity * new Vector3(dx == ox ? 1.0f : 0.0f,
                dy == oy ? 1.0f : 0.0f,
                dz == oz ? 1.0f : 0.0f);
            Position = MathExt.AbsModVector(BoundingBox.CenterXZ, world.Size * 16);
            BoundingBox.CenterXZ = Position;
        }

        public void LookAt(Vector3 target)
        {
            pitch = Math.Min(89.99f, Math.Max(-89.99f, MathHelper.DegreesToRadians(Vector3.CalculateAngle(Vector3.UnitY, target))));
            yaw = MathHelper.DegreesToRadians(Vector3.CalculateAngle(Vector3.UnitZ, target));
        }

        private void UpdateRotation()
        {
            Rotation = Quaternion.FromEulerAngles(0, pitch, yaw);
        }

        #endregion

        public virtual void Update(float deltaTime, World world)
        {
            if (UseGravity)
            {
                Velocity -= Vector3.UnitY * 9.8f * deltaTime;
                MoveBy(world, Velocity * deltaTime);
                //CheckCollisionWithWorld(world);
            }
        }
    }
}
