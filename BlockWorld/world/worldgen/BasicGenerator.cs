
using BlockWorld.util.indexers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockWorld.world.worldgen
{
    class BasicGenerator
    {
        public static void GenerateChunkAt(ChunkIndex index, ref Chunk chunk)
        {
            for (int x = 0; x < 16; x++)
            {
                for (int z = 0; z < 16; z++)
                {
                    for (int y = 0; y < 16; y++)
                    {
                        float yf = (index.Y * 16 + y) / 255.0f;
                        double range = 10;
                        double f = 0.1 * Math.Max(0, yf) + (0.95 / (range / 256)) * Math.Min(range / 256, Math.Max(0, yf - 0.5 + range / 512));
                        if (Perlin.perlin(index.X + x / 16.0f, index.Y + y / 16.0f, index.Z + z / 16.0f) > f)
                        {
                            chunk.SetBlockAt(x, y, z, 1);
                        }
                        else
                        {
                            chunk.SetBlockAt(x, y, z, 0);
                        }
                    }
                }
            }
        }
    }
}
