using OTKUtilities;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockWorld.render
{
    class Frustrum
    {
        private readonly Camera Camera;
        private readonly Vector4[] Planes;

        public Frustrum(Camera camera)
        {
            this.Camera = camera;
            Planes = new Vector4[6];
        }

        public bool IsPointInFrustrum(Vector3 point)
        {
            for (int i = 0; i < 6; i++)
            {
                if (DistanceToPlane(i, point) < 0)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsSphereInFrustrum(Vector3 center, float radius)
        {
            //UpdateFrustrum();
            float distance;

            for (int i = 0; i < 6; i++)
            {
                distance = DistanceToPlane(i, center);
                if (distance < -radius)
                {
                    return false;
                }
                else if (distance < radius)
                    ;
            }
            return true;
        }

        public void UpdateFrustrum()
        {
            Matrix4 mp = Camera.View * Camera.Projection;

            SetPlaneNormal(0,
                                 mp[0, 2] + mp[0, 3],
                                 mp[1, 2] + mp[1, 3],
                                 mp[2, 2] + mp[2, 3],
                                 mp[3, 2] + mp[3, 3]);
            SetPlaneNormal(1,
                                -mp[0, 2] + mp[0, 3],
                                -mp[1, 2] + mp[1, 3],
                                -mp[2, 2] + mp[2, 3],
                                -mp[3, 2] + mp[3, 3]);
            SetPlaneNormal(2,
                                 mp[0, 1] + mp[0, 3],
                                 mp[1, 1] + mp[1, 3],
                                 mp[2, 1] + mp[2, 3],
                                 mp[3, 1] + mp[3, 3]);
            SetPlaneNormal(3,
                                -mp[0, 1] + mp[0, 3],
                                -mp[1, 1] + mp[1, 3],
                                -mp[2, 1] + mp[2, 3],
                                -mp[3, 1] + mp[3, 3]);
            SetPlaneNormal(4,
                                 mp[0, 0] + mp[0, 3],
                                 mp[1, 0] + mp[1, 3],
                                 mp[2, 0] + mp[2, 3],
                                 mp[3, 0] + mp[3, 3]);
            SetPlaneNormal(5,
                                -mp[0, 0] + mp[0, 3],
                                -mp[1, 0] + mp[1, 3],
                                -mp[2, 0] + mp[2, 3],
                                -mp[3, 0] + mp[3, 3]);
        }

        private void SetPlaneNormal(int i, float a, float b, float c, float d)
        {
            Planes[i] = new Vector4(a, b, c, 0);
            float l = Planes[i].Length;
            Planes[i].Normalize();
            Planes[i].W = d / l;
        }

        private float DistanceToPlane(int p, Vector3 v)
        {
            return Vector3.Dot(Planes[p].Xyz, v) + Planes[p].W;
        }
    }
}
