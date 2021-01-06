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


        // TODO : 3개 반환하는게 맞나?
        public (Room selectedRoom, RectInt relativeBorderDoorRect, RectInt absoulteRoomRect) GetRoomWithMatchedDoor(RectInt absoluteDoorRect, Room.BorderDoorsGroup.DoorDirection buildDirection)
        {
            var condidateRooms = rooms.RandomSwap();

            foreach (var condidateRoom in condidateRooms)
            {
                var borderDoors = condidateRoom.BorderDoorGroup.GetBorderDoorsForOppositeDirecton(buildDirection);
                foreach (var relativeDoor in borderDoors)
                {
                    if (relativeDoor.width == absoluteDoorRect.width && relativeDoor.height == absoluteDoorRect.height)
                    {
                        var absoluteRoomRect = condidateRoom.rect;
                        absoluteRoomRect.x = absoluteDoorRect.x - relativeDoor.x;
                        absoluteRoomRect.y = absoluteDoorRect.y + relativeDoor.y;

                        return (condidateRoom, relativeDoor, absoluteRoomRect);
                    }
                }
            }

            return (null, new RectInt(0,0,0,0), new RectInt(0, 0, 0, 0));
        }


    }
}