
using BlockWorld.world;
using OTKUtilities;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;

namespace BlockWorld.render
{
    internal class WorldRenderer
    {
        /**
         * 0 z+ NORTH
         * 1 z- SOUTH
         * 2 x+ EAST
         * 3 x- WEST
         * 4 y+ TOP
         * 5 y- BOTTOM
         */
        private static readonly float[][] vertices = new float[][]{
            new float[]
            {
                1.0f,  1.0f,  1.0f,  1.0f, 1.0f,
                0.0f, 0.0f,  1.0f,  0.0f, 0.0f,
                1.0f, 0.0f,  1.0f,  1.0f, 0.0f,
                0.0f, 0.0f,  1.0f,  0.0f, 0.0f,
                1.0f,  1.0f,  1.0f,  1.0f, 1.0f,
                0.0f,  1.0f,  1.0f,  0.0f, 1.0f
            },
            new float[]
            {
                0.0f, 0.0f, 0.0f,  0.0f, 0.0f,
                1.0f, 1.0f, 0.0f,  1.0f, 1.0f,
                1.0f, 0.0f, 0.0f,  1.0f, 0.0f,
                0.0f, 0.0f, 0.0f,  0.0f, 0.0f,
                0.0f, 1.0f, 0.0f,  0.0f, 1.0f,
                1.0f, 1.0f, 0.0f,  1.0f, 1.0f
            },
            new float[]
            {
                1.0f, 0.0f, 0.0f,  0.0f, 1.0f,
                1.0f,  1.0f, 0.0f,  1.0f, 1.0f,
                1.0f,  1.0f,  1.0f,  1.0f, 0.0f,
                1.0f, 0.0f, 0.0f,  0.0f, 1.0f,
                1.0f,  1.0f,  1.0f,  1.0f, 0.0f,
                1.0f, 0.0f,  1.0f,  0.0f, 0.0f
            },
            new float[]
            {
                0.0f, 0.0f, 0.0f,  0.0f, 1.0f,
                0.0f,  1.0f,  1.0f,  1.0f, 0.0f,
                0.0f,  1.0f, 0.0f,  1.0f, 1.0f,
                0.0f, 0.0f, 0.0f,  0.0f, 1.0f,
                0.0f, 0.0f,  1.0f,  0.0f, 0.0f,
                0.0f,  1.0f,  1.0f,  1.0f, 0.0f
            },
            new float[]
            {
                0.0f,  1.0f, 0.0f,  0.0f, 1.0f,
                1.0f,  1.0f,  1.0f,  1.0f, 0.0f,
                1.0f,  1.0f, 0.0f,  1.0f, 1.0f,
                0.0f,  1.0f, 0.0f,  0.0f, 1.0f,
                0.0f,  1.0f,  1.0f,  0.0f, 0.0f,
                1.0f,  1.0f,  1.0f,  1.0f, 0.0f
            },
            new float[]
            {
                0.0f, 0.0f, 0.0f,  0.0f, 1.0f,
                1.0f, 0.0f, 0.0f,  1.0f, 1.0f,
                1.0f, 0.0f,  1.0f,  1.0f, 0.0f,
                0.0f, 0.0f, 0.0f,  0.0f, 1.0f,
                1.0f, 0.0f,  1.0f,  1.0f, 0.0f,
                0.0f, 0.0f,  1.0f,  0.0f, 0.0f
            }
        };

        private readonly World world;
        public Shader blockShader;
        public readonly int[] VBOs;

        public WorldRenderer(World world)
        {
            this.world = world;

            blockShader = new Shader("assets/shaders/block.vert", "assets/shaders/blockFrag.frag");
            blockShader.Use();
            blockShader.SetInt("texture0", 0);

            VBOs = new int[6];

            for (int i = 0; i < 6; i++)
            {
                VBOs[i] = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBOs[i]);
                GL.BufferData(BufferTarget.ArrayBuffer, vertices[i].Length * sizeof(float), vertices[i], BufferUsageHint.StaticDraw);
            }
        }

        public void SetPlayer()
        {
            blockShader.SetMatrix4("projection", world.Player.PlayerCamera.Projection);
        }

        public void Render(double Time)
        {
            Vector3 light = new Vector3(100.0f * (float)Math.Sin((Time / 20) * 6.283) + 100.0f, 200, 100.0f * (float)Math.Cos((Time / 20) * 6.283) + 100.0f);

            /*Matrix4 lightProj = Matrix4.CreateOrthographic(100.0f, 100.0f, 1.0f, 200.0f);
            Matrix4 lightView = Matrix4.LookAt(100.0f, 200.0f, 100.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f);
            Matrix4 lightSpaceSun = lightProj * lightView;*/

            blockShader.Use();
            blockShader.SetMatrix4("view", world.Player.PlayerCamera.View);
            blockShader.SetVec3("sunLightPos", new Vector3(100.0f, 200.0f, 100.0f));

            GL.ActiveTexture(TextureUnit.Texture0);
            BlockWorld.Atlas.Bind();
        }
    }
}
