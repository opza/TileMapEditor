﻿using UnityEngine;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System;

using Util;
using UnityEngine.Tilemaps;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Worlds
{
    [CreateAssetMenu]
    public class TileSet : ScriptableObject
    {
        [SerializeField]
        string tileName;
        public string TileName => tileName;

        [SerializeField]
        bool isOnlyCross;
        public bool IsOnlyCross => isOnlyCross;

        [SerializeField]
        List<TileSetElement> tileSetElements = new List<TileSetElement>();

        public Sprite this[byte mask] => GetTileElement(mask);
        public Sprite Default => GetTileElement(0);

        public Sprite GetTileElement(byte mask)
        {
            for (int i = 0; i < tileSetElements.Count; i++)
            {
                var tileSetElement = tileSetElements[i];
                if (tileSetElement.Mask == mask)
                    return tileSetElement.Sprite;
            }

            throw new ArgumentException($"{Convert.ToString(mask, 2).PadLeft(8,'0')}에 해당하는 요소가 없습니다");
        }

        [Serializable]
        public class TileSetElement : ISerializationCallbackReceiver
        {
            [SerializeField]
            Sprite sprite;
            public Sprite Sprite => sprite;

            [SerializeField]
            bool[] mask;

            public byte Mask => mask.ToByte();

            public TileSetElement()
            {

            }

            void ISerializationCallbackReceiver.OnBeforeSerialize()
            {
                
            }

            void ISerializationCallbackReceiver.OnAfterDeserialize()
            {
                if (mask == null || mask.Length <= 0)
                    mask = new bool[8];
            }
        }

        public class Tilett : UnityEngine.Tilemaps.Tile
        {

        }

    }


}