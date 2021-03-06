﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Worlds
{
    public class CreateBlcokObjectArgs : EventArgs
    {
        public int X { get; }

        public int Y { get; }

        public Block Block { get; }

        public CreateBlcokObjectArgs(int x, int y, Block block)
        {
            X = x;
            Y = y;
            Block = block;
        }
    }
}
