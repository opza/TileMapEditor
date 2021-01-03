using System;
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
                    // TEST
                    //var randomRoom = currMap.GetRandomRoom();
                    var randomRoom = currMap[0];
                    var startX = world.Width / 2;
                    var startY = world.Height / 2;

                    Build(randomRoom, startX, startY);

                    foreach (var borderDoors in randomRoom.BorderDoorGroup.AllBorderDoors)
                    {
                        foreach (var doorRect in borderDoors)
                        {
                            var absoluteDoorRect = ConvertToAbsoluteDoorRect(doorRect, startX, startY);
                            var buildCondidate = new BuildCondidate(randomRoom, doorRect, absoluteDoorRect, borderDoors.Direction);

                            levelMapSets[level].BuildedCondidates.Enqueue(buildCondidate);
                        }
                    }
                }
                else
                    return;

            }

            var mapSize = CalculateBuildedMapSize(currMap.Count);
            var currSize = 0;

            var doorCondidates = levelMapSets[level].BuildedCondidates;


            // TODO : 맵 전체의 Door후보와 현재 만드는 던전맵의 Door후보를 따로 나눠서 계산해야할 듯
            while (mapSize > currSize++ && doorCondidates.Count > 0)
            {
                var selectedCondidate = doorCondidates.Dequeue();
                var selectedRoomInfo = currMap.GetRoomWithMatchedDoor(selectedCondidate.AbsoluteDoorRect, selectedCondidate.Direction);

                var selectedRoom = selectedRoomInfo.selectedRoom;
                var relatvieDoorRect = selectedRoomInfo.relativeBorderDoorRect;
                var absoluteRoomRect = selectedRoomInfo.absoulteRoomRect;

                Build(selectedRoom, (int)absoluteRoomRect.x, (int)absoluteRoomRect.y);

                // TODO : DoorGorup을 제거해야함
                var doorTiles = selectedRoom.DoorGroup.GetDoorTiles((int)relatvieDoorRect.x, (int)relatvieDoorRect.y);

                Remove(doorTiles, (int)absoluteRoomRect.x, (int)absoluteRoomRect.y);
            }

        }

        int CalculateBuildedMapSize(int roomCount)
        {
            var minMapSize = Mathf.RoundToInt(roomCount * .5f);
            minMapSize = minMapSize <= 0 ? 1 : minMapSize;

            var maxMapSize = Mathf.RoundToInt(roomCount * 1.5f);

            var mapSize = UnityEngine.Random.Range(minMapSize, maxMapSize);
            return mapSize;
        }

        void Build(Room room, int pivotX, int pivotY)
        {
            for (int x = 0; x < room.Width; x++)
            {
                for (int y = 0; y < room.Height; y++)
                {
                    var worldTile = world.GetTile(pivotX + x, pivotY - y);
                    if (worldTile == null)
                        continue;

                    var roomTile = room.GetTile(x, y);

                    if (roomTile == null)
                        continue;

                    

                    worldTile.BuildBlock(roomTile.BlockInfo);
                }
            }
        }

        // TODO : 제거시를 만들어야함
        void Remove(Room.Tile[] doorTiles, int pivotX, int pivotY)
        {
            foreach (var doorTile in doorTiles)
            {
                var absoluteX = pivotX + doorTile.X;
                var absoluteY = pivotY - doorTile.Y;

                var worldTile = world.GetTile(absoluteX, absoluteY);
                worldTile?.DestroyBlock();
            }
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

        Rect ConvertToAbsoluteDoorRect(Rect relativePosition, int worldRoomX, int worldRoomY)
        {
            var absolutePos = new Rect(relativePosition);
            absolutePos.x = worldRoomX + absolutePos.x;
            absolutePos.y = worldRoomY - absolutePos.y;

            return absolutePos;
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

            public Rect RelativeDoorRect { get; }
            public Rect AbsoluteDoorRect { get; }

            public Room.BorderDoorsGroup.DoorDirection Direction { get; }

            public BuildCondidate(Room room, Rect relativeDoorRect, Rect absoluteDoorRect, Room.BorderDoorsGroup.DoorDirection direction)
            {
                Room = room;
                RelativeDoorRect = relativeDoorRect;
                AbsoluteDoorRect = absoluteDoorRect;
                Direction = direction;
            }
        }


    }
}

