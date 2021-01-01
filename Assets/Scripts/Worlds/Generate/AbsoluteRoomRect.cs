using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Worlds.Generate
{
    public class AbsoluteRoomRect
    {
        Rect relativeRect;
        Rect absoluteRect;
        public Rect AbsoluteRect => absoluteRect;

        public static AbsoluteRoomRect zero => new AbsoluteRoomRect()
        {
            relativeRect = Rect.zero,
            absoluteRect = Rect.zero
        };

        public AbsoluteRoomRect(Rect relativeRoomRect, int worldRoomX, int worldRoomY)
        {
            relativeRect = relativeRoomRect;
            absoluteRect = ConvertToAbsoluteDoorRect(relativeRoomRect, worldRoomX, worldRoomY);
        }

        AbsoluteRoomRect() { }

        Rect ConvertToAbsoluteDoorRect(Rect relativePosition, int worldRoomX, int worldRoomY)
        {
            var absolutePos = new Rect(relativePosition);
            absolutePos.x = worldRoomX;
            absolutePos.y = worldRoomY;

            return absolutePos;
        }
    }
}
