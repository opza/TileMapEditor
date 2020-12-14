using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using UnityEngine;
using UnityEngine.WSA;

namespace Editor.Dungeon
{
    [CreateAssetMenu(fileName = "DungeonPalette", menuName = "DungeonEditor/Palette")]
    public class Palette : ScriptableObject, IEnumerable<BlockInfo>
    {
        readonly static int MIN_ARRAY_INDEX = 0;

        public event Action updateElementEvent;

        [SerializeField]
        List<BlockInfo> elements = new List<BlockInfo>();

        public BlockInfo this[int i] => elements[i];

        public int Count => elements.Count;

        void Awake()
        {
            var emptyBlockInfo = CreateInstance<BlockInfo>();
            elements.Add(emptyBlockInfo);
        }

        public void Add(BlockInfo blockInfo)
        {
            if (elements.Contains(blockInfo))
                return;

            elements.Add(blockInfo);

            updateElementEvent?.Invoke();
        }

        public void Remove(BlockInfo element)
        {
            var idx = elements.IndexOf(element);
            Remove(idx);
        }

        public void Remove(int idx)
        {
            if (idx <= MIN_ARRAY_INDEX || idx >= elements.Count)
                return;

            elements.RemoveAt(idx);

            updateElementEvent?.Invoke();
        }

        public IEnumerator<BlockInfo> GetEnumerator()
        {
            return elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return elements.GetEnumerator();
        }

           
    }
}
