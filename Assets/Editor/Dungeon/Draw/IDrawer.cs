using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Worlds.Generate;

namespace Editor.Dungeon.Draw
{
    public interface IDrawer
    { 
        Action<int, int> DrawAction { get; set; }

        void Draw(int x, int y);
    }
}
