using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Worlds.Generate;

namespace Editor.Dungeon.Draw
{
    public class DotDrawer : IDrawer
    {

        public Action<int, int> DrawAction { get; set; }

        public DotDrawer() { }
        public DotDrawer(Action<int,int> drawAction)
        {
            DrawAction = drawAction;
        }

        public void Draw(int x, int y)
        {
            DrawAction?.Invoke(x, y);
        }
        
    }
}
