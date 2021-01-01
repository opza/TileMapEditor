using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using Util;

namespace Worlds.Generate
{
    [CreateAssetMenu(fileName = "MapSet", menuName = "DungeonEditor/MapSet")]
    public class MapSet : ScriptableObject
    {
        [SerializeField]
        Room[] rooms;
        public Room[] Room => rooms;

        public Room GetRandomRoom() => rooms.GetRandomItem();

        public Room this[int idx] => rooms[idx];

        public int Count => rooms?.Count() ?? 0;

        public (Room, Rect) GetRandomRoomWithMatchedDoor(Room room, Room.BorderDoorsGroup.DoorDirection direction, int idx)
        {
            var doorRect = room.BorderDoorGroup[direction][idx];

            var condidateRooms = rooms.RandomSwap();

            foreach (var condidateRoom in condidateRooms)
            {
                var doorHeaders = condidateRoom.BorderDoorGroup.GetBorderDoorsForOppositeDirecton(direction);
                if (doorHeaders.Count <= 0)
                    continue;

                foreach (var rect in doorHeaders)
                {
                    if (doorRect.width == rect.width && doorRect.height == rect.height)
                        return (condidateRoom, rect);
                }
            }

            return (null, Rect.zero);
        }

        public (Room, AbsoluteRoomRect) GetRoomWithMatchedDoor(AbsoluteDoorRect doorRect)
        {
            var absoluteDoorRect = doorRect.AbsoluteRect;
            //var condidateRooms = rooms.RandomSwap();
            // TEST
            var condidateRooms = rooms;

            foreach (var condidateRoom in condidateRooms)
            {
                var borderDoors = condidateRoom.BorderDoorGroup.GetBorderDoorsForOppositeDirecton(doorRect.Direction);
                foreach (var relativeDoor in borderDoors)
                {
                    if (relativeDoor.width == absoluteDoorRect.width && relativeDoor.height == absoluteDoorRect.height)
                    {
                        var absoluteRectX = (int)(absoluteDoorRect.x - relativeDoor.x);
                        var absoluteRectY = (int)(absoluteDoorRect.y + relativeDoor.y);

                        var absoluteRoomRect = new AbsoluteRoomRect(condidateRoom.rect, absoluteRectX, absoluteRectY);

                        return (condidateRoom, absoluteRoomRect);
                    }
                }
            }

            return (null, AbsoluteRoomRect.zero);
        }



    }
}