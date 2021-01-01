using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Worlds.Generate
{
    public class AbsoluteDoorRect
    {
        Rect relativeRect;
        Rect absoluteRect;
        public Rect AbsoluteRect => absoluteRect;

        Room.BorderDoorsGroup.DoorDirection direction;
        public Room.BorderDoorsGroup.DoorDirection Direction => direction;

        public AbsoluteDoorRect(Rect relativeDoorRect, int worldRoomX, int worldRoomY, Room.BorderDoorsGroup.DoorDirection direction)
        {
            relativeRect = relativeDoorRect;
            absoluteRect = ConvertToAbsoluteDoorRect(relativeDoorRect, worldRoomX, worldRoomY);
            
            this.direction = direction;
        }

        Rect ConvertToAbsoluteDoorRect(Rect relativePosition, int worldRoomX, int worldRoomY)
        {
            var absolutePos = new Rect(relativePosition);
            absolutePos.x = worldRoomX + absolutePos.x;
            absolutePos.y = worldRoomY - absolutePos.y;

            return absolutePos;
        }

        
    }
}
