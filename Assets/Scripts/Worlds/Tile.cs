using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Util;

namespace Worlds
{
    public class Tile
    {
        public static event EventHandler<ChangeBlockSpriteArgs> changedBlockSpriteEvent;
        public static event EventHandler<CreateBlcokObjectArgs> createdBlockEvent;
        public static event EventHandler<DestrotyBlockOjectArgs> destroyedBlockEvent;

        int x;
        public int X => x;

        int y;
        public int Y => y;

        Vector3Int Vector3Int => new Vector3Int(x, y, 0);

        Tile[] crossNeighborTiles;
        Tile[] neighborTiles;

        Block block;

        public bool HasBlock => block != null;

        public Tile(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public void SetNeighborTiles(Tile[] neighborTiles)
        {
            if (neighborTiles.Length != 8)
                return;

            this.neighborTiles = neighborTiles;

            crossNeighborTiles = new Tile[4]
            {
                neighborTiles[1],
                neighborTiles[3],
                neighborTiles[4],
                neighborTiles[6]
            };
        }

        public void BuildBlock(BlockInfo blockInfo)
        {
            if (blockInfo == null)
                return;

            if (HasBlock)
                return;

            block = Block.Create(blockInfo);

            var createBlockArgs = new CreateBlcokObjectArgs(x, y, block);           
            createdBlockEvent?.Invoke(this, createBlockArgs);

            ChangeCreateBlockSprite();
            ChangeCreateNeighborBlockSprite();
        }

        public void DestroyBlock()
        {
            if (!HasBlock)
                return;       

            var destroyBlockArgs = new DestrotyBlockOjectArgs(x, y, block);           
            destroyedBlockEvent?.Invoke(this, destroyBlockArgs);

            block = null;

            ChangeDestroyBlockSprite();
            ChangeDestroyNeighborBlockSprite();
        }

        #region Change CreateSprite

        void ChangeCreateBlockSprite()
        {
            if (!HasBlock)
                return;

            byte thisTileMask = 0x00;
            byte neighborDirMask = 0x01;


            foreach (var neighborTile in neighborTiles)
            {
                if (neighborTile?.block?.BlockInfo.Name == block.BlockInfo.Name)
                {

                    if (!block.BlockInfo.TileSet.IsOnlyCross || (block.BlockInfo.TileSet.IsOnlyCross && crossNeighborTiles.Contains(neighborTile)))
                    {
                        thisTileMask |= neighborDirMask;
                    }
                }
                neighborDirMask <<= 1;
            }

            if (block.BlockInfo.TileSet.IsOnlyCross)
                thisTileMask &= 0b_0101_1010;

            block.Mask = thisTileMask;

            var e = new ChangeBlockSpriteArgs(Vector3Int, block.BlockInfo.TileSet[thisTileMask]);
            changedBlockSpriteEvent?.Invoke(this, e);            
        }

        void ChangeCreateNeighborBlockSprite()
        {
            if (!HasBlock)
                return;

            byte neighborDirMask = 0x01;

            byte neighborMask = 0x80;

            foreach (var neighborTile in neighborTiles)
            {
                if (neighborTile?.block?.BlockInfo.Name == block.BlockInfo.Name)
                {

                    if (!block.BlockInfo.TileSet.IsOnlyCross || (block.BlockInfo.TileSet.IsOnlyCross && crossNeighborTiles.Contains(neighborTile)))
                    {
                        neighborTile.block.Mask |= neighborMask;

                        var e = new ChangeBlockSpriteArgs(neighborTile.Vector3Int, neighborTile.block.CurrentSprite);
                        changedBlockSpriteEvent?.Invoke(this, e);
                    }

                }

                neighborDirMask <<= 1;
                neighborMask >>= 1;
            }
        }

        #endregion

        #region Change DestroySprite

        void ChangeDestroyBlockSprite()
        {
            if (HasBlock)
                return;

            var changeBlockSpriteArgs = new ChangeBlockSpriteArgs(Vector3Int, null);
            changedBlockSpriteEvent?.Invoke(this, changeBlockSpriteArgs);
        }

        void ChangeDestroyNeighborBlockSprite()
        {
            if (HasBlock)
                return;

            byte neighborDirMask = 0x7F;

            foreach (var neighborTile in neighborTiles)
            {
                if (neighborTile?.block != null)
                {
                    neighborTile.block.Mask &= neighborDirMask;

                    var e = new ChangeBlockSpriteArgs(neighborTile.Vector3Int, neighborTile.block.CurrentSprite);
                    changedBlockSpriteEvent?.Invoke(this, e);
                }

                neighborDirMask >>= 1;
                neighborDirMask |= 0x80;
            }
        }

        #endregion

    }
}
