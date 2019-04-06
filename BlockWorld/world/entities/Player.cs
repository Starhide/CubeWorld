using BlockWorld.render;
using OTKUtilities;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockWorld.world.entities
{
    class Player : Entity
    {

        public Camera PlayerCamera;

        public Player(World world, Matrix4 Projection, Vector3 pos, float pitch, float yaw) : base(world, pos, pitch, yaw)
        {
            PlayerCamera = new Camera(Projection, pos, Up, yaw, pitch);
            BoundingBox.Size = new Vector3(0.5f, 1.68f, 0.5f);
            BoundingBox.Center = Position;
        }

        public override void Update(float deltaTime, World world)
        {
            base.Update(deltaTime, world);
            PlayerCamera.Yaw = this.Yaw;
            PlayerCamera.Pitch = this.Pitch;
            PlayerCamera.Position = this.Position + Vector3.UnitY * 1.6f;
        }

    }
}
