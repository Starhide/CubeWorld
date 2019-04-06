namespace BlockWorld.world.block
{
    internal class Block
    {
        public static Block[] Blocks = new Block[]
        {
            new Block(0),
            new BlockDirt()
        };

        public enum Side
        {
            NORTH,
            SOUTH,
            EAST,
            WEST,
            TOP,
            BOTTOM
        }

        public readonly ushort BlockID;

        public Block(ushort blockID)
        {
            BlockID = blockID;
        }

        public bool IsSideSolid(Side s)
        {
            if (BlockID == 0)
                return false;
            return true;
        }

        public virtual string GetTexture(Side s)
        {
            return null;
        }

    }
}
