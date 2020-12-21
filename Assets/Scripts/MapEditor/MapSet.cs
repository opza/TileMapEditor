using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using Util;

[CreateAssetMenu(fileName = "MapSet", menuName = "DungeonEditor/MapSet")]
public class MapSet : ScriptableObject
{
    [SerializeField]
    Room[] rooms;
    public Room[] Room => rooms;

    public Room this[int idx] => rooms[idx];

    public int Count => rooms?.Count() ?? 0;

    public Room GetRandomRoom()
    {
        UnityEngine.Random.InitState(Environment.TickCount);
        var ranIdx = UnityEngine.Random.Range(0, Count);
      
        return rooms[ranIdx];
    }

    public (Room, Rect) GetRandomRoomWithMatchedDoor(Room room, Room.DoorHeaderGroup.DoorDirection direction, int idx)
    {
        var doorRect = room.HeaderGroup[direction][idx];

        var condidateRooms = rooms.RandomSwap();

        foreach (var condidateRoom in condidateRooms)
        {
            var doorHeaders = condidateRoom.HeaderGroup.GetDoorHeadersForOppositeDirecton(direction);
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

   

}