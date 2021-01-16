using UnityEngine;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System;

using Util;

namespace Worlds
{
    [CreateAssetMenu]
    public class TileSet : ScriptableObject
    {
        [SerializeField]
        string tileName;
        public string TileName => tileName;

        [SerializeField]
        bool onlyCross;
        public bool OnlyCross => onlyCross;

        [SerializeField]
        List<TileSetElement> tileSetElements = new List<TileSetElement>();
        

        public Sprite this[byte mask] => GetTileElement(mask);
        public Sprite Default => tileSetElements[0]?.Sprite;

        public void AddEmpty() => tileSetElements.Add(new TileSetElement());

        public void Add(bool[] mask, Sprite sprite) => tileSetElements.Add(new TileSetElement(mask, sprite));

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= tileSetElements.Count)
                return;

            tileSetElements.RemoveAt(index);
        }

        public Sprite GetTileElement(byte mask)
        {
            for (int i = 0; i < tileSetElements.Count; i++)
            {
                var tileSetElement = tileSetElements[i];
                if (tileSetElement.Mask == mask)
                    return tileSetElement.Sprite;
            }

            //Debug.LogError($"{Convert.ToString(mask, 2).PadLeft(8, '0')}에 해당하는 요소가 없습니다");
            return Default;
            
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
                mask = new bool[8];
            }

            public TileSetElement(bool[] mask, Sprite sprite)
            {
                this.mask = mask;
                this.sprite = sprite;
            }

            void ISerializationCallbackReceiver.OnBeforeSerialize()
            {
                maskInit();
            }

            void ISerializationCallbackReceiver.OnAfterDeserialize()
            {
                maskInit();
            }

            void maskInit()
            {
                if (mask == null || mask.Length <= 0)
                    mask = new bool[8];
            }
        }

    }


}
