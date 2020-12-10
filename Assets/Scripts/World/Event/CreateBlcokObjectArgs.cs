using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worlds
{
    public class CreateBlcokObjectArgs : EventArgs
    {
        int x;
        public int X => x;

        int y;
        public int Y => y;

        Block block;
        public Block Block => block;

        public CreateBlcokObjectArgs(int x, int y, Block block)
        {
            this.x = x;
            this.y = y;
            this.block = block;
        }
    }
}
