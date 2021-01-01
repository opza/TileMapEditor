using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Worlds
{
    public class Block
    {
        BlockInfo blockInfo;
        public BlockInfo BlockInfo => blockInfo;

        public byte Mask { get; set; }

        public static Block Create(BlockInfo blockInfo)
        {
            var block = new Block(blockInfo);
            return block;
        }


        Block(BlockInfo blockInfo)
        {
            this.blockInfo = blockInfo;
        }
        
    }
}
