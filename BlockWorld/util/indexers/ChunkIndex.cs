using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockWorld.util.indexers
{
    public class ChunkIndex : Vector3IRModded
    {
        public ChunkIndex(int x, int y, int z, WorldSize worldSize) : base(x,y,z , worldSize)
        {

        }

        public ChunkIndex(Vector3 worldCoordinates, WorldSize size)
            : base((int)Math.Floor(worldCoordinates.X / 16), (int)Math.Floor(worldCoordinates.Y / 16), (int)Math.Floor(worldCoordinates.Z / 16), size)
        { }

        public float Distance(WorldSize size, ChunkIndex target)
        {
            int x = MathExt.DisDirMod(X, target.X, size.X);
            int y = MathExt.DisDirMod(Y, target.Y, size.Y);
            int z = MathExt.DisDirMod(Z, target.Z, size.Z);

            return (float)Math.Sqrt((x * x) + (y * y) + (z * z));
        }

        
    }
}
