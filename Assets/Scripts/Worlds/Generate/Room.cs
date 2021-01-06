using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Unity.Collections;
using System.Collections;
using UnityEditor;


using Util.SerializableObjects;

namespace Worlds.Generate
{

    [Serializable]
    public class Room : ScriptableObject, ISerializationCallbackReceiver
    {

        public event Action updateEvent;

        Tile[,] tiles;

        [SerializeField]
        DoorsGroup doorGroup;
        public DoorsGroup DoorGroup => doorGroup;

        [SerializeField]
        BorderDoorsGroup borderDoorGroup;
        public BorderDoorsGroup BorderDoorGroup => borderDoorGroup;

        [SerializeField]
        Serializable2dArray<Tile> serializable2dTiles;


        public Tile this[int x, int y] => GetTile(x, y);

        public int Width => tiles != null ? tiles.GetLength(0) : 0;
        public int Height => tiles != null ? tiles.GetLength(1) : 0;

        public RectInt rect => new RectInt(0, 0, Width, Height);

        public void SetSize(int resizedWidth, int resizedHeight)
        {
            var resizedTiles = CreateEmptyTiles(resizedWidth, resizedHeight);
            if (tiles != null)
                PasteToAnotherTiles(resizedTiles, tiles);

            tiles = resizedTiles;

            UpdateDoorTile();
            updateEvent?.Invoke();
        }

        public void SetTile(BlockInfo blockInfo, int x, int y)
        {
            if (!Inside(x, y))
                return;

            if (blockInfo == null)
                tiles[x, y] = Tile.CreateEmpty(x, y, tiles[x, y].IsDoor);
            else
                tiles[x, y] = Tile.Create(x, y, blockInfo, tiles[x, y].IsDoor);

            updateEvent?.Invoke();
        }

        public void SwitchDoor(int x, int y)
        {
            if (!Inside(x, y))
                return;

            tiles[x, y].IsDoor = !tiles[x, y].IsDoor;

            UpdateDoorTile();
            updateEvent?.Invoke();
        }

        public Tile GetTile(int x, int y)
        {
            if (!Inside(x, y))
                return null;

            return tiles[x, y];
        }

        Tile[,] CreateEmptyTiles(int width, int height)
        {
            var emptyTiles = new Tile[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    emptyTiles[x, y] = Tile.CreateEmpty(x, y);
                }
            }

            return emptyTiles;
        }

        void UpdateDoorTile()
        {
            doorGroup = DoorsGroup.CreateGroup(this);
            borderDoorGroup = BorderDoorsGroup.BuildBorderDoorsGroup(this);
        }

        void PasteToAnotherTiles(Tile[,] targetTiles, Tile[,] copiedTiles)
        {
            var targetTilesWidth = targetTiles.GetLength(0);
            var targetTilesHeight = targetTiles.GetLength(1);
            var copiedTilesWidth = copiedTiles.GetLength(0);
            var copiedTilesHeight = copiedTiles.GetLength(1);


            for (int x = 0; x < copiedTilesWidth; x++)
            {
                if (x >= targetTilesWidth)
                    break;

                for (int y = 0; y < copiedTilesHeight; y++)
                {
                    if (y >= targetTilesHeight)
                        break;

                    targetTiles[x, y] = copiedTiles[x, y];
                }
            }
        }

        bool Inside(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return false;

            return true;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            serializable2dTiles = new Serializable2dArray<Tile>(tiles);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            tiles = serializable2dTiles?.To2dArray();
            UpdateDoorTile();
        }

        [Serializable]
        public class DoorsGroup
        {
            Room room;

            [SerializeField]
            List<DoorTiles> doorGroup = new List<DoorTiles>();

            public static DoorsGroup CreateGroup(Room room)
            {
                var doorGroup = new DoorsGroup(room);
                doorGroup.BuildGroup(room.tiles);

                return doorGroup;
            }

            DoorsGroup(Room room)
            {
                this.room = room;
            }

            public Tile[] GetDoorTiles(int x, int y)
            {
                var tile = room.GetTile(x, y);
                if (tile?.IsDoor != true)
                    return null;

                foreach (var doorTiles in doorGroup)
                {
                    if (doorTiles.Contains(tile))
                        return doorTiles.DoorTilesArray;
                }

                return null;
            }

            public bool Contains(Tile tile)
            {
                foreach (var doorTiles in doorGroup)
                {
                    if (doorTiles.Contains(tile))
                        return true;
                }

                return false;
            }

            void BuildGroup(Tile[,] tiles)
            {
                doorGroup.Clear();

                foreach (var tile in tiles)
                {
                    AddTile(tile);
                }
            }

            void AddTile(Tile tile)
            {
                if (!tile.IsDoor)
                    return;

                if (Contains(tile))
                    return;

                var containableList = FindContainableListAll(tile);
                if (containableList.Length == 0)
                {
                    var newDoorTiles = new DoorTiles();
                    newDoorTiles.Add(tile);

                    doorGroup.Add(newDoorTiles);

                    return;
                }

                if (containableList.Length == 1)
                {
                    containableList[0].Add(tile);

                    return;
                }

                var concatedDoorTile = new DoorTiles();
                concatedDoorTile.Add(tile);

                foreach (var doorTiles in containableList)
                {
                    concatedDoorTile += doorTiles;
                }

                doorGroup.RemoveAll(doorTiles => containableList.Contains(doorTiles));
                doorGroup.Add(concatedDoorTile);

            }

            DoorTiles[] FindContainableListAll(Tile tile)
            {
                var findedDoorTiles = new List<DoorTiles>();

                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        if (x == 0 && y == 0)
                            continue;

                        if (x * y != 0)
                            continue;

                        var neighborTile = room.GetTile(tile.X + x, tile.Y + y);
                        if (neighborTile == null)
                            continue;

                        var condidateDoorTiles = FindDoorTiles(neighborTile);
                        if (condidateDoorTiles == null)
                            continue;

                        findedDoorTiles.Add(condidateDoorTiles);
                    }
                }

                return findedDoorTiles.Distinct().ToArray();
            }

            DoorTiles FindDoorTiles(Tile tile)
            {
                return doorGroup.Where(doorTiles => doorTiles.Contains(tile)).FirstOrDefault();
            }


            [Serializable]
            class DoorTiles
            {
                [SerializeField]
                List<Tile> doorTiles = new List<Tile>();

                public Tile[] DoorTilesArray => doorTiles.ToArray();

                public DoorTiles()
                {

                }

                public DoorTiles(List<Tile> doorTiles)
                {
                    this.doorTiles = doorTiles;
                }

                public void Add(Tile tile)
                {
                    if (doorTiles.Contains(tile))
                        return;

                    doorTiles.Add(tile);
                }

                public void Remove(Tile tile)
                {
                    if (!doorTiles.Contains(tile))
                        return;

                    doorTiles.Remove(tile);
                }

                public bool Contains(Tile tile)
                {
                    return doorTiles.Contains(tile);
                }

                public DoorTiles Concat(DoorTiles otherTiles)
                {
                    return new DoorTiles(doorTiles.Concat(otherTiles.doorTiles).ToList());
                }

                public static DoorTiles operator +(DoorTiles doorTiles1, DoorTiles doorTiles2)
                {
                    return doorTiles1.Concat(doorTiles2);
                }
            }
        }


        [Serializable]
        public class BorderDoorsGroup
        {
            [Serializable]
            public enum DoorDirection { Top, Bottom, Left, Right }

            [SerializeField]
            BorderDoors topBorderDoors;
            public BorderDoors TopBorderDoors => topBorderDoors;

            [SerializeField]
            BorderDoors BottomBorderDoors;
            public BorderDoors bottomBorderDoors => BottomBorderDoors;

            [SerializeField]
            BorderDoors leftBorderDoors;
            public BorderDoors LeftBorderDoors => leftBorderDoors;

            [SerializeField]
            BorderDoors rightBorderDoors;
            public BorderDoors RightBorderDoors => rightBorderDoors;

            public BorderDoors this[DoorDirection direction] => GetBorderDoorsForDirection(direction);

            public BorderDoors[] AllBorderDoors => new BorderDoors[]
            {
                topBorderDoors,
                BottomBorderDoors,
                leftBorderDoors,
                rightBorderDoors
            };

            public static BorderDoorsGroup BuildBorderDoorsGroup(Room room)
            {
                var doorHeaderGroup = new BorderDoorsGroup(room);
                return doorHeaderGroup;

            }

            BorderDoorsGroup(Room room)
            {
                topBorderDoors = new BorderDoors(GetRowDoors(room, 0), DoorDirection.Top);
                BottomBorderDoors = new BorderDoors(GetRowDoors(room, room.Height - 1), DoorDirection.Bottom);
                leftBorderDoors = new BorderDoors(GetColumnDoors(room, 0), DoorDirection.Left);
                rightBorderDoors = new BorderDoors(GetColumnDoors(room, room.Width - 1), DoorDirection.Right);
            }

            public BorderDoors GetBorderDoorsForDirection(DoorDirection direction)
            {
                switch (direction)
                {
                    case DoorDirection.Top: return topBorderDoors;
                    case DoorDirection.Bottom: return BottomBorderDoors;
                    case DoorDirection.Left: return leftBorderDoors;
                    case DoorDirection.Right: return rightBorderDoors;
                    default: return null;
                }
            }

            public BorderDoors GetBorderDoorsForOppositeDirecton(DoorDirection direction)
            {
                switch (direction)
                {
                    case DoorDirection.Top: return BottomBorderDoors;
                    case DoorDirection.Bottom: return topBorderDoors;
                    case DoorDirection.Left: return rightBorderDoors;
                    case DoorDirection.Right: return leftBorderDoors;
                    default: return null;
                }
            }


            RectInt[] GetRowDoors(Room room, int y)
            {
                var rowDoors = new List<RectInt>();
                if (room.Width == 1)
                {
                    if (room.GetTile(0, y).IsDoor)
                        rowDoors.Add(new RectInt(0, y, 1, 1));

                    return rowDoors.ToArray();
                }

                var currStartX = 0;
                var lastTile = room.GetTile(0, y);

                for (int x = 1; x < room.Width; x++)
                {
                    var tile = room.GetTile(x, y);
                    if (tile.IsDoor)
                    {
                        if (!lastTile.IsDoor)
                        {
                            currStartX = x;

                            if (x == room.Width - 1)
                                rowDoors.Add(new RectInt(currStartX, y, 1, 1));
                        }
                        else
                        {
                            if (x == room.Width - 1)
                                rowDoors.Add(new RectInt(currStartX, y, x - currStartX + 1, 1));
                        }

                    }
                    else
                    {
                        if (lastTile.IsDoor)
                            rowDoors.Add(new RectInt(currStartX, y, x - currStartX, 1));
                    }

                    lastTile = tile;
                }

                return rowDoors.ToArray();
            }

            RectInt[] GetColumnDoors(Room room, int x)
            {
                var columnDoors = new List<RectInt>();
                if (room.Height == 1)
                {
                    if (room.GetTile(x, 0).IsDoor)
                        columnDoors.Add(new RectInt(x, 0, 1, 1));

                    return columnDoors.ToArray();
                }

                var currStartY = 0;
                var lastTile = room.GetTile(x, 0);

                for (int y = 1; y < room.Height; y++)
                {
                    var tile = room.GetTile(x, y);
                    if (tile.IsDoor)
                    {
                        if (!lastTile.IsDoor)
                        {
                            currStartY = y;

                            if (y == room.Height - 1)
                                columnDoors.Add(new RectInt(x, currStartY, 1, 1));
                        }
                        else
                        {
                            if (y == room.Height - 1)
                                columnDoors.Add(new RectInt(x, currStartY, 1, y - currStartY + 1));
                        }
                    }
                    else
                    {
                        if (lastTile.IsDoor)
                            columnDoors.Add(new RectInt(x, currStartY, 1, y - currStartY));
                    }

                    lastTile = tile;
                }

                return columnDoors.ToArray();
            }

            [Serializable]
            public class BorderDoors : IEnumerable<RectInt>
            {

                [SerializeField]
                RectInt[] doorRects;

                [SerializeField]
                DoorDirection direction;

                public DoorDirection Direction => direction;

                public RectInt this[int idx] => doorRects[idx];
                public int Count => doorRects.Count();

                public BorderDoors(RectInt[] doorRects, DoorDirection direction)
                {
                    this.doorRects = doorRects;
                    this.direction = direction;
                }

                public IEnumerator<RectInt> GetEnumerator()
                {
                    return ((IEnumerable<RectInt>)doorRects).GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return doorRects.GetEnumerator();
                }
            }
        }

        [Serializable]
        public class Tile
        {
            [SerializeField]
            int x;

            public int X => x;

            [SerializeField]
            int y;

            public int Y => y;

            [SerializeField]
            BlockInfo blockInfo;

            public BlockInfo BlockInfo => blockInfo;

            [SerializeField]
            bool isDoor;

            public bool IsDoor
            {
                get => isDoor;
                set => isDoor = value;
            }

            public static Tile Create(int x, int y, BlockInfo blockInfo, bool isDoor = false)
            {
                var tile = new Tile(x, y, blockInfo, isDoor);
                return tile;
            }

            public static Tile CreateEmpty(int x, int y, bool isDoor = false)
            {
                return new Tile(x, y, isDoor);
            }

            private Tile(int x, int y, BlockInfo blockInfo, bool isDoor = false) : this(x, y, isDoor)
            {
                this.blockInfo = blockInfo;
            }

            private Tile(int x, int y, bool isDoor = false)
            {
                this.x = x;
                this.y = y;
                this.isDoor = isDoor;
            }
        }




    }
}
