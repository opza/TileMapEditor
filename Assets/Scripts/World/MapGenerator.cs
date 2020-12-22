using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using Util;

namespace Worlds
{
    public class MapGenerator
    {
        World world;
        Dictionary<string, MapSet> mapSets;

        public MapGenerator(World world, string mapSetsPath)
        {
            this.world = world;
            mapSets = ResourceLoader.LoadResources<MapSet>(mapSetsPath);
        }

        // TODO : 현재는 방에서 1개의 Door만 골라서 방을 추가하는 형식, 이후 바꿔야함
        public void Generate(string mapSetName)
        {
            if (!mapSets.ContainsKey(mapSetName))
                return;

            var mapSet = mapSets[mapSetName];

            // TEST : 시작 부분
            var startX = 50;
            var staryY = 50;

            var targetRoom = mapSet[1];
            return;
            //Build(targetRoom, startX, staryY);

            var headerGroup = targetRoom.HeaderGroup;
            for (int i = 0; i < headerGroup.DoorRightHeaders.Count; i++)
            {
                var roomAndDoorRect = mapSet.GetRandomRoomWithMatchedDoor(targetRoom, Room.DoorHeaderGroup.DoorDirection.Right, i);
                var room = roomAndDoorRect.Item1;
                var doorRect = roomAndDoorRect.Item2;

                if (room == null)
                    continue;

                
            }
            
        }

       
        void Build(Room room, int pivotX, int pivotY)
        {
            for (int x = 0; x < room.Width; x++)
            {
                for (int y = 0; y < room.Height; y++)
                {
                    var worldTile = world.GetTile(pivotX + x, pivotY + y);
                    var roomTile = room.GetTile(x, y);

                    worldTile.OnCreatedBlock(roomTile.BlockInfo);
                }
            }
        }

        


    }
}
