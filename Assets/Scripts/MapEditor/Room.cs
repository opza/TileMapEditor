using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Unity.Collections;
using System.Collections;
using UnityEditor;

using Editor.Dungeon;

using Util.SerializableObjects;

[Serializable]
public class Room : ScriptableObject, ISerializationCallbackReceiver
{
    
    public event Action updateEvent;

    Tile[,] tiles;

    [SerializeField]
    DoorGroup doorGroup;

    [SerializeField]
    DoorHeaderGroup doorHeaderGroup;
    public DoorHeaderGroup HeaderGroup => doorHeaderGroup;

    [SerializeField]
    Serializable2dArray<Tile> serializable2dTiles;


    public Tile this[int x, int y]=>GetTile(x,y);

    public int Width => tiles != null ? tiles.GetLength(0) : 0;
    public int Height => tiles != null ? tiles.GetLength(1) : 0;

    public void SetSize(int resizedWidth, int resizedHeight)
    {
        var resizedTiles = CreateEmptyTiles(resizedWidth, resizedHeight);
        if(tiles != null)
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
        doorGroup = DoorGroup.CreateGroup(this);
        doorHeaderGroup = DoorHeaderGroup.GetDoorHeaders(this);
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
    public class DoorHeaderGroup
    {
        [Serializable]
        public enum DoorDirection { Top, Bottom, Left, Right }

        [SerializeField]
        DoorHeaders doorTopHeaders;
        public DoorHeaders DoorTopHeaders => doorTopHeaders;

        [SerializeField]
        DoorHeaders doorBottomHeaders;
        public DoorHeaders DoorBottomHeaders => doorBottomHeaders;

        [SerializeField]
        DoorHeaders doorLeftHeaders;
        public DoorHeaders DoorLeftHeaders => doorLeftHeaders;

        [SerializeField]
        DoorHeaders doorRightHeaders;
        public DoorHeaders DoorRightHeaders => doorRightHeaders;

        public DoorHeaders this[DoorDirection direction] => GetDoorHeadersForDirection(direction);

        public DoorHeaders[] AllDoorHeaders => new DoorHeaders[]
        {
            doorTopHeaders,
            doorBottomHeaders,
            doorLeftHeaders,
            doorRightHeaders
        };

        public static DoorHeaderGroup GetDoorHeaders(Room room)
        {
            var doorHeaderGroup = new DoorHeaderGroup(room);       
            return doorHeaderGroup;

        }

        public DoorHeaders GetDoorHeadersForDirection(DoorDirection direction)
        {
            switch (direction)
            {
                case DoorDirection.Top: return doorTopHeaders;
                case DoorDirection.Bottom: return doorBottomHeaders;
                case DoorDirection.Left: return doorLeftHeaders;
                case DoorDirection.Right: return doorRightHeaders;
                default: return null;
            }
        }

        public DoorHeaders GetDoorHeadersForOppositeDirecton(DoorDirection direction)
        {
            switch (direction)
            {
                case DoorDirection.Top: return doorBottomHeaders;
                case DoorDirection.Bottom: return doorTopHeaders;
                case DoorDirection.Left: return doorRightHeaders;
                case DoorDirection.Right: return doorLeftHeaders;
                default: return null;
            }
        }

        DoorHeaderGroup(Room room)
        {
            doorTopHeaders = new DoorHeaders(GetRowDoors(room, 0), DoorDirection.Top);
            doorBottomHeaders = new DoorHeaders(GetRowDoors(room, room.Height - 1), DoorDirection.Bottom);
            doorLeftHeaders = new DoorHeaders(GetColumnDoors(room, 0), DoorDirection.Left);
            doorRightHeaders = new DoorHeaders(GetColumnDoors(room, room.Width - 1), DoorDirection.Right);
        }    

        Rect[] GetRowDoors(Room room, int y)
        {
            var rowDoors = new List<Rect>();
            if (room.Width == 1)
            {
                if (room.GetTile(0, y).IsDoor)
                    rowDoors.Add(new Rect(0, y, 1, 1));

                return rowDoors.ToArray();
            }

            var currStartX = 0;
            var lastTile = room.GetTile(0, y);

            for (int x = 1; x < room.Width; x++)
            {
                var tile = room.GetTile(x, y);
                if (tile.IsDoor)
                {
                    if (!lastTile.IsDoor) {
                        currStartX = x;
                        
                        if(x == room.Width - 1)
                            rowDoors.Add(new Rect(currStartX, y, 1, 1));
                    }
                    else
                    {
                        if (x == room.Width - 1)
                            rowDoors.Add(new Rect(currStartX, y, x - currStartX + 1, 1));
                    }

                }
                else
                {
                    if (lastTile.IsDoor)
                        rowDoors.Add(new Rect(currStartX, y, x - currStartX, 1));
                }

                lastTile = tile;
            }

            return rowDoors.ToArray();
        }

        Rect[] GetColumnDoors(Room room, int x)
        {
            var columnDoors = new List<Rect>();
            if(room.Height == 1)
            {
                if (room.GetTile(x, 0).IsDoor)
                    columnDoors.Add(new Rect(x, 0, 1, 1));

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

                        if(y == room.Height - 1)
                            columnDoors.Add(new Rect(x, currStartY, 1, 1));
                    }
                    else
                    {
                        if(y == room.Height - 1)
                            columnDoors.Add(new Rect(x, currStartY, 1, y - currStartY + 1));
                    }
                }
                else
                {
                    if (lastTile.IsDoor)
                        columnDoors.Add(new Rect(x, currStartY, 1, y - currStartY));
                }

                lastTile = tile;
            }

            return columnDoors.ToArray();
        }

        [Serializable]
        public class DoorHeaders : IEnumerable<Rect>
        {
            

            [SerializeField]
            Rect[] doorRects;

           [SerializeField]
            DoorDirection direction;

            public DoorDirection Direction => direction;

            public Rect this[int idx] => doorRects[idx];
            public int Count => doorRects.Count();

            public DoorHeaders(Rect[] doorRects, DoorDirection direction)
            {
                this.doorRects = doorRects;
                this.direction = direction;
            }

            public IEnumerator<Rect> GetEnumerator()
            {
                return ((IEnumerable<Rect>)doorRects).GetEnumerator();
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
