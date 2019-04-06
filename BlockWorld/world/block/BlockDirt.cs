using BlockWorld.render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockWorld.world.block
{
    internal class BlockDirt : Block
    {
        public BlockDirt() : base(1) {
        }

        public override string GetTexture(Side s)
        {
            if(s == Side.TOP)
            {
                return "starhide.grass";
            }
            return "starhide.dirt";
        }
    }
}
