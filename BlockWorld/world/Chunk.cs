using System;
using BlockWorld.render;
using BlockWorld.util.indexers;
using OpenTK;

namespace BlockWorld.world
{
    internal class Chunk
    {
        #region Properties and Fields

        private readonly int[,,] blocks = new int[16, 16, 16];
        private ChunkIndex index;
        private readonly ChunkRenderer renderer;
        public World World;

        public bool IsChunkDirty;
        public bool RenderChunk = false;

        public Vector3 Position { get; private set; }

        public ChunkIndex ChunkID
        {
            get => index;
            set
            {
                index = value;
                Position = new Vector3(index.X * 16, index.Y * 16, index.Z * 16);
            }
        }

        #endregion

        public Chunk(World world)
        {
            this.World = world;
            this.renderer = new ChunkRenderer(this);
            renderer.Initialize(World.Renderer);
        }

        public Chunk(World world, ChunkIndex index) : this(world)
        {
            ChunkID = index;
        }

        public void Update()
        {

        }

        public void Render()
        {
            if (IsChunkDirty)
                Compile();

            if (RenderChunk && renderer.Initialized)
                renderer.Render();
        }

        public void Compile()
        {
            renderer.CompileMesh();
            IsChunkDirty = false;
        }

        public int GetBlockAt(int x, int y, int z)
        {
            if (x < 0 || x > 15 || y < 0 || y > 15 || z < 0 || z > 15)
            {
                return World.GetBlockAt(Position + new Vector3(x, y, z)).BlockID;
            }

            lock (blocks)
            {
                return blocks[x, y, z];
            }
        }

        public bool SetBlockAt(int x, int y, int z, int blockID)
        {
            if (x < 0 || x > 15 || y < 0 || y > 15 || z < 0 || z > 15)
            {
                return false;
            }
            lock (blocks)
            {
                blocks[x, y, z] = blockID;
            }
            IsChunkDirty = true;
            return true;
        }

        internal void CleanUp()
        {
            renderer.CleanUp();
        }
    }
}
