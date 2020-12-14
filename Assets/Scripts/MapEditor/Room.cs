using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Unity.Collections;
using System.Collections;
using System.IO.IsolatedStorage;
using System.Runtime.CompilerServices;
using UnityEditor;


using Editor.Dungeon;
using UnityEngine.Rendering;
using JetBrains.Annotations;

[Serializable]
public class Room : ScriptableObject, ISerializationCallbackReceiver
{
    public event Action updateEvent;

    Tile[,] tiles;

    [SerializeField]
    DoorGroup doorGroup;

    [SerializeField]
    Serializable2dArray serializable2dTiles;


    public Tile this[int x, int y]=>GetTile(x,y);

    public int Width => tiles != null ? tiles.GetLength(0) : 0;
    public int Height => tiles != null ? tiles.GetLength(1) : 0;

    public void SetSize(int resizedWidth, int resizedHeight)
    {
        var resizedTiles = CreateEmptyTiles(resizedWidth, resizedHeight);
        if(tiles != null)
            PasteToAnotherTiles(resizedTiles, tiles);

        tiles = resizedTiles;

        doorGroup = DoorGroup.CreateGroup(this);
        updateEvent?.Invoke();
    }

    public void SetTile(BlockInfo blockInfo, int x, int y)
    {
        if (!Inside(x, y))
            return;

        tiles[x, y] = Tile.Create(x, y, blockInfo);
        updateEvent?.Invoke();
    }

    public void SwitchDoor(int x, int y)
    {
        if (!Inside(x, y))
            return;

        tiles[x, y].IsDoor = !tiles[x, y].IsDoor;
        doorGroup = DoorGroup.CreateGroup(this);

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

    public void OnBeforeSerialize()
    {
        serializable2dTiles = new Serializable2dArray(tiles);
    }

    public void OnAfterDeserialize()
    {
        tiles = serializable2dTiles?.To2dArray() as Tile[,];
    }

    [Serializable]
    public class DoorGroup
    {
        Room room;

        [SerializeField]
        List<DoorTiles> doorGroup = new List<DoorTiles>();

        public static DoorGroup CreateGroup(Room room)
        {
            var doorGroup = new DoorGroup(room);
            doorGroup.BuildGroup(room.tiles);

            return doorGroup;
        } 

        DoorGroup(Room room)
        {
            this.room = room;
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

        public static Tile Create(int x, int y, BlockInfo blockInfo)
        {
            var tile = new Tile(x, y, blockInfo);
            return tile;
        }

        public static Tile CreateEmpty(int x, int y)
        {
            return new Tile();
        }

        private Tile() { }

        private Tile(int x, int y, BlockInfo blockInfo) : this(x, y)
        {        
            this.blockInfo = blockInfo;
        }

        private Tile(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    [Serializable]
    class Serializable2dArray
    {
        [SerializeField]
        [HideInInspector]
        Tile[] serializableArray;

        [SerializeField]
        [ReadOnly]
        int xSize;
        public int XSize => xSize;

        [SerializeField]
        [ReadOnly]
        int ySize;
        public int YSize => ySize;

        public int Length => xSize * ySize;

        public Tile this[int x, int y]
        {
            get => Get(x, y);
            set => Set(value, x, y);
        }

        public Serializable2dArray(Tile[,] original2dArray)
            : this(original2dArray?.GetLength(0) ?? 0, original2dArray?.GetLength(1) ?? 0)
        {
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    Set(original2dArray[x, y], x, y);
                }
            }
        }

        public Serializable2dArray(int xSize, int ySize)
        {
            this.xSize = xSize;
            this.ySize = ySize;

            serializableArray = new Tile[xSize * ySize];
        }

        public void Set(Tile value, int x, int y)
        {
            if (!Inside(x, y))
                throw new IndexOutOfRangeException();

            var idx = GetOffsetIndex(x, y);
            serializableArray[idx] = value;
        }

        public Tile Get(int x, int y)
        {
            if (!Inside(x, y))
                throw new IndexOutOfRangeException();

            var idx = GetOffsetIndex(x, y);
            return serializableArray[idx];
        }

        public Tile[,] To2dArray()
        {
            var original2dArray = new Tile[xSize, ySize];
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    original2dArray[x, y] = Get(x, y);
                }
            }

            return original2dArray;
        }

        int GetOffsetIndex(int x, int y)
        {
            return xSize * x + y;
        }

        bool Inside(int x, int y)
        {
            if (x < 0 || x >= xSize || y < 0 || y >= ySize)
                return false;

            return true;
        }
    }


}
