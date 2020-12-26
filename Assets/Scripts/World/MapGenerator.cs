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

        // TODO : 맵 MIN 및 MAX Size를 가져와야함
        readonly int MIN_SIZE = 10;
        readonly int MAX_SIZE = 10;

        List<LevelMapSet> levelMapSets = new List<LevelMapSet>();

        public MapGenerator(World world, List<MapSet> level0Maps, List<MapSet> level1Maps, List<MapSet> level2Maps)
        {
            this.world = world;

            levelMapSets = new List<LevelMapSet>(){
                new LevelMapSet(0, level0Maps),
                new LevelMapSet(1, level1Maps),
                new LevelMapSet(2, level2Maps)
            };
        }

        // TODO : 레벨별 맵 생성 함수를 만들어야함
        public void Generate(int level)
        {
            if (levelMapSets.Count < level)
                return;

            var currMap = levelMapSets[level].MapSet.GetRandomItem();
            if (currMap == null)
                return;
            
            //if(levelMapSets)
            //{
            //    var randomRoom = currMap.GetRandomRoom();
            //    var startX = world.Width / 2;
            //    var startY = world.Height / 2;

            //    Build(randomRoom, startX, startY);

            //    foreach (var doorHeader in randomRoom.HeaderGroup.AllDoorHeaders)
            //    {
            //        foreach (var doorRect in doorHeader)
            //        {
            //            var absoultePosDoorRect = ConvertAbsolutePosition(doorRect, startX, startY);
            //            level1DoorCondidates.Enqueue(absoultePosDoorRect);
            //        }
            //    }
            //}

            // TODO : Door 후보가 없을 때 방을 생성하도록 정의해야함
            if(levelMapSets[level].DoorCondidates.Count <= 0)
            {
                if(level == 0)
                {
                    var randomRoom = currMap.GetRandomRoom();
                    var startX = world.Width / 2;
                    var startY = world.Height / 2;

                    Build(randomRoom, startX, startY);

                    foreach (var doorHeader in randomRoom.HeaderGroup.AllDoorHeaders)
                    {
                        foreach (var doorRect in doorHeader)
                        {
                            var absoultePosDoorRect = ConvertAbsolutePosition(doorRect, startX, startY);
                            levelMapSets[level].DoorCondidates.Enqueue(absoultePosDoorRect);
                        }
                    }
                }
                else if(levelMapSets[level].DoorCondidates.Count <= 0)
            }

            var mapSize = UnityEngine.Random.Range(MIN_SIZE, MAX_SIZE);
            var currSize = 0;

            // TODO : 맵 전체의 Door후보와 현재 만드는 던전맵의 Door후보를 따로 나눠서 계산해야할 듯
            while(mapSize > currSize++)
            {
                
            }

        }

       
        void Build(Room room, int pivotX, int pivotY)
        {
            for (int x = 0; x < room.Width; x++)
            {
                for (int y = 0; y < room.Height; y++)
                {
                    var worldTile = world.GetTile(pivotX + x, pivotY - y);
                    var roomTile = room.GetTile(x, y);

                    worldTile.OnCreatedBlock(roomTile.BlockInfo);
                }
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

        Rect ConvertAbsolutePosition(Rect relativePosition, int worldX, int worldY)
        {
            var absolutePos = new Rect(relativePosition);
            absolutePos.x = worldX;
            absolutePos.y = worldY;

            return absolutePos;
        }

        class LevelMapSet
        {
            int level;

            List<MapSet> mapSet;
            public List<MapSet> MapSet => mapSet;

            Queue<Rect> doorCondidates = new Queue<Rect>();
            public Queue<Rect> DoorCondidates => doorCondidates;

            public LevelMapSet(int level, List<MapSet> mapSet)
            {
                this.mapSet = mapSet;
            }
        }


    }
}
