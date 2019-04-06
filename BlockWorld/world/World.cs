
using BlockWorld.render;
using BlockWorld.util.collision;
using BlockWorld.util.indexers;
using BlockWorld.world.block;
using BlockWorld.world.entities;
using OpenTK;
using System;
using System.Collections.Generic;

namespace BlockWorld.world
{
    internal class World
    {
        public static int LoadDistance = 8;

        public ChunkManager ChunkManager;
        public readonly WorldSize Size;
        public readonly int Seed;
        public readonly WorldRenderer Renderer;

        public Player Player { get => player; set { player = value; Renderer.SetPlayer(); } }
        private ChunkIndex lastPlayerChunk;
        private Player player;

        public World(int sx, int sy, int sz)
        {
            Size = new WorldSize(sx, sy, sz);

            Random rand = new Random();
            Seed = rand.NextDouble().GetHashCode();

            lastPlayerChunk = new ChunkIndex(Vector3.Zero, Size);

            ChunkManager = new ChunkManager(this);
            Renderer = new WorldRenderer(this);
        }

        public void PreLoad()
        {
            int d = LoadDistance / 2;

            ChunkManager.QueueAddChunks(LoadDistance);

            ChunkManager.LoadChunks(d * d * d, false);
        }

        public void Update(float time)
        {
            Player.Update(time, this);

            ChunkUpdates();
        }

        public void Render(double time)
        {
            Renderer.Render(time);

            foreach (Chunk c in ChunkManager.Chunks.Values)
            {
                c.Render();
            }
        }

        public void ChunkUpdates()
        {
            ChunkIndex currPlayerChunk = Player.ChunkID;

            if (currPlayerChunk != lastPlayerChunk)
            {
                ChunkManager.QueueAddChunks(LoadDistance);
                ChunkManager.QueueRemoveChunks(LoadDistance + 1);
                ChunkManager.QueueChunkUpdates(LoadDistance);
            }

            ChunkManager.UnloadChunks(25);
            ChunkManager.LoadChunks(10);
            ChunkManager.UpdateChunks(25);

            lastPlayerChunk = currPlayerChunk;
        }

        public Block GetBlockAt(Vector3 position)
        {
            ChunkIndex chunkPos = new ChunkIndex(position, Size);

            bool result = ChunkManager.Chunks.TryGetValue(chunkPos, out Chunk c);

            if (!result)
            {
                return new Block(1);
            }

            int blockPosX = (int)(position.X % 16 + 16) % 16;
            int blockPosY = (int)(position.Y % 16 + 16) % 16;
            int blockPosZ = (int)(position.Z % 16 + 16) % 16;
            return new Block((ushort)c.GetBlockAt(blockPosX, blockPosY, blockPosZ));
        }

        public bool SetBlockAt(Vector3 position, Block b)
        {
            ChunkIndex chunkPos = new ChunkIndex(position, Size);

            //Monitor.Enter(ChunkLock);
            bool result = ChunkManager.Chunks.TryGetValue(chunkPos, out Chunk c);
            //Monitor.Exit(ChunkLock);
            if (!result)
            {
                return false;
            }

            int blockPosX = (int)(position.X % 16 + 16) % 16;
            int blockPosY = (int)(position.Y % 16 + 16) % 16;
            int blockPosZ = (int)(position.Z % 16 + 16) % 16;
            return c.SetBlockAt(blockPosX, blockPosY, blockPosZ, b.BlockID);
        }

        public List<AABBox> GetCollisionBoxesAt(Vector3 position)
        {
            List<AABBox> boxes = new List<AABBox>();
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        if (GetBlockAt(position + new Vector3(x, y, z)).BlockID > 0)
                        {
                            boxes.Add(new AABBox(position + new Vector3(x, y, z)));
                        }
                    }
                }
            }
            return boxes;
        }

        public List<AABBox> GetCollisionBoxesIntersecting(AABBox box)
        {
            List<AABBox> boxes = new List<AABBox>();

            for (int y = (int)Math.Floor(box.MinY) - 1; y <= Math.Ceiling(box.MaxY) + 1; y++)
            {
                for (int x = (int)Math.Floor(box.MinX) - 1; x <= Math.Ceiling(box.MaxX) + 1; x++)
                {
                    for (int z = (int)Math.Floor(box.MinZ) - 1; z <= Math.Ceiling(box.MaxZ) + 1; z++)
                    {
                        if (GetBlockAt(new Vector3(x, y, z)).BlockID > 0)
                        {
                            boxes.Add(new AABBox(new Vector3(x, y, z)));
                        }
                    }
                }
            }

            return boxes;
        }


    }
}
