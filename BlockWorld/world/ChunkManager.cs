
using BlockWorld.util;
using BlockWorld.util.indexers;
using BlockWorld.world.worldgen;
using Priority_Queue;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlockWorld.world
{
    internal class ChunkManager
    {
        private readonly World world;

        public ConcurrentDictionary<ChunkIndex, Chunk> Chunks;
        private readonly ObjectPool<Chunk> chunkPool;
        private SimplePriorityQueue<ChunkIndex> chunkUpdateQueue;
        private SimplePriorityQueue<ChunkIndex> chunkLoadQueue;
        private List<ChunkIndex> chunkLoadingList;
        private SimplePriorityQueue<ChunkIndex> chunkUnloadQueue;

        public ChunkManager(World world)
        {
            this.world = world;
            Chunks = new ConcurrentDictionary<ChunkIndex, Chunk>();
            chunkPool = new ObjectPool<Chunk>(() => { return new Chunk(world); }, World.LoadDistance* World.LoadDistance* World.LoadDistance);

            chunkUpdateQueue = new SimplePriorityQueue<ChunkIndex>();
            chunkLoadQueue = new SimplePriorityQueue<ChunkIndex>();
            chunkUnloadQueue = new SimplePriorityQueue<ChunkIndex>();
            chunkLoadingList = new List<ChunkIndex>();
        }

        private void AddChunk(Chunk c)
        {
            if (Chunks.TryAdd(c.ChunkID, c))
            {
                if (Chunks.TryGetValue(new ChunkIndex(c.ChunkID.X + 1, c.ChunkID.Y, c.ChunkID.Z, c.World.Size), out Chunk neighbor))
                {
                    neighbor.Compile();
                }

                if (Chunks.TryGetValue(new ChunkIndex(c.ChunkID.X - 1, c.ChunkID.Y, c.ChunkID.Z, c.World.Size), out Chunk neighbor2))
                {
                    neighbor2.Compile();
                }

                if (Chunks.TryGetValue(new ChunkIndex(c.ChunkID.X, c.ChunkID.Y + 1, c.ChunkID.Z, c.World.Size), out Chunk neighbor3))
                {
                    neighbor3.Compile();
                }

                if (Chunks.TryGetValue(new ChunkIndex(c.ChunkID.X, c.ChunkID.Y - 1, c.ChunkID.Z, c.World.Size), out Chunk neighbor4))
                {
                    neighbor4.Compile();
                }

                if (Chunks.TryGetValue(new ChunkIndex(c.ChunkID.X, c.ChunkID.Y, c.ChunkID.Z + 1, c.World.Size), out Chunk neighbor5))
                {
                    neighbor5.Compile();
                }

                if (Chunks.TryGetValue(new ChunkIndex(c.ChunkID.X, c.ChunkID.Y, c.ChunkID.Z - 1, c.World.Size), out Chunk neighbor6))
                {
                    neighbor6.Compile();
                }
            }
        }

        public void UpdateChunks(int n)
        {
            for (int i = 0; i < Math.Min(n, chunkUpdateQueue.Count); i++)
            {
                ChunkIndex ci = chunkUpdateQueue.Dequeue();
                if (Chunks.TryGetValue(ci, out Chunk chunk))
                {
                    chunk.Update();
                }
                else
                {
                    i--;
                }
            }
        }

        public void QueueChunkUpdates(int loadDistance)
        {
            chunkUpdateQueue.Clear();

            foreach (ChunkIndex i in Chunks.Keys)
            {
                float d = i.X * i.X + i.Y * i.Y + i.Z * i.Z;
                chunkUpdateQueue.Enqueue(i, d);
            }
        }

        public void LoadChunks(int n, bool async = true)
        {
            int s = chunkLoadingList.Count;
            for (int i = 0; i < Math.Min(n - s, chunkLoadQueue.Count); i++)
            {
                ChunkIndex ci = chunkLoadQueue.Dequeue();
                if (Chunks.ContainsKey(ci))
                {
                    i--;
                }
                else
                {
                    if (world.Player.ChunkID.Distance(world.Size, ci) < World.LoadDistance)
                    {
                        Chunk c = chunkPool.GetObject();
                        c.ChunkID = ci;

                        chunkLoadingList.Add(ci);

                        if (async)
                        {
                            Task t = Task.Factory.StartNew(() =>
                            {
                                LoadChunk(ci, c);
                            });
                        }
                        else
                        {
                            LoadChunk(ci, c);
                        }
                    }
                    else
                    {
                        i--;
                    }
                }
            }
        }

        private void LoadChunk(ChunkIndex index, Chunk c)
        {
            BasicGenerator.GenerateChunkAt(index, ref c);
            c.Compile();
            c.RenderChunk = true;
            AddChunk(c);
            lock (chunkLoadingList)
            {
                chunkLoadingList.Remove(index);
            }
        }

        public void QueueAddChunks(int loadDistance)
        {
            chunkLoadQueue.Clear();

            float ld2 = loadDistance * loadDistance;
            ChunkIndex playerLoc = world.Player.ChunkID;

            for (int y = -loadDistance; y < loadDistance; y++)
            {
                for (int x = -loadDistance; x < loadDistance; x++)
                {
                    for (int z = -loadDistance; z < loadDistance; z++)
                    {
                        float d = x * x + y * y + z * z;
                        if (d < ld2)
                        {
                            ChunkIndex index = new ChunkIndex(playerLoc.X + x, playerLoc.Y + y, playerLoc.Z + z, world.Size);
                            lock (chunkLoadingList)
                            {
                                if (!Chunks.ContainsKey(index) && !chunkLoadingList.Contains(index))
                                {
                                    chunkLoadQueue.Enqueue(index, d);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void UnloadChunks(int n)
        {
            for (int i = 0; i < Math.Min(n, chunkUnloadQueue.Count); i++)
            {
                ChunkIndex ci = chunkUnloadQueue.Dequeue();
                if (!Chunks.ContainsKey(ci))
                {
                    i--;
                }
                else
                {
                    if (Chunks.TryRemove(ci, out Chunk chunk))
                    {
                        if (!chunkPool.PutObject(chunk))
                        {
                            chunk.CleanUp();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Failed to remove chunk");
                    }
                }
            }
        }

        public void QueueRemoveChunks(int loadDistance)
        {
            chunkUnloadQueue.Clear();

            ChunkIndex playerLoc = world.Player.ChunkID;

            foreach (Chunk c in Chunks.Values.ToList())
            {
                float d = playerLoc.Distance(world.Size, c.ChunkID);
                if (d > loadDistance)
                {
                    chunkUnloadQueue.Enqueue(c.ChunkID, 64 / (d+1));
                }
            }
        }

        internal string GetDebugString()
        {
            return $"Chunks: {Chunks.Count}\n" +
                $"Chunk Pool: {chunkPool.Count}\n" +
                $"LoadQueue: {chunkLoadQueue.Count}\n" +
                $"Loading: {chunkLoadingList.Count}\n" +
                $"Unload: {chunkUnloadQueue.Count}\n" +
                $"Update: {chunkUpdateQueue.Count}";
        }
    }
}
