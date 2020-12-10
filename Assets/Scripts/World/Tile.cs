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

        int x;
        public int X => x;

        int y;
        public int Y => y;

        Vector3Int Vector3Int => new Vector3Int(x, y, 0);

        Tile[] crossNeighborTiles;
        Tile[] neighborTiles;

        Block block;

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

        public void OnCreatedBlock(BlockInfo blockInfo)
        {
            if (block != null)
                return;

            block = Block.Create(blockInfo);

            var craeteBlockArgs = new CreateBlcokObjectArgs(x, y, block);
            var changeBlockSpriteArgs = new ChangeBlockSpriteArgs(Vector3Int, block.BlockInfo.TileSet.Default);

            createdBlockEvent?.Invoke(this, craeteBlockArgs);
            changedBlockSpriteEvent?.Invoke(this, changeBlockSpriteArgs);
            OnChangedBlockSprite(block);
        }

        void OnChangedBlockSprite(Block block)
        {
            byte mask = 0x00;
            byte dirNeighborMask = 0x01;

            byte neighborMask = 0x80;

            ChangeBlockSpriteArgs e;

            foreach (var neighborTile in neighborTiles)
            {
                if (neighborTile?.block?.BlockInfo.Name == block.BlockInfo.Name)
                {

                    if (!block.BlockInfo.TileSet.IsOnlyCross || (block.BlockInfo.TileSet.IsOnlyCross && crossNeighborTiles.Contains(neighborTile)))
                    {
                        neighborTile.block.Mask |= neighborMask;

                        e = new ChangeBlockSpriteArgs(neighborTile.Vector3Int, neighborTile.block.BlockInfo.TileSet[neighborTile.block.Mask]);
                        changedBlockSpriteEvent?.Invoke(this, e);

                        mask |= dirNeighborMask;
                    }

                }

                dirNeighborMask <<= 1;
                neighborMask >>= 1;
            }

            if (block.BlockInfo.TileSet.IsOnlyCross)
                mask &= 0b_0101_1010;

            e = new ChangeBlockSpriteArgs(Vector3Int, block.BlockInfo.TileSet[mask]);
            changedBlockSpriteEvent?.Invoke(this, e);

            block.Mask = mask;
        }

    }
}
