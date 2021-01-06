using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Util;

namespace Worlds.Generate
{
   
    public class MapGenerator
    {
        World world;
        List<LevelMapSet> levelMapSets = new List<LevelMapSet>();

        public static MapGenerator CreateGenerator(World world, List<LevelMapSet> levelMapSets)
        {
            var mapGenerator = new MapGenerator()
            {
                world = world,
                levelMapSets = new List<LevelMapSet>(levelMapSets)
            };

            return mapGenerator;
        }

        MapGenerator() { }
      
        public void Generate(int level)
        {
            if (levelMapSets.Count <= level)
                return;

            var currMap = levelMapSets[level].MapSet.GetRandomItem();
            if (currMap == null)
                return;

            // TODO : Door 후보가 없을 때 방을 생성하도록 정의해야함
            if (levelMapSets[level].BuildedCondidates.Count <= 0)
            {
                if (level == 0)
                {
                    var randomRoom = currMap.GetRandomRoom();
                    var startX = world.Width / 2;
                    var startY = world.Height / 2;

                    AddCondidateDoorInRoom(randomRoom, startX, startY, 0);
                    Build(randomRoom, startX, startY);

                }
                else
                    return;

            }

            // TEST
            var mapSize = 10;//CalculateBuildedMapSize(currMap.Count);
            var currSize = 0;

            var doorCondidates = levelMapSets[level].BuildedCondidates;

            while (mapSize > currSize && doorCondidates.Count > 0)
            {
                var selectedCondidate = doorCondidates.Dequeue();
                var selectedRoomInfo = currMap.GetRoomWithMatchedDoor(selectedCondidate.AbsoluteDoorRect, selectedCondidate.Direction);

                var selectedRoom = selectedRoomInfo.selectedRoom;
                var relatvieDoorRect = selectedRoomInfo.relativeBorderDoorRect;
                var absoluteRoomRect = selectedRoomInfo.absoulteRoomRect;

                if (selectedRoom == null)
                    continue;

                if (!CanBuildWithoutBorder(absoluteRoomRect))
                    continue;

                AddCondidateDoorInRoom(selectedRoom, absoluteRoomRect.x, absoluteRoomRect.y, 0);
                Build(selectedRoom, absoluteRoomRect.x, absoluteRoomRect.y);              

                RemoveDoorTiles(selectedCondidate.Room, selectedCondidate.RelativeDoorRect, selectedCondidate.AbsoluteRoomRect);                
                RemoveDoorTiles(selectedRoom, relatvieDoorRect, absoluteRoomRect);

                currSize++;
            }

        }

        public void Clear()
        {
            levelMapSets.ForEach(mapset => mapset.BuildedCondidates.Clear());
        }

        void AddCondidateDoorInRoom(Room room, int absoluteRoomX, int absoluteRoomY, int level)
        {
            foreach (var borderDoors in room.BorderDoorGroup.AllBorderDoors)
            {
                foreach (var doorRect in borderDoors)
                {
                    var absoluteRoomRect = new RectInt(absoluteRoomX, absoluteRoomY, doorRect.width, doorRect.height);
                    var absoluteDoorRect = ConvertToAbsoluteDoorRect(doorRect, absoluteRoomX, absoluteRoomY);
                    if (!CanBuild(absoluteDoorRect))
                        continue;

                    var buildCondidate = new BuildCondidate(room, absoluteRoomRect, doorRect, absoluteDoorRect, borderDoors.Direction);

                    levelMapSets[level].BuildedCondidates.Enqueue(buildCondidate);
                }
            }
        }    

        #region Build

        void Build(Room room, int absoluteRoomX, int absoluteRoomY)
        {
            for (int x = 0; x < room.Width; x++)
            {
                for (int y = 0; y < room.Height; y++)
                {
                    var worldTile = world.GetTile(absoluteRoomX + x, absoluteRoomY - y);
                    if (worldTile == null)
                        continue;

                    var roomTile = room.GetTile(x, y);

                    if (roomTile == null)
                        continue;

                    

                    worldTile.BuildBlock(roomTile.BlockInfo);
                }
            }
        }  

        bool CanBuildWithoutBorder(RectInt buildRect)
        {
            return CanBuildWithoutBorder(buildRect.x, buildRect.y, buildRect.width, buildRect.height);
        }

        bool CanBuildWithoutBorder(int startX, int startY, int width, int height)
        {
            return CanBuild(startX + 1, startY - 1, width - 2, height - 2);
        }

        bool CanBuild(RectInt buildRect)
        {
            return CanBuild(buildRect.x, buildRect.y, buildRect.width, buildRect.height);
        }

        bool CanBuild(int startX, int startY, int width, int height)
        {
            for (int x = startX; x < startX + width; x++)
            {
                for (int y = startY; y > startY - height; y--)
                {
                    if (world.GetTile(x, y)?.HasBlock != false)
                        return false;
                }
            }

            return true;
        }

        #endregion

        #region Remove

        void RemoveDoorTiles(Room room, RectInt relativeDoorRect, RectInt absoluteRoomRect)
        {
            var doorTiles = room.DoorGroup.GetDoorTiles(relativeDoorRect.x, relativeDoorRect.y);
            Remove(doorTiles, absoluteRoomRect.x, absoluteRoomRect.y);
        }

        void Remove(Room.Tile[] roomTiles, int absoluteRoomX, int absoluteRoomY)
        {
            foreach (var roomtile in roomTiles)
            {
                var absoluteX = absoluteRoomX + roomtile.X;
                var absoluteY = absoluteRoomY - roomtile.Y;

                var worldTile = world.GetTile(absoluteX, absoluteY);
                worldTile?.DestroyBlock();
            }
        }

        #endregion

        RectInt ConvertToAbsoluteDoorRect(RectInt relativePosition, int worldRoomX, int worldRoomY)
        {
            var absolutePos = relativePosition;
            absolutePos.x = worldRoomX + absolutePos.x;
            absolutePos.y = worldRoomY - absolutePos.y;

            return absolutePos;
        }

        int CalculateBuildedMapSize(int roomCount)
        {
            var minMapSize = Mathf.RoundToInt(roomCount * .5f);
            minMapSize = minMapSize <= 0 ? 1 : minMapSize;

            var maxMapSize = Mathf.RoundToInt(roomCount * 1.5f);

            var mapSize = UnityEngine.Random.Range(minMapSize, maxMapSize);
            return mapSize;
        }


        public class LevelMapSet
        {
            int level;

            List<MapSet> mapSet;
            public List<MapSet> MapSet => mapSet;

            Queue<BuildCondidate> buildedCondidates = new Queue<BuildCondidate>();
            public Queue<BuildCondidate> BuildedCondidates => buildedCondidates;

            public LevelMapSet(int level, List<MapSet> mapSet, int levelDepth)
            {
                this.mapSet = mapSet;
            }
        }

        public class BuildCondidate
        {
            public Room Room { get; }


            public RectInt AbsoluteRoomRect { get; }
            public RectInt RelativeDoorRect { get; }
            public RectInt AbsoluteDoorRect { get; }

            public Room.BorderDoorsGroup.DoorDirection Direction { get; }

            public BuildCondidate(Room room, RectInt absoulteRoomRect, RectInt relativeDoorRect, RectInt absoluteDoorRect, Room.BorderDoorsGroup.DoorDirection direction)
            {
                Room = room;
                AbsoluteRoomRect = absoulteRoomRect;
                RelativeDoorRect = relativeDoorRect;
                AbsoluteDoorRect = absoluteDoorRect;
                Direction = direction;
            }
        }


    }
}

