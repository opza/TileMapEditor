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
        public (Room selectedRoom, Rect relativeBorderDoorRect, Rect absoulteRoomRect) GetRoomWithMatchedDoor(Rect absoluteDoorRect, Room.BorderDoorsGroup.DoorDirection buildDirection)
        {
            //var condidateRooms = rooms.RandomSwap();
            // TEST
            var condidateRooms = rooms;

            foreach (var condidateRoom in condidateRooms)
            {
                var borderDoors = condidateRoom.BorderDoorGroup.GetBorderDoorsForOppositeDirecton(buildDirection);
                foreach (var relativeDoor in borderDoors)
                {
                    if (relativeDoor.width == absoluteDoorRect.width && relativeDoor.height == absoluteDoorRect.height)
                    {
                        var absoluteRoomRect = new Rect(condidateRoom.rect);
                        absoluteRoomRect.x = (int)(absoluteDoorRect.x - relativeDoor.x);
                        absoluteRoomRect.y = (int)(absoluteDoorRect.y + relativeDoor.y);

                        return (condidateRoom, relativeDoor, absoluteRoomRect);
                    }
                }
            }

            return (null, Rect.zero, Rect.zero);
        }


    }
}