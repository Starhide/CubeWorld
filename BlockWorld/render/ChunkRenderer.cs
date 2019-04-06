
using BlockWorld.util;
using BlockWorld.world;
using BlockWorld.world.block;
using OTKUtilities;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;

namespace BlockWorld.render
{
    internal class ChunkRenderer
    {
        #region Fields

        private static readonly Vector3[] normals = new Vector3[]
        {
            new Vector3(0.0f, 0.0f, 1.0f),
            new Vector3(0.0f, 0.0f, -1.0f),
            new Vector3(1.0f, 0.0f, 0.0f),
            new Vector3(-1.0f, 0.0f, 0.0f),
            new Vector3(0.0f, 1.0f, 0.0f),
            new Vector3(0.0f, -1.0f, 0.0f),
        };

        private GLFloatBuffer[] IVBOs;
        private FloatVertexArrayObject[] VAOs;
        private int[] TriangleCounts;
        public bool Initialized;
        private readonly Chunk chunk;
        private List<float>[] mesh;
        private bool isMeshDirty;

        #endregion

        public ChunkRenderer(Chunk c)
        {
            chunk = c;
        }

        #region Initialization & CleanUp

        public void Initialize(WorldRenderer worldRenderer)
        {
            IVBOs = new GLFloatBuffer[6];
            VAOs = new FloatVertexArrayObject[6];
            TriangleCounts = new int[6];

            for (int i = 0; i < 6; i++)
            {
                VAOs[i] = new FloatVertexArrayObject();
                IVBOs[i] = new GLFloatBuffer(hint: BufferUsageHint.DynamicDraw);

                VAOs[i].Bind();

                GL.BindBuffer(BufferTarget.ArrayBuffer, worldRenderer.VBOs[i]);

                VAOs[i].Attribute(0, 3, false, 5, 0);
                VAOs[i].Attribute(1, 2, false, 5, 3);

                IVBOs[i].Bind();

                VAOs[i].Attribute(2, 3, false, 4, 0);
                VAOs[i].Attribute(3, 1, false, 4, 3);

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                VAOs[i].AttributeDivisor(2, 1);
                VAOs[i].AttributeDivisor(3, 1);

                GL.BindVertexArray(0);
            }

            mesh = new List<float>[6];

            for (int i = 0; i < 6; i++)
            {
                mesh[i] = new List<float>();
            }

            Initialized = true;
        }

        public void CleanUp()
        {            
            Initialized = false;
        }

        #endregion

        #region Rendering

        public void Render()
        {
            if (Initialized && chunk != null && chunk.RenderChunk)
            {
                if (isMeshDirty)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        lock (mesh[i])
                        {
                            TriangleCounts[i] = mesh[i].Count / 4;
                            IVBOs[i].Bind();
                            IVBOs[i].ReplaceData(mesh[i].ToArray());
                            mesh[i].Clear();
                        }
                    }
                    isMeshDirty = false;
                }

                Camera camera = chunk.World.Player.PlayerCamera;

                Vector3 pc = chunk.World.Player.ChunkPosition;
                Vector3 chunkModPos = pc * 16 + MathExt.DirectionMod(pc * 16, chunk.Position, chunk.World.Size * 16);

                if (IsChunkInView(chunk.World.Player.PlayerCamera, chunkModPos))
                {
                    Shader shader = chunk.World.Renderer.blockShader;
                    shader.SetMatrix4("model", Matrix4.CreateTranslation(chunkModPos));

                    if (MathExt.DisDirMod(camera.Position.X, chunk.Position.X, chunk.World.Size.X * 16) < 0)
                        RenderSide(shader, Block.Side.EAST);

                    if (MathExt.DisDirMod(camera.Position.X, chunk.Position.X + 16, chunk.World.Size.X * 16) > 0)
                        RenderSide(shader, Block.Side.WEST);

                    if (MathExt.DisDirMod(camera.Position.Z, chunk.Position.Z, chunk.World.Size.Z * 16) < 0)
                        RenderSide(shader, Block.Side.NORTH);

                    if (MathExt.DisDirMod(camera.Position.Z, chunk.Position.Z + 16, chunk.World.Size.Z * 16) > 0)
                        RenderSide(shader, Block.Side.SOUTH);

                    if (MathExt.DisDirMod(camera.Position.Y, chunk.Position.Y, chunk.World.Size.Y * 16) < 0)
                        RenderSide(shader, Block.Side.TOP);

                    if (MathExt.DisDirMod(camera.Position.Y, chunk.Position.Y + 16, chunk.World.Size.Y * 16) > 0)
                        RenderSide(shader, Block.Side.BOTTOM);

                }
            }
        }

        private void RenderSide(Shader shader, Block.Side side)
        {
            if (TriangleCounts[(int)side] > 0)
            {
                shader.SetVec3("normal", normals[(int)side]);
                VAOs[(int)side].Bind();
                GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, TriangleCounts[(int)side]);
                GL.BindVertexArray(0);
            }
        }

        private bool IsChunkInView(Camera camera, Vector3 position)
        {
            return camera.IsSphereInFrustrum(position + new Vector3(8, 8, 8), 14);
        }

        #endregion

        #region Meshing

        public void CompileMesh()
        {
            isMeshDirty = false;

            for (int i = 0; i < 6; i++)
            {
                lock (mesh[i])
                {
                    mesh[i].Clear();
                }
            }

            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    for (int z = 0; z < 16; z++)
                    {
                        int b = chunk.GetBlockAt(x, y, z);
                        if (b != 0)
                        {
                            Block block = Block.Blocks[b];
                            if (chunk.GetBlockAt(x, y, z - 1) == 0)
                            {
                                lock (mesh[(int)Block.Side.SOUTH])
                                {
                                    mesh[(int)Block.Side.SOUTH].Add(x * 1.0f);
                                    mesh[(int)Block.Side.SOUTH].Add(y * 1.0f);
                                    mesh[(int)Block.Side.SOUTH].Add(z * 1.0f);
                                    mesh[(int)Block.Side.SOUTH].Add(BlockWorld.Atlas.GetImage(block.GetTexture(Block.Side.SOUTH)));
                                }
                            }
                            if (chunk.GetBlockAt(x, y, z + 1) == 0)
                            {
                                lock (mesh[(int)Block.Side.NORTH])
                                {
                                    mesh[(int)Block.Side.NORTH].Add(x * 1.0f);
                                    mesh[(int)Block.Side.NORTH].Add(y * 1.0f);
                                    mesh[(int)Block.Side.NORTH].Add(z * 1.0f);
                                    mesh[(int)Block.Side.NORTH].Add(BlockWorld.Atlas.GetImage(block.GetTexture(Block.Side.NORTH)));
                                }
                            }
                            if (chunk.GetBlockAt(x - 1, y, z) == 0)
                            {
                                lock (mesh[(int)Block.Side.WEST])
                                {
                                    mesh[(int)Block.Side.WEST].Add(x * 1.0f);
                                    mesh[(int)Block.Side.WEST].Add(y * 1.0f);
                                    mesh[(int)Block.Side.WEST].Add(z * 1.0f);
                                    mesh[(int)Block.Side.WEST].Add(BlockWorld.Atlas.GetImage(block.GetTexture(Block.Side.WEST)));
                                }
                            }
                            if (chunk.GetBlockAt(x + 1, y, z) == 0)
                            {
                                lock (mesh[(int)Block.Side.EAST])
                                {
                                    mesh[(int)Block.Side.EAST].Add(x * 1.0f);
                                    mesh[(int)Block.Side.EAST].Add(y * 1.0f);
                                    mesh[(int)Block.Side.EAST].Add(z * 1.0f);
                                    mesh[(int)Block.Side.EAST].Add(BlockWorld.Atlas.GetImage(block.GetTexture(Block.Side.EAST)));
                                }
                            }
                            if (chunk.GetBlockAt(x, y - 1, z) == 0)
                            {
                                lock (mesh[(int)Block.Side.BOTTOM])
                                {
                                    mesh[(int)Block.Side.BOTTOM].Add(x * 1.0f);
                                    mesh[(int)Block.Side.BOTTOM].Add(y * 1.0f);
                                    mesh[(int)Block.Side.BOTTOM].Add(z * 1.0f);
                                    mesh[(int)Block.Side.BOTTOM].Add(BlockWorld.Atlas.GetImage(block.GetTexture(Block.Side.BOTTOM)));
                                }
                            }
                            if (chunk.GetBlockAt(x, y + 1, z) == 0)
                            {
                                lock (mesh[(int)Block.Side.TOP])
                                {
                                    mesh[(int)Block.Side.TOP].Add(x * 1.0f);
                                    mesh[(int)Block.Side.TOP].Add(y * 1.0f);
                                    mesh[(int)Block.Side.TOP].Add(z * 1.0f);
                                    mesh[(int)Block.Side.TOP].Add(BlockWorld.Atlas.GetImage(block.GetTexture(Block.Side.TOP)));
                                }
                            }
                        }
                    }
                }
            }

            isMeshDirty = true;
        }

        #endregion
    }
}
